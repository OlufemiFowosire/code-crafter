internal class CdCommand : IBuiltinCommand
{
    public string Name { get; } = "echo";
    public int Execute(string[] args)
    {
        string directory = args.Length > 0 ? Directory.GetAbsoluteTargetDirectory(args) : Directory.GetHomeDirectory();
        Console.WriteLine($"{directory}");
        return 0;
    }
}