internal class PwdCommand : IBuiltinCommand
{
    public string Name { get; } = "pwd";
    public void Execute(string[] args)
    {
        if (args.Length > 0)
        {
            Console.WriteLine("pwd: too many arguments");
        }
        Directory.PrintCurrentDirectory();
    }
}