internal class EchoCommand : IBuiltinCommand
{
    public string Name { get; } = "echo";

    public async Task ExecuteAsync(string[] args, Stream stdin, Stream stdout, Stream stderr)
    {
        // We use StreamWriter to write text to the binary stream
        using var writer = new StreamWriter(stdout, leaveOpen: true);

        string output = string.Join(" ", args);
        await writer.WriteLineAsync(output);

        // Flush is important in pipelines to push data to the next consumer
        await writer.FlushAsync();
    }
}