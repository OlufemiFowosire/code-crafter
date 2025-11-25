internal class TypeCommand(Dictionary<string, IBuiltinCommand> builtins, ExecutableDirectories executableDirectories) : IBuiltinCommand
{
    public string Name { get; } = "type";
    public int Execute(string[] args)
    {
        string? path = executableDirectories.GetProgramPath(args[0]);
        string output = args.Length == 0 ? "type: not enough arguments" :
                builtins.ContainsKey(args[0]) ?
                    $"{args[0]} is a shell builtin" :
                    path != null ?
                        $"{args[0]} is {path}" : $"{args[0]}: not found";
        Console.WriteLine(output);
        return 0;
    }
}