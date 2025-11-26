using System.Diagnostics;
internal class ExternalCommand(string commandName) : ICommand
{
    public string Name { get; } = "external";
    public int Execute(string[] args)
    {
        Console.WriteLine($"Executing external command...{commandName}");
        string? path = ExecutableDirectories.GetProgramPath(commandName);
        Console.WriteLine($"Path found: {path}");
        if (path != null)
        {
            var process = new Process();
            process.StartInfo.FileName = commandName;
            process.StartInfo.Arguments = string.Join(" ", args);
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = false;
            process.StartInfo.RedirectStandardError = false;
            process.Start();
            process.WaitForExit();
            Console.WriteLine($"{commandName} is {path}");
            return process.ExitCode;
        }
        else
        {
            Console.WriteLine($"{commandName}: command not found");
            return -1;
        }
    }
}