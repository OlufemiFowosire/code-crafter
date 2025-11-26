internal class TypeCommand(Dictionary<string, int> builtins) : IBuiltinCommand
{
    public string Name { get; } = "type";
    public void Execute(string[] args)
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
                return;
            }
            if (path != null)
            {
                Console.WriteLine($"{args[0]} is {path}");
                return;
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