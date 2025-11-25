using System.Diagnostics;
using System.IO;
class Program
{
    static void Main()
    {
        var validCommands = new Dictionary<string, IBuiltinCommand>()
        {
            ["exit"] = new ExitCommand(),
            ["echo"] = new EchoCommand()
        };
        var executableDirectories = new ExecutableDirectories();
        validCommands["type"] = new TypeCommand(validCommands, executableDirectories);

        while (true)
        {
            // TODO: Uncomment the code below to pass the first stage
            Console.Write("$ ");

            // Captures the user's command in the "command" variable
            string? input = Console.ReadLine()?.Trim();

            if (string.IsNullOrEmpty(input))
            {
                break;
            }
            string[] tokens = input.Split(' ') ?? [$"{input}"];
            string command = tokens[0];
            if (!validCommands.ContainsKey(command))
            {
                string? path = executableDirectories.GetProgramPath(command);
                if (path != null)
                {
                    var process = new Process();
                    process.StartInfo.FileName = path;
                    process.StartInfo.Arguments = string.Join(' ', tokens.Skip(1));
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.RedirectStandardOutput = false;
                    process.StartInfo.RedirectStandardError = false;
                    process.Start();
                    process.WaitForExit();
                    continue;
                }
                else
                {
                    Console.WriteLine($"{command}: command not found");
                    continue;
                }
            }
            validCommands[command].Execute(tokens.Skip(1).ToArray());
        }
    }
}
