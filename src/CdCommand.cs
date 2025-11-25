internal class CdCommand : IBuiltinCommand
{
    public string Name { get; } = "cd";
    public int Execute(string[] args)
    {
        if (!Directory.ChangeToTargetDirectory(args))
        {
            return 1;
        }
        return 0;
    }
}