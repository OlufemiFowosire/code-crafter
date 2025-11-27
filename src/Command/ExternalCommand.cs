using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes; // Added for PipeStream check
using System.Threading.Tasks;

internal class ExternalCommand(string commandName) : ICommand
{
    public string Name { get; } = commandName;

    public async Task ExecuteAsync(string[] args, Stream stdin, Stream stdout, Stream stderr)
    {
        // Resolve full path (using your existing logic)
        string? fullPath = ExecutableDirectories.GetProgramPath(commandName);
        if (fullPath == null)
        {
            throw new Win32Exception($"{commandName}: command not found");
        }

        var startInfo = new ProcessStartInfo
        {
            FileName = fullPath,
            UseShellExecute = false,
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };

        foreach (var arg in args) startInfo.ArgumentList.Add(arg);

        using var process = new Process { StartInfo = startInfo };

        try
        {
            process.Start();

            var ioTasks = new List<Task>();

            // 1. Pump Input (Shell -> Process)
            ioTasks.Add(Task.Run(async () =>
            {
                if (stdin != Stream.Null)
                {
                    try
                    {
                        await stdin.CopyToAsync(process.StandardInput.BaseStream);
                    }
                    catch (IOException) { } // Broken pipe ignorable here
                    finally { process.StandardInput.Close(); }
                }
            }));

            // 2. Pump Output (Process -> Shell)
            ioTasks.Add(Task.Run(async () =>
            {
                try
                {
                    await process.StandardOutput.BaseStream.CopyToAsync(stdout);

                    // CRITICAL FIX: Only flush if it is a Pipe. 
                    // Flushing ConsoleStream (System.IO.Stream) can cause hangs/errors in test runners.
                    if (stdout is PipeStream)
                    {
                        await stdout.FlushAsync();
                    }
                }
                catch (IOException) { }
            }));

            // 3. Pump Error (Process -> Shell)
            ioTasks.Add(Task.Run(async () =>
            {
                try
                {
                    await process.StandardError.BaseStream.CopyToAsync(stderr);
                    if (stderr is PipeStream)
                    {
                        await stderr.FlushAsync();
                    }
                }
                catch (IOException) { }
            }));

            // Wait for IO and Exit
            await Task.WhenAll(ioTasks);
            await process.WaitForExitAsync();
        }
        catch (Win32Exception ex)
        {
            // Write to the error stream provided by the pipeline
            using var writer = new StreamWriter(stderr, leaveOpen: true);
            await writer.WriteLineAsync($"{commandName}: {ex.Message}");
            await writer.FlushAsync();
        }
    }
}