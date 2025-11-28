internal class HistoryCommand : IBuiltinCommand
{
    public string Name => "history";

    public async Task ExecuteAsync(string[] args, Stream? stdin, Stream? stdout, Stream? stderr)
    {
        // Default output stream
        Stream target = stdout ?? Console.OpenStandardOutput();
        using var writer = new StreamWriter(target, leaveOpen: true) { AutoFlush = true };

        // 1. Handle File Operations (-r, -w, -a)
        if (args.Length > 0 && args[0].StartsWith("-"))
        {
            string flag = args[0];

            // Get path from arg or Environment Variable
            string? path = args.Length > 1
                ? args[1]
                : Environment.GetEnvironmentVariable("HISTFILE");

            if (string.IsNullOrEmpty(path))
            {
                await Console.Error.WriteLineAsync("history: no history file path configured (HISTFILE is empty)");
                return;
            }

            try
            {
                switch (flag)
                {
                    case "-r": // Read
                        await HistoryService.Instance.LoadAsync(path);
                        break;
                    case "-w": // Write (Overwrite)
                        await HistoryService.Instance.WriteAsync(path);
                        break;
                    case "-a": // Append
                        await HistoryService.Instance.AppendAsync(path);
                        break;
                    default:
                        await Console.Error.WriteLineAsync($"history: invalid option {flag}");
                        break;
                }
            }
            catch (Exception ex)
            {
                await Console.Error.WriteLineAsync($"history: {ex.Message}");
            }
            return;
        }

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