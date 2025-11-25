internal class PwdCommand : IBuiltinCommand
{
    public string Name { get; } = "echo";
    public int Execute(string[] args)
    {
        if (args.Length > 0)
        {
            Console.WriteLine("pwd: too many arguments");
            return 1;
        }
        Directory.PrintCurrentDirectory();
        return 0;
    }
}