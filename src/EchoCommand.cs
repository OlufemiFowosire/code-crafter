internal class EchoCommand : IBuiltinCommand
{
    public string Name { get; } = "echo";
    public int Execute(string[] args)
    {
        string output = string.Join(" ", args.Skip(1));
        Console.WriteLine(output);
        return 0;
    }
}