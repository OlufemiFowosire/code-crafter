internal class CommandsFactory
{

    public static void Handle(string? commandName, string[] args)
    {
        TryCreateCommand(commandName, out ICommand? command);
        command!.Execute(args);
    }
    private static void TryCreateCommand(string? commandName, out ICommand? command)
    {
        command = commandName switch
        {
            "exit" => new ExitCommand(),
            "echo" => new EchoCommand(),
            "pwd" => new PwdCommand(),
            "cd" => new CdCommand(),
            "type" => new TypeCommand(GetBuiltins()),
            _ => new ExternalCommand(commandName!),
        };
    }

    public static Dictionary<string, int> GetBuiltins()
    {
        return new Dictionary<string, int>()
        {
            { "exit", 0 },
            { "echo", 0 },
            { "pwd", 0 },
            { "cd", 0 },
            { "type", 0 }
        };
    }
}