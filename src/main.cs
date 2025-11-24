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
        var typeCommand = new TypeCommand(validCommands, executableDirectories);
        validCommands["type"] = typeCommand;

        while (true)
        {
            // TODO: Uncomment the code below to pass the first stage
            Console.Write("$ ");

            // Captures the user's command in the "command" variable
            string? command = Console.ReadLine()?.Trim();

            if (string.IsNullOrEmpty(command))
            {
                break;
            }
            string token = command.Split(' ')[0].ToLower() ?? $"{command}";
            switch (token)
            {
                case "exit":
                    validCommands["exit"].Execute(command.Split(' '));
                    return;
                case "echo":
                    // Handled separately below
                    string echoOutput = command.Length < 5 ? "" : command.Substring(5); // Get everything after "echo "
                    validCommands["echo"].Execute(new string[] { "echo", echoOutput });
                    continue;
                case "type":
                    string typeArg = command.Length < 5 ? "" : command.Substring(5).TrimEnd().ToLower(); // Get everything after "type "
                    typeCommand.Execute(new string[] { "type", typeArg });
                    continue;
                default:
                    Console.WriteLine($"{command}: command not found");
                    continue;
            }
        }
    }
}
