public class LineEditor
{
    private readonly Dictionary<ConsoleKey, IKeyHandler> _keyMap;
    private readonly IKeyHandler _defaultHandler;
    private readonly CompletionEngine _completer;

    public LineEditor(CompletionEngine completer)
    {
        _completer = completer;

        // 1. Register Special Keys
        _keyMap = new Dictionary<ConsoleKey, IKeyHandler>
        {
            { ConsoleKey.Enter,     new EnterHandler() },
            { ConsoleKey.Tab,       new TabHandler() },
            { ConsoleKey.Backspace, new BackspaceHandler() },
            // Easy to extend: { ConsoleKey.LeftArrow, new LeftArrowHandler() }
            // [NEW] Register History Handlers
            { ConsoleKey.UpArrow,   new UpArrowHandler() },
            { ConsoleKey.DownArrow, new DownArrowHandler() }
        };

        // 2. Register Default (Typing) Handler
        _defaultHandler = new TypingHandler();
    }

    public string ReadLine(string prompt)
    {
        Console.Write(prompt);
        var context = new EditorContext(prompt, _completer);

        while (true)
        {
            ConsoleKeyInfo keyInfo = Console.ReadKey(intercept: true);

            // Strategy Lookup
            if (!_keyMap.TryGetValue(keyInfo.Key, out var handler))
            {
                handler = _defaultHandler;
            }

            // Execute
            bool shouldContinue = handler.Handle(context, keyInfo);

            if (!shouldContinue)
            {
                return context.Buffer.ToString();
            }
        }
    }
}