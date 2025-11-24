public class ExecutableDirectories
{
    public string? GetProgramPath(string programName)
    {
        // Implementation to find the program in executable directories
        return Environment.GetEnvironmentVariable("PATH")?.Split(Path.PathSeparator)
            .Select(dir => Path.Combine(dir, programName))
            .FirstOrDefault(path => File.Exists(path));
    }
}