using System.Runtime.InteropServices;

internal static class ExecutableDirectories
{
    private static readonly string[] _paths = (Environment.GetEnvironmentVariable("PATH") ?? string.Empty)
        .Split(Path.PathSeparator);

    // Prepare Windows extensions once. On Linux/Mac, this array is empty.
    private static readonly string[] _windowsExtensions = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
        ? (Environment.GetEnvironmentVariable("PATHEXT") ?? ".COM;.EXE;.BAT;.CMD").Split(Path.PathSeparator)
        : Array.Empty<string>();

    public static string? GetProgramPath(string programName)
    {
        foreach (var dir in _paths)
        {
            // 1. Check exact match (e.g. "git" on Linux, or "code.exe" on Windows)
            var exactPath = Path.Combine(dir, programName);
            if (CheckFile(exactPath)) return exactPath;

            // 2. Windows-only: If exact match fails, try appending extensions
            // (e.g. user typed "code", we check "code.exe", "code.cmd")
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                foreach (var ext in _windowsExtensions)
                {
                    var pathWithExt = exactPath + ext;
                    if (CheckFile(pathWithExt)) return pathWithExt;
                }
            }
        }

        return null;
    }

    private static bool CheckFile(string fullPath)
    {
        // Fast fail if file doesn't exist
        if (!File.Exists(fullPath)) return false;

        return IsExecutable(fullPath);
    }

    private static bool IsExecutable(string filePath)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            // On Windows, if File.Exists passed, and extension matched PATHEXT logic 
            // (or was provided explicitly), it's generally considered runnable.
            return true;
        }

        // Linux/macOS permissions check
        try
        {
            var mode = File.GetUnixFileMode(filePath);
            return (mode & (UnixFileMode.UserExecute |
                            UnixFileMode.GroupExecute |
                            UnixFileMode.OtherExecute)) != 0;
        }
        catch
        {
            return false;
        }
    }
}