class Program
{
    static void Main()
    {
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
                case "help":
                    Console.WriteLine("Available commands: help, exit, echo [text]");
                    continue;
                case "exit":
                    return;
                case "echo":
                    // Handled separately below
                    string echoOutput = command.Length < 5 ? "" : command.Substring(5); // Get everything after "echo "
                    Console.WriteLine(echoOutput);
                    continue;
                case "type":
                    string typeArg = command.Length < 5 ? "" : command.Substring(5).TrimEnd().ToLower(); // Get everything after "type "
                    switch (typeArg)
                    {
                        case "help":
                        case "exit":
                        case "echo":
                        case "type":
                            Console.WriteLine($"{typeArg} is a shell builtin");
                            continue;
                        case "":
                            Console.WriteLine("type: not enough arguments");
                            continue;
                        default:
                            Console.WriteLine($"{typeArg}: not found");
                            continue;
                    }
                default:
                    Console.WriteLine($"{command}: command not found");
                    continue;
            }
        }
    }
}
