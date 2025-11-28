public class UpArrowHandler : IKeyHandler
{
    public bool Handle(EditorContext context, ConsoleKeyInfo keyInfo)
    {
        // If we haven't started navigating history, snap to the end
        if (context.HistoryIndex == -1)
        {
            context.HistoryIndex = HistoryService.Instance.Count;
        }

        // Move back in history
        if (context.HistoryIndex > 0)
        {
            context.HistoryIndex--;
            string historyItem = HistoryService.Instance.GetEntry(context.HistoryIndex);
            context.ReplaceBuffer(historyItem);
        }

        return true; // Continue the loop
    }
}