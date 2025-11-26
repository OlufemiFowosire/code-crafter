public class BackspaceHandler : IKeyHandler
{
    public bool Handle(EditorContext context, ConsoleKeyInfo keyInfo)
    {
        context.TabCount = 0; // Reset tab tracking
        context.Backspace();
        return true; // Continue
    }
}