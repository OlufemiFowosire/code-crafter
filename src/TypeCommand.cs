internal class TypeCommand(Dictionary<string, int> builtins) : IBuiltinCommand
{
    public string Name { get; } = "type";
    public int Execute(string[] args)
    {
        string? path = ExecutableDirectories.GetProgramPath(args[0]);
        try
        {
            if (args.Length == 0)
            {
                throw new ArgumentException("type: not enough arguments");
            }
            if (builtins.ContainsKey(args[0]))
            {
                Console.WriteLine($"{args[0]} is a shell builtin");
                return 0;
            }
            if (path != null)
            {
                Console.WriteLine($"{args[0]} is an external command");
                //Console.WriteLine($"{args[0]} is {path}");
                return 0;
            }
            throw new ArgumentException($"{args[0]}: not found");
        }
        catch (ArgumentException)
        {
            throw;
        }
    }

    public Dictionary<string, int> Builtins
    {
        get { return builtins; }
    }


}