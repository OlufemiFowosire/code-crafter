using System;
using System.IO;
using System.Threading.Tasks;

public class TypeCommand : IBuiltinCommand
{
    public string Name => "type";

    public async Task ExecuteAsync(string[] args, Stream stdin, Stream stdout, Stream stderr)
    {
        using var writer = new StreamWriter(stdout, leaveOpen: true);

        if (args.Length == 0) return;

        string target = args[0];

        // 1. Check Builtin Registry
        if (CommandRegistry.IsBuiltin(target))
        {
            await writer.WriteLineAsync($"{target} is a shell builtin");
            return;
        }

        // 2. Check File System
        string? path = ExecutableDirectories.GetProgramPath(target);
        if (path is null)
        {
            await writer.WriteLineAsync($"{target}: not found");
            return;
        }

        await writer.WriteLineAsync($"{target} is {path}");
    }
}