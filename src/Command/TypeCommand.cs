public class TypeCommand : IBuiltinCommand
{
    public string Name => "type";

    public async Task ExecuteAsync(string[] args, Stream? stdin, Stream? stdout, Stream? stderr)
    {
        if (args.Length == 0) return;

        // Helper to get the correct writer
        TextWriter writer;
        bool isConsole = stdout == null;

        if (isConsole)
        {
            writer = Console.Out;
        }
        else
        {
            writer = new StreamWriter(stdout!, leaveOpen: true);
        }

        try
        {
            string targetCommand = args[0];

            if (CommandRegistry.IsBuiltin(targetCommand))
            {
                await writer.WriteLineAsync($"{targetCommand} is a shell builtin");
            }
            else
            {
                string? path = ExecutableDirectories.GetProgramPath(targetCommand);
                if (path is null)
                {
                    await writer.WriteLineAsync($"{targetCommand}: not found");
                }
                else
                {
                    await writer.WriteLineAsync($"{targetCommand} is {path}");
                }
            }

            await writer.FlushAsync();
        }
        finally
        {
            // Only dispose if we created the StreamWriter (Pipeline case)
            if (!isConsole)
            {
                writer.Dispose();
            }
        }
    }
}