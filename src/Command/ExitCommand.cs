using System;
using System.IO;
using System.Threading.Tasks;

// Grouping simple builtins here for conciseness

internal class ExitCommand : IBuiltinCommand
{
    public string Name => "exit";
    public async Task ExecuteAsync(string[] args, Stream? stdin, Stream? stdout, Stream? stderr)
    {
        // [NEW] Save history before exiting
        string? histFile = Environment.GetEnvironmentVariable("HISTFILE");
        if (!string.IsNullOrEmpty(histFile))
        {
            try
            {
                // Standard shell behavior is usually to Append on exit
                // (preserves history from other concurrent sessions)
                await HistoryService.Instance.AppendAsync(histFile);

                // If you prefer Overwrite on exit (erase other sessions), swap with:
                // await HistoryService.Instance.WriteAsync(histFile);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error saving history: {ex.Message}");
            }
        }

        int exit = args.Length > 0 && int.TryParse(args[0], out int exitCode) ? exitCode : 0;
        Environment.Exit(exit);
    }
}