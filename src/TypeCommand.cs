internal class TypeCommand(Dictionary<string, IBuiltinCommand> builtins, ExecutableDirectories executableDirectories) : IBuiltinCommand
{
    public string Name { get; } = "type";
    public int Execute(string[] args)
    {
        string output = args.Length == 0 ? "type: not enough arguments" :
                builtins.ContainsKey(args[0]) ?
                    $"{args[0]} is a shell builtin" :
                    executableDirectories.GetProgramPath(args[0]) != null ?
                        $"{args[0]} is an external command" : $"{args[0]}: not found";
        Console.WriteLine(output);
        return 0;
    }
}