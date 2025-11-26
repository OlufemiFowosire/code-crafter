public class EnterHandler : IKeyHandler
{
    public bool Handle(EditorContext context, ConsoleKeyInfo keyInfo)
    {
        context.NewLine();
        return false; // Stop the loop
    }
}