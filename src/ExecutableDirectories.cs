internal class ExecutableDirectories
{
    // We can cache the directories so we don't re-read the environment every time
    private readonly IEnumerable<string> paths;
    private readonly IEnumerable<string> extensions;

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
                    .FirstOrDefault(fullPath => File.Exists(fullPath));
    }
}