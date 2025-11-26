internal class CommandRegistry
{
    // The "Flyweight" storage. 
    // We map the Name string directly to the Command Instance.
    private static readonly Dictionary<string, IBuiltinCommand> _builtins = new();

    static CommandRegistry()
    {
        // Register all builtins here once.
        Register(new ExitCommand());
        Register(new EchoCommand());
        Register(new PwdCommand());
        Register(new CdCommand());
        Register(new TypeCommand());
    }

    private static void Register(IBuiltinCommand command)
    {
        _builtins[command.Name] = command;
    }

    public static bool IsBuiltin(string name) => _builtins.ContainsKey(name);

    public static bool TryGetBuiltin(string name, out ICommand? command)
    {
        if (_builtins.TryGetValue(name, out var builtin))
        {
            command = builtin;
            return true;
        }
        command = null;
        return false;
    }
    public static IEnumerable<string> GetAllBuiltinNames() => _builtins.Keys;
}