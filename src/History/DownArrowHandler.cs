public class DownArrowHandler : IKeyHandler
{
    public bool Handle(EditorContext context, ConsoleKeyInfo keyInfo)
    {
        // If not navigating, do nothing
        if (context.HistoryIndex == -1) return true;

        int count = HistoryService.Instance.Count;

        // Move forward
        if (context.HistoryIndex < count)
        {
            context.HistoryIndex++;

            if (context.HistoryIndex == count)
            {
                // We are back at the "current" empty line
                context.ReplaceBuffer(string.Empty);
            }
            else
            {
                string historyItem = HistoryService.Instance.GetEntry(context.HistoryIndex);
                context.ReplaceBuffer(historyItem);
            }
        }

        return true;
    }
}