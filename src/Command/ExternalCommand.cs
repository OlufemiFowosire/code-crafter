using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Threading.Tasks;

internal class ExternalCommand(string commandName) : ICommand
{
    public string Name => commandName;

    public async Task ExecuteAsync(string[] args, Stream? stdin, Stream? stdout, Stream? stderr)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = commandName,
            UseShellExecute = false,
            // FIX: Only redirect input if we were given a specific stream (like a pipe).
            // If stdin is null, we inherit the Console input directly.
            RedirectStandardInput = stdin != null,
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
            Stream targetOut = stdout ?? Console.OpenStandardOutput();
            outputTasks.Add(Task.Run(async () =>
            {
                try
                {
                    await process.StandardOutput.BaseStream.CopyToAsync(targetOut);
                    await targetOut.FlushAsync();
                }
                catch (IOException) { }
            }));

            // 3. Pump Error
            Stream targetErr = stderr ?? Console.OpenStandardError();
            outputTasks.Add(Task.Run(async () =>
            {
                try
                {
                    await process.StandardError.BaseStream.CopyToAsync(targetErr);
                    await targetErr.FlushAsync();
                }
                catch (IOException) { }
            }));

            await process.WaitForExitAsync();
            await Task.WhenAll(outputTasks);
        }
        catch (Win32Exception ex)
        {
            throw new Win32Exception($"{ex.Message}");
        }
    }
}