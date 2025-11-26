using System;
using System.IO;
using System.Threading.Tasks;

internal class CdCommand : IBuiltinCommand
{
    public string Name { get; } = "cd";

    public Task ExecuteAsync(string[] args, Stream stdin, Stream stdout, Stream stderr)
    {
        try
        {
            Directory.ChangeToTargetDirectory(args);
        }
        catch (DirectoryNotFoundException)
        {
            // Rethrow with the specific message format required by the original solution
            // Note: We assume args[0] exists if we have a specific path error, 
            // mirroring the original implementation.
            string path = args.Length > 0 ? args[0] : "";
            throw new DirectoryNotFoundException($"cd: {path}: No such file or directory");
        }

        return Task.CompletedTask;
    }
}