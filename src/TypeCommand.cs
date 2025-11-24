internal class TypeCommand(Dictionary<string, IBuiltinCommand> builtins, ExecutableDirectories executableDirectories) : IBuiltinCommand
{
    public string Name { get; } = "type";
    public int Execute(string[] args)
    {
        string output = "";
        foreach (string programName in args.Skip(1))
        {
            output = builtins.ContainsKey(programName) ?
                $"{programName} is a shell builtin" :
                executableDirectories.GetProgramPath(programName) != null ?
                    $"{programName} is an external command" :
                    programName == "" ? "type: not enough arguments" :
                    $"{programName}: not found";
        }
        Console.WriteLine(output);
        return 0;
    }
}