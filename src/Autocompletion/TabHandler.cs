public class TabHandler : IKeyHandler
{
    public bool Handle(EditorContext context, ConsoleKeyInfo keyInfo)
    {
        context.TabCount++; // Increment count (DO NOT RESET)

        string currentInput = context.Buffer.ToString();
        var matches = context.Completer.GetMatches(currentInput);

        // 1. No Matches
        if (matches.Count == 0)
        {
            context.RingBell();
            return true;
        }

        // 2. Exact/Single Match
        if (matches.Count == 1)
        {
            string match = matches[0];
            // If we have "gi" and match is "git", append "t "
            string remainder = match.Substring(currentInput.Length) + " ";
            context.Write(remainder);

            // We finished a completion, arguably we could reset tab count here,
            // or keep it to allow subsequent argument completion.
            context.TabCount = 0;
            return true;
        }

        // 3. Multiple Matches
        string lcp = context.Completer.FindLongestCommonPrefix(matches);

        // A. We can auto-complete partially (Extend the prefix)
        if (lcp.Length > currentInput.Length)
        {
            string remainder = lcp.Substring(currentInput.Length);
            context.Write(remainder);
            // Don't show list yet, wait for next tab
        }
        // B. We are stuck at the common prefix
        else
        {
            if (context.TabCount == 1)
            {
                context.RingBell();
            }
            else
            {
                // Double Tab: Show options
                context.NewLine();
                Console.WriteLine(string.Join("  ", matches));

                // Restore prompt line
                Console.Write(context.Prompt);
                Console.Write(context.Buffer.ToString());
            }
        }

        return true;
    }
}