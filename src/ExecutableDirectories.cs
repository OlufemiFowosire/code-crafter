internal class ExecutableDirectories
{
    // We can cache the directories so we don't re-read the environment every time
    private readonly string[] _paths;

    public ExecutableDirectories()
    {
        string pathVariable = Environment.GetEnvironmentVariable("PATH") ?? string.Empty;
        _paths = pathVariable.Split(Path.PathSeparator);
    }

    public string? GetProgramPath(string programName)
    {
        // Use the LINQ chain we discussed earlier
        return _paths
            .FirstOrDefault(dir => File.Exists(Path.Combine(dir, $"{programName}.exe")));
    }
}