internal class CdCommand : IBuiltinCommand
{
    public string Name { get; } = "cd";
    public int Execute(string[] args)
    {
        try
        {
            Directory.ChangeToTargetDirectory(args);
        }
        catch (DirectoryNotFoundException)
        {
            throw;
        }
        return 0;
    }
}