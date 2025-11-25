internal class CdCommand : IBuiltinCommand
{
    public string Name { get; } = "echo";
    public int Execute(string[] args)
    {
        if (args.Length == 1 && Directory.IsValidTargetDirectory(args))
        {
            Environment.CurrentDirectory = Directory.GetAbsoluteTargetDirectory(args);
            return 0;
        }
        else if (args.Length == 1 && !Directory.IsValidTargetDirectory(args))
        {
            Console.WriteLine($"cd: {args[0]}: No such file or directory");
            return 1;
        }
        else if (args.Length > 1)
        {
            Console.WriteLine("cd: too many arguments");
            return 2;
        }
        else if (args.Length == 0)
        {
            Environment.CurrentDirectory = Directory.GetHomeDirectory();
            return 0;
        }
        return 0;
    }
}