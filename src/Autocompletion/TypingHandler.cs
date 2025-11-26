public class TypingHandler : IKeyHandler
{
    public bool Handle(EditorContext context, ConsoleKeyInfo keyInfo)
    {
        // Ignore control characters (like Ctrl+C)
        if (char.IsControl(keyInfo.KeyChar)) return true;

        context.TabCount = 0; // Reset tab tracking
        context.Write(keyInfo.KeyChar);
        return true;
    }
}