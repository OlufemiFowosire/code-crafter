internal class CdCommand : IBuiltinCommand
{
    public string Name { get; } = "echo";
    public int Execute(string[] args)
    {
        Environment.CurrentDirectory = args.Length > 0 ? Directory.GetAbsoluteTargetDirectory(args) : Directory.GetHomeDirectory();
        return 0;
    }
}