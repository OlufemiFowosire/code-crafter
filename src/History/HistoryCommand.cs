internal class HistoryCommand : IBuiltinCommand
{
    public string Name => "history";

    public async Task ExecuteAsync(string[] args, Stream? stdin, Stream? stdout, Stream? stderr)
    {
        // Default output stream
        Stream target = stdout ?? Console.OpenStandardOutput();
        using var writer = new StreamWriter(target, leaveOpen: true);

        int limit = -1;

        // Check for 'history <n>' syntax
        if (args.Length > 0)
        {
            if (!int.TryParse(args[0], out limit))
            {
                await writer.WriteLineAsync($"history: invalid number argument: {args[0]}");
                await writer.FlushAsync();
                return;
            }
        }

        foreach (var (index, cmd) in HistoryService.Instance.GetEntries(limit))
        {
            // Format: "  1  ls -la"
            await writer.WriteLineAsync($"{index,4}  {cmd}");
        }

        await writer.FlushAsync();
    }
}