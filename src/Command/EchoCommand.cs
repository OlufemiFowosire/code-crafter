internal class EchoCommand : IBuiltinCommand
{
    public string Name => "echo";

    public async Task ExecuteAsync(string[] args, Stream? stdin, Stream? stdout, Stream? stderr)
    {

        string output = string.Join(" ", args);
        if (stdout == null)
        {
            // Direct to Console (No Stream buffering issues)
            await Console.Out.WriteLineAsync(output);
            await Console.Out.FlushAsync();
        }
        else
        {
            // Use StreamWriter only for pipes/files
            using var writer = new StreamWriter(stdout, leaveOpen: true);
            await writer.WriteLineAsync(output);
            await writer.FlushAsync();
        }
    }
}