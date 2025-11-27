internal class PwdCommand : IBuiltinCommand
{
    public string Name { get; } = "pwd";
    public async Task ExecuteAsync(string[] args, Stream? stdin, Stream? stdout, Stream? stderr)
    {
        // Fallback to Console if stdout is null (Inherit)
        Stream target = stdout ?? Console.OpenStandardOutput();
        using var writer = new StreamWriter(target, leaveOpen: true);
        // Note: Pwd writes to stdout
        await writer.WriteLineAsync(Directory.GetCurrentDirectory());
    }
}