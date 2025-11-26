using System.Text;
using System.Text.RegularExpressions;

internal class Argument
{
    /* public static string[] Parse(string input)
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
    } */
    public static string[] Parse(string commandLine)
    {
        var args = new List<string>();
        var currentArg = new StringBuilder();
        bool inSingleQuotes = false;
        bool inDoubleQuotes = false;

        // This flag tracks if we are currently building an argument. 
        // It is necessary to distinguish between an empty string "" and "nothing".
        // e.g. echo " " -> 1 arg. echo -> 0 args.
        bool parsingArg = false;

        for (int i = 0; i < commandLine.Length; i++)
        {
            char c = commandLine[i];

            if (inSingleQuotes)
            {
                if (c == '\'')
                {
                    inSingleQuotes = false;
                }
                else
                {
                    currentArg.Append(c);
                }
            }
            else if (inDoubleQuotes)
            {
                if (c == '"')
                {
                    inDoubleQuotes = false;
                }
                // (Optional) Handle backslash logic here for later steps
                // else if (c == '\\' && i + 1 < commandLine.Length && ...)
                else
                {
                    currentArg.Append(c);
                }
            }
            else
            {
                // UNQUOTED STATE
                if (char.IsWhiteSpace(c))
                {
                    if (parsingArg)
                    {
                        args.Add(currentArg.ToString());
                        currentArg.Clear();
                        parsingArg = false;
                    }
                }
                else if (c == '\'')
                {
                    inSingleQuotes = true;
                    parsingArg = true; // We found a quote, so we are definitely parsing an arg
                }
                else if (c == '"')
                {
                    inDoubleQuotes = true;
                    parsingArg = true;
                }
                else
                {
                    currentArg.Append(c);
                    parsingArg = true;
                }
            }
        }

        // If we have leftover data in the buffer after the loop ends, add it.
        if (parsingArg)
        {
            args.Add(currentArg.ToString());
        }

        return args.ToArray();
    }
}