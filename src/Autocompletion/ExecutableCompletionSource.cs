public class ExecutableCompletionSource : ICompletionSource
{
    public IEnumerable<string> GetOptions()
    {
        var paths = Environment.GetEnvironmentVariable("PATH")?.Split(Path.PathSeparator);
        if (paths == null) yield break;

        var returned = new HashSet<string>();

        foreach (var dir in paths)
        {
            if (!Directory.IsDirectory(dir)) continue;

            // Enumerate executables in the directory
            // We use a try-catch to skip directories we don't have permission to read
            IEnumerable<string> files = Enumerable.Empty<string>();
            try
            {
                files = Directory.ListFiles(dir);
            }
            catch (Exception) { continue; }

            foreach (var file in files)
            {
                string name = Path.GetFileName(file);
                // On Windows, you might want to strip extensions, but for now returned raw
                if (returned.Add(name)) // Prevent duplicates if same tool is in multiple PATHs
                {
                    yield return name;
                }
            }
        }
    }
}