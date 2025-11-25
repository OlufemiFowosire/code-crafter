internal class ExecutableDirectories
{
    // We can cache the directories so we don't re-read the environment every time
    private readonly IEnumerable<string> paths;
    private readonly IEnumerable<string> extensions;

    public ExecutableDirectories()
    {
        string pathVariable = Environment.GetEnvironmentVariable("PATH") ?? string.Empty;
        paths = pathVariable.Split(Path.PathSeparator);
        string pathExt = Environment.GetEnvironmentVariable("PATHEXT") ?? string.Empty;
        extensions = pathExt.Split(Path.PathSeparator).Prepend("");
    }

    public string? GetProgramPath(string programName)
    {
        // 3. The Search (using SelectMany to flatten the loops)
        return paths
            .FirstOrDefault(dir =>
                extensions
                    .Select(ext => Path.Combine(dir, programName + ext))
                    .Any(fullPath => File.Exists(fullPath)));
    }
}