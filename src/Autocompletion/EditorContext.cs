using System.Text;

public class EditorContext
{
    public StringBuilder Buffer { get; } = new();
    // [NEW] Track current position in history. -1 means not navigating.
    public int HistoryIndex { get; set; } = -1;
    public int TabCount { get; set; } = 0;
    public string Prompt { get; }
    public CompletionEngine Completer { get; }

    public EditorContext(string prompt, CompletionEngine completer)
    {
        Prompt = prompt;
        Completer = completer;
    }

    // [NEW] Visual Helper to replace the entire line
    public void ReplaceBuffer(string newText)
    {
        // 1. Visually erase current buffer
        while (Buffer.Length > 0)
        {
            Backspace();
        }

        // 2. Append new text
        Write(newText);
    }

    // --- Output Helpers ---
    public void Write(string text)
    {
        Console.Write(text);
        Buffer.Append(text);
    }

    public void Write(char c)
    {
        Console.Write(c);
        Buffer.Append(c);
    }

    public void Backspace()
    {
        if (Buffer.Length > 0)
        {
            Buffer.Length--;
            // Visual backspace hack: Move cursor back, overwrite with space, move back again
            Console.Write("\b \b");
        }
    }

    public void NewLine() => Console.WriteLine();

    public void RingBell() => Console.Write("\a");
}