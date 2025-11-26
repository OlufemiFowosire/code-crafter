using System.ComponentModel;
using System.Diagnostics;

internal class ExternalCommand(string commandName) : ICommand
{
    public string Name { get; } = commandName;

    public void Execute(string[] args)
    {
        var processInfo = new ProcessStartInfo
        {
            FileName = commandName, // FIX: Use the resolved full path
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };

        foreach (var arg in args)
        {
            processInfo.ArgumentList.Add(arg);
        }

        try
        {
            using var process = new Process { StartInfo = processInfo };

            // FIX: Handle output asynchronously to prevent deadlocks 
            // and allow real-time printing.
            process.OutputDataReceived += (sender, e) =>
            {
                if (e.Data != null) Console.WriteLine(e.Data);
            };

            // FIXED: This pipes Error -> Error -> Terminal
            process.ErrorDataReceived += (sender, e) =>
            {
                if (e.Data != null) Console.Error.WriteLine(e.Data);
            };

            process.Start();

            // Start listening to the streams
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            process.WaitForExit();
        }
        catch (Win32Exception)
        {
            throw new Win32Exception($"{commandName}: command not found");
        }
    }
}