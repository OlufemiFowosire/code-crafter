using System.Diagnostics;
internal class ExternalCommand(string commandName) : ICommand
{
    public string Name { get; } = "external";
    public void Execute(string[] args)
    {
        //Console.WriteLine($"Executing external command...{commandName}");
        string? path = ExecutableDirectories.GetProgramPath(commandName);
        //Console.WriteLine($"Path found: {path}");
        if (path != null)
        {
            /* var process = new Process();
            process.StartInfo.FileName = commandName;
            process.StartInfo.Arguments = string.Join(" ", args);
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = false;
            process.StartInfo.RedirectStandardError = false;
            process.Start();
            process.WaitForExit();
            Console.WriteLine($"{commandName} is {path}");
            return process.ExitCode; */
            var processInfo = new ProcessStartInfo
            {
                FileName = commandName,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            // CORRECT WAY: Add arguments one by one. 
            // .NET handles spaces/escaping automatically here.
            foreach (var arg in args)
            {
                processInfo.ArgumentList.Add(arg);
            }
            var process = Process.Start(processInfo);

            // Ensure we capture output to print it to the shell
            process!.WaitForExit();
            Console.Write(process.StandardOutput.ReadToEnd());
            Console.Write(process.StandardError.ReadToEnd());
        }
        else
        {
            Console.WriteLine($"{commandName}: command not found");
        }
    }
}