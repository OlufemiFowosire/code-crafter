internal class ExitCommand : IBuiltinCommand
{
    public string Name { get; } = "exit";
    public void Execute(string[] args)
    {
        int exit = args.Length > 1 && int.TryParse(args[0], out int exitCode) ? exitCode : 0;
        Environment.Exit(exit);
        return;
    }
}