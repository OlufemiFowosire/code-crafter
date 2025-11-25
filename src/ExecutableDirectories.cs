internal class ExecutableDirectories
{
    // We can cache the directories so we don't re-read the environment every time
    private readonly IEnumerable<string> paths;

    public ExecutableDirectories()
    {
        string pathVariable = Environment.GetEnvironmentVariable("PATH") ?? string.Empty;
        paths = pathVariable.Split(Path.PathSeparator);
    }

    public string? GetProgramPath(string programName)
    {
        // 3. The Search (using SelectMany to flatten the loops)
        /**return paths
            .SelectMany(dir => extensions, (dir, ext) => Path.Combine(dir, programName + ext))
            .OrderByDescending(fullPath => fullPath) // 4. Lexicographical Order
            .FirstOrDefault(fullPath => File.Exists(fullPath));**/
        return paths.Select(dir => Path.Combine(dir, programName))
                    .FirstOrDefault(fullPath => File.Exists(fullPath) && IsExecutable(fullPath));
    }

    private bool IsExecutable(string filePath)
    {
        // ---------------------------------------------------------
        // STRATEGY 1: Try to read Unix Permissions (Linux/macOS)
        // ---------------------------------------------------------
        try
        {
            // This is the Native .NET 7 replacement for Mono.Unix
            UnixFileMode mode = File.GetUnixFileMode(filePath);

            // Check if ANY execute bit is set (User, Group, or Other)
            return (mode & (UnixFileMode.UserExecute |
                            UnixFileMode.GroupExecute |
                            UnixFileMode.OtherExecute)) != 0;
        }
        catch (PlatformNotSupportedException)
        {
            // ---------------------------------------------------------
            // STRATEGY 2: We are on Windows (Fallback)
            // ---------------------------------------------------------

            // On Windows, "Executable" is defined by the file extension.
            // Instead of hardcoding ".exe", we read the system's definition.

            var pathExt = Environment.GetEnvironmentVariable("PATHEXT") ?? ".COM;.EXE;.BAT;.CMD";
            var supportedExtensions = pathExt.Split(Path.PathSeparator);

            string fileExtension = Path.GetExtension(filePath);

            // Check if the file's extension is in the list (Case-Insensitive)
            foreach (var ext in supportedExtensions)
            {
                if (string.Equals(fileExtension, ext, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }
        catch (Exception)
        {
            // File doesn't exist or access denied
            return false;
        }
    }
}