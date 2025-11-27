using System;
using System.IO;
using System.Threading.Tasks;

public class TypeCommand : IBuiltinCommand
{
    public string Name => "type";

    public async Task ExecuteAsync(string[] args, Stream? stdin, Stream? stdout, Stream? stderr)
    {
        // Fallback to Console if stdout is null (Inherit)
        Stream target = stdout ?? Console.OpenStandardOutput();
        using var writer = new StreamWriter(target, leaveOpen: true);

        if (args.Length == 0) return;

        string targetCommand = args[0];

        // 1. Check Builtin Registry
        if (CommandRegistry.IsBuiltin(targetCommand))
        {
            await writer.WriteLineAsync($"{targetCommand} is a shell builtin");
            return;
        }

        // 2. Check File System
        string? path = ExecutableDirectories.GetProgramPath(targetCommand);
        if (path is null)
        {
            await writer.WriteLineAsync($"{targetCommand}: not found");
            return;
        }

        await writer.WriteLineAsync($"{targetCommand} is {path}");
    }
}