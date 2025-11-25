internal class CdCommand : IBuiltinCommand
{
    public string Name { get; } = "echo";
    public int Execute(string[] args)
    {
        Environment.CurrentDirectory = args.Length > 0 ? args[0] : Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        string output = Environment.CurrentDirectory;
        Console.WriteLine(output);
        return 0;
    }
}