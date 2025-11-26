internal class CdCommand : IBuiltinCommand
{
    public string Name { get; } = "cd";
    public void Execute(string[] args)
    {
        try
        {
            Directory.ChangeToTargetDirectory(args);
        }
        catch (DirectoryNotFoundException)
        {
            throw;
        }
    }
}