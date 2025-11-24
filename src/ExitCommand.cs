internal class ExitCommand : IBuiltinCommand
{
    public string Name { get; } = "exit";
    public int Execute(string[] args)
    {
        return args.Length > 1 && int.TryParse(args[1], out int exitCode) ? exitCode : 0;
    }
}