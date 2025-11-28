public class HistoryService
{
    // Singleton Instance
    public static readonly HistoryService Instance = new();

    private readonly List<string> _history = new();

    private HistoryService() { }

    public void Add(string command)
    {
        // Optional: Don't add duplicates consecutively or empty commands
        if (string.IsNullOrWhiteSpace(command)) return;

        // Simple implementation: Always add
        _history.Add(command);
    }

    public int Count => _history.Count;

    public string GetEntry(int index)
    {
        if (index >= 0 && index < _history.Count)
        {
            return _history[index];
        }
        return string.Empty;
    }

    public IEnumerable<(int Index, string Command)> GetEntries(int limit = -1)
    {
        // Return indexed entries
        var entries = _history.Select((cmd, idx) => (Index: idx + 1, Command: cmd));

        if (limit > 0)
        {
            return entries.TakeLast(limit);
        }
        return entries;
    }
}