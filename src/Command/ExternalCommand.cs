using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
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

        // Pass through environment variables if needed
        // startInfo.Environment["TERM"] = "xterm-256color";

        try
        {
            process.Start();

            var ioTasks = new List<Task>();

            // 1. Pump Input (Shell -> Process)
            if (stdin != Stream.Null)
            {
                ioTasks.Add(Task.Run(async () =>
                {
                    try
                    {
                        await stdin.CopyToAsync(process.StandardInput.BaseStream);
                    }
                    catch (IOException) { } // Broken pipe ignorable here
                    finally { process.StandardInput.Close(); }
                }));
            }
            else
            {
                process.StandardInput.Close();
            }

            // 2. Pump Output (Process -> Shell)
            ioTasks.Add(Task.Run(async () =>
            {
                try { await process.StandardOutput.BaseStream.CopyToAsync(stdout); }
                catch (IOException) { }
            }));

            // 3. Pump Error (Process -> Shell)
            ioTasks.Add(Task.Run(async () =>
            {
                try { await process.StandardError.BaseStream.CopyToAsync(stderr); }
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
        }
    }
}