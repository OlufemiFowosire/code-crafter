// --- 3. Flyweight Actions (Reusable Instances) ---
public static class Actions
{
    // A. Simple Actions
    public static readonly ICharAction Append = new AppendAction();
    public static readonly ICharAction Commit = new CommitAction();

    // B. Complex Actions (Pre-configured)
    // Mode: Escape everything (Unquoted)
    public static readonly ICharAction EscapeAll = new EscapeAction(alwaysEscape: true);

    // Mode: Escape only " and \ (Double Quoted)
    public static readonly ICharAction EscapeComplex = new EscapeAction(alwaysEscape: false, '\\', '"');

    // Internal Implementations
    private class AppendAction : ICharAction
    {
        public void Execute(ParserContext ctx, char c) => ctx.Append(c);
    }

    private class CommitAction : ICharAction
    {
        public void Execute(ParserContext ctx, char c) => ctx.CommitArg();
    }

    private class EscapeAction : ICharAction
    {
        private readonly bool _alwaysEscape;
        private readonly HashSet<char>? _targets;

        public EscapeAction(bool alwaysEscape, params char[] targets)
        {
            _alwaysEscape = alwaysEscape;
            if (targets.Length > 0) _targets = new HashSet<char>(targets);
        }

        public void Execute(ParserContext ctx, char c)
        {
            char next = ctx.Peek();
            if (_alwaysEscape || (_targets != null && _targets.Contains(next)))
            {
                ctx.Consume(); // Eat backslash target
                ctx.Append(next);
            }
            else
            {
                ctx.Append(c); // Treat backslash as literal
            }
        }
    }
}