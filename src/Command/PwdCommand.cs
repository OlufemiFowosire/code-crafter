internal class PwdCommand : IBuiltinCommand
{
    public string Name { get; } = "pwd";
    public async Task ExecuteAsync(string[] args, Stream? stdin, Stream stdout, Stream stderr)
    {
        using var writer = new StreamWriter(stdout, leaveOpen: true);
        // Note: Pwd writes to stdout
        await writer.WriteLineAsync(Directory.GetCurrentDirectory());
    }
}