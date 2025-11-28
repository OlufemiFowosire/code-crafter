internal class EchoCommand : IBuiltinCommand
{
    public string Name => "echo";

    public async Task ExecuteAsync(string[] args, Stream? stdin, Stream? stdout, Stream? stderr)
    {
        // Fallback to Console if stdout is null (Inherit)
        Stream target = stdout ?? Console.OpenStandardOutput();
        // We use StreamWriter to write text to the binary stream
        using var writer = new StreamWriter(target, leaveOpen: true);

        string output = string.Join(" ", args);
        await writer.WriteLineAsync(output);

        // Flush is important in pipelines to push data to the next consumer
        await writer.FlushAsync();
    }
}