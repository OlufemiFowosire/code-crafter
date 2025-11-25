internal class PwdCommand : IBuiltinCommand
{
    public string Name { get; } = "pwd";
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