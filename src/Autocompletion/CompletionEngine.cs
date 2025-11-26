public class CompletionEngine(IEnumerable<ICompletionSource> sources)
{
    private readonly List<ICompletionSource> _sources = sources.ToList();

    public List<string> GetMatches(string prefix)
    {
        // Aggregate all options from all sources and filter by prefix
        return _sources
            .SelectMany(s => s.GetOptions())
            .Where(name => name.StartsWith(prefix)) // Case-sensitive for CodeCrafters usually
            .OrderBy(name => name) // Alphabetical order is required for the "List" feature
            .ToList();
    }

    public string FindLongestCommonPrefix(List<string> matches)
    {
        if (matches.Count == 0) return string.Empty;
        if (matches.Count == 1) return matches[0];

        // Sort acts as a shortcut; we only need to compare the first and last item
        // to find the common prefix for the whole group.
        string first = matches[0];
        string last = matches[matches.Count - 1];

        int length = 0;
        while (length < first.Length && length < last.Length && first[length] == last[length])
        {
            length++;
        }

        return first.Substring(0, length);
    }
}