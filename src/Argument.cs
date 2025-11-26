using System.Text.RegularExpressions;

internal class Argument
{
    public static string[] Parse(string input)
    {
        // Regex Breakdown:
        // 1. [^\s"']+    -> Matches unquoted text (stops at space or quotes)
        // 2. "([^"]*)"   -> Matches double quotes, captures content in Group 1
        // 3. '([^']*)'   -> Matches single quotes, captures content in Group 2

        var matches = Regex.Matches(input, @"[^\s""']+|""([^""]*)""|'([^']*)'");

        string[] args = matches.Cast<Match>()
                            .Select(m =>
                            {
                                // If it matched the double-quote pattern (Group 1 has success/value)
                                if (m.Groups[1].Success) return m.Groups[1].Value;

                                // If it matched the single-quote pattern (Group 2 has success/value)
                                if (m.Groups[2].Success) return m.Groups[2].Value;

                                // Otherwise, it's a regular unquoted word
                                return m.Value;
                            })
                            .ToArray();
        return args;
    }
}