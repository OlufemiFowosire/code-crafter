internal class HistoryCommand : IBuiltinCommand
{
    public string Name => "history";

    public async Task ExecuteAsync(string[] args, Stream? stdin, Stream? stdout, Stream? stderr)
    {
        TextWriter writer;
        bool isConsole = stdout == null;

        if (isConsole) writer = Console.Out;
        else writer = new StreamWriter(stdout!, leaveOpen: true);

        try
        {
            int limit = -1;
            if (args.Length > 0 && !int.TryParse(args[0], out limit))
            {
                await writer.WriteLineAsync($"history: invalid number argument: {args[0]}");
                await writer.FlushAsync();
                return;
            }

            foreach (var (index, cmd) in HistoryService.Instance.GetEntries(limit))
            {
                await writer.WriteLineAsync($"{index,4}  {cmd}");
            }
            await writer.FlushAsync();
        }
        finally
        {
            if (!isConsole) writer.Dispose();
        }
    }
}