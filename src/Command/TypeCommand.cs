public class TypeCommand : IBuiltinCommand
{
    public string Name => "type";

    public void Execute(string[] args)
    {
        if (args.Length == 0) return;

        string target = args[0];

        // 1. Query the Registry (Zero allocation lookup)
        if (CommandRegistry.IsBuiltin(target))
        {
            Console.WriteLine($"{target} is a shell builtin");
            return;
        }

        // 2. Check File System
        string? path = ExecutableDirectories.GetProgramPath(target);
        if (path is not null)
        {
            Console.WriteLine($"{target} is {path}");
            return;
        }

        Console.WriteLine($"{target}: not found");
    }
}