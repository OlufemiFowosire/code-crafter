internal class PwdCommand : IBuiltinCommand
{
    public string Name { get; } = "pwd";

    public async Task ExecuteAsync(string[] args, Stream? stdin, Stream? stdout, Stream? stderr)
    {
        string dir = Directory.GetCurrentDirectory();

        if (stdout == null)
        {
            await Console.Out.WriteLineAsync(dir);
            await Console.Out.FlushAsync();
        }
        else
        {
            using var writer = new StreamWriter(stdout, leaveOpen: true);
            await writer.WriteLineAsync(dir);
            await writer.FlushAsync();
        }
    }
}