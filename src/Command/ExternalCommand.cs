using System.ComponentModel;
using System.Diagnostics;

internal class ExternalCommand(string commandName) : ICommand
{
    public string Name { get; } = commandName;

    public async Task ExecuteAsync(string[] args, Stream? stdin, Stream stdout, Stream stderr)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = commandName,
            UseShellExecute = false,
            RedirectStandardInput = (stdin != null),
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };

        foreach (var arg in args) startInfo.ArgumentList.Add(arg);

        using var process = new Process { StartInfo = startInfo };

        try
        {
            process.Start();

            // We track Output/Error tasks to ensure we capture all text.
            var outputTasks = new List<Task>();

            // 1. Pump Input (Fire and Forget)
            // Do NOT add this to outputTasks. We don't want to wait for it.
            _ = Task.Run(async () =>
            {
                if (stdin != Stream.Null)
                {
                    try
                    {
                        await stdin.CopyToAsync(process.StandardInput.BaseStream);
                    }
                    catch (IOException) { }
                    finally { process.StandardInput.Close(); }
                }
                else
                {
                    process.StandardInput.Close();
                }
            });

            // 2. Pump Output
            outputTasks.Add(Task.Run(async () =>
            {
                try
                {
                    await process.StandardOutput.BaseStream.CopyToAsync(stdout);
                    // Force flush to ensure data is sent immediately
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

            // A. Wait for the process to exit primarily
            await process.WaitForExitAsync();

            // B. Then wait for the output streams to finish draining
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