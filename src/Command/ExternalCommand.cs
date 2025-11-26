using System.Diagnostics;

internal class ExternalCommand(string commandName) : ICommand
{
    public string Name { get; } = commandName;

    public void Execute(string[] args)
    {
        string? path = ExecutableDirectories.GetProgramPath(commandName);

        if (path == null)
        {
            Console.WriteLine($"{commandName}: command not found");
            return;
        }

        var processInfo = new ProcessStartInfo
        {
            FileName = path, // FIX: Use the resolved full path
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };

        foreach (var arg in args)
        {
            processInfo.ArgumentList.Add(arg);
        }

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
}