using System.Text;

public class EditorContext
{
    public StringBuilder Buffer { get; } = new();
    public int TabCount { get; set; } = 0;
    public string Prompt { get; }
    public CompletionEngine Completer { get; }

    public EditorContext(string prompt, CompletionEngine completer)
    {
        Prompt = prompt;
        Completer = completer;
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