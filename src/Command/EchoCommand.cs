internal class EchoCommand : IBuiltinCommand
{
    public string Name { get; } = "echo";
    public void Execute(string[] args)
    {
        string output = string.Join(" ", args);
        Console.WriteLine(output);
    }
}