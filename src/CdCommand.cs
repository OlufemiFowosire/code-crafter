internal class CdCommand : IBuiltinCommand
{
    public string Name { get; } = "cd";
    public int Execute(string[] args)
    {
        Environment.CurrentDirectory = Directory.GetTargetDirectory(args);
        return 0;
    }
}