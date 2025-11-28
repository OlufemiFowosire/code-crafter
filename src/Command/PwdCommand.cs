internal class PwdCommand : IBuiltinCommand
{
    public string Name => "pwd";
    public async Task ExecuteAsync(string[] args, Stream? stdin, Stream? stdout, Stream? stderr)
    {
        // Fallback to Console if stdout is null (Inherit)
        Stream target = stdout ?? Console.OpenStandardOutput();
        using var writer = new StreamWriter(target, leaveOpen: true) { AutoFlush = true };
        // Note: Pwd writes to stdout
        await writer.WriteLineAsync(Directory.GetCurrentDirectory());
        // Ensure all data is sent to the stream before disposing
        // (AutoFlush=true handles this, but explicit FlushAsync is also good practice)
        await writer.FlushAsync();
    }
}