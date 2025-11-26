using System;
using System.IO;
using System.Threading.Tasks;

// Grouping simple builtins here for conciseness

internal class ExitCommand : IBuiltinCommand
{
    public string Name { get; } = "exit";
    public Task ExecuteAsync(string[] args, Stream stdin, Stream stdout, Stream stderr)
    {
        int exit = args.Length > 0 && int.TryParse(args[0], out int exitCode) ? exitCode : 0;
        Environment.Exit(exit);
        return Task.CompletedTask;
    }
}