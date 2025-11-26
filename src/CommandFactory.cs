internal class CommandFactory
{
    public static ICommand Create(string commandName)
    {
        // 1. Try to return a cached Flyweight instance (Fast, 0 memory allocation)
        if (CommandRegistry.TryGetBuiltin(commandName, out var builtin))
        {
            return builtin!;
        }

        // 2. If not builtin, it must be external. 
        // We MUST create a new instance here because ExternalCommand holds state (the name).
        return new ExternalCommand(commandName);
    }
}