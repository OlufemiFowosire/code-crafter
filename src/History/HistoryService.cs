public class HistoryService
{
    // Singleton Instance
    public static readonly HistoryService Instance = new();
    private readonly List<string> _history = new();

    // Tracks how many items have been synchronized to disk/loaded.
    // This prevents 'history -a' from duplicating old entries.
    private int _persistedCount = 0;

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

    // [NEW] Read history from file (append to memory)
    // Corresponds to: history -r
    public async Task LoadAsync(string path)
    {
        if (!File.Exists(path)) return;

        var lines = await File.ReadAllLinesAsync(path);
        _history.AddRange(lines);

        // Update our pointer so we don't re-append these lines later
        _persistedCount = _history.Count;
    }

    // [NEW] Write in-memory history to file (overwrite)
    // Corresponds to: history -w
    public async Task WriteAsync(string path)
    {
        await File.WriteAllLinesAsync(path, _history);
        _persistedCount = _history.Count;
    }

    // [NEW] Append new in-memory commands to file
    // Corresponds to: history -a
    public async Task AppendAsync(string path)
    {
        // Only append lines that haven't been persisted yet
        if (_persistedCount < _history.Count)
        {
            var newLines = _history.Skip(_persistedCount).ToList();
            await File.AppendAllLinesAsync(path, newLines);
            _persistedCount = _history.Count;
        }
    }
}