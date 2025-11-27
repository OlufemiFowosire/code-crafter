using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Threading.Tasks;

internal class ExternalCommand(string commandName) : ICommand
{
    public string Name { get; } = commandName;

    public async Task ExecuteAsync(string[] args, Stream? stdin, Stream stdout, Stream stderr)
    {
        string? fullPath = ExecutableDirectories.GetProgramPath(commandName);
        if (fullPath == null) throw new Win32Exception($"{commandName}: command not found");

        var startInfo = new ProcessStartInfo
        {
            FileName = fullPath,
            UseShellExecute = false,
            // FIX: Only redirect input if we were given a specific stream (like a pipe).
            // If stdin is null, we inherit the Console input directly.
            RedirectStandardInput = (stdin != Stream.Null),
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };

        foreach (var arg in args) startInfo.ArgumentList.Add(arg);

        using var process = new Process { StartInfo = startInfo };

        try
        {
            process.Start();

            var outputTasks = new List<Task>();

            // 1. Pump Input (Only if redirected)
            if (stdin != null)
            {
                // We fire-and-forget this because we can't easily wait for it if the source is infinite
                _ = Task.Run(async () =>
                {
                    try
                    {
                        if (stdin != Stream.Null)
                            await stdin.CopyToAsync(process.StandardInput.BaseStream);
                    }
                    catch (IOException) { }
                    finally { process.StandardInput.Close(); }
                });
            }

            // 2. Pump Output
            outputTasks.Add(Task.Run(async () =>
            {
                try
                {
                    await process.StandardOutput.BaseStream.CopyToAsync(stdout);
                    await stdout.FlushAsync();
                }
                catch (IOException) { }
            }));

            // 3. Pump Error
            outputTasks.Add(Task.Run(async () =>
            {
                try
                {
                    await process.StandardError.BaseStream.CopyToAsync(stderr);
                    await stderr.FlushAsync();
                }
                catch (IOException) { }
            }));

            await process.WaitForExitAsync();
            await Task.WhenAll(outputTasks);
        }
        catch (Win32Exception ex)
        {
            using var writer = new StreamWriter(stderr, leaveOpen: true);
            await writer.WriteLineAsync($"{ex.Message}");
            await writer.FlushAsync();
        }
    }
}