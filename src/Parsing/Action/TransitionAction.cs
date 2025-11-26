// 3. Switch to a different State (handle quotes)
/* public class TransitionAction : ICharAction
{
    private readonly Func<IParserState> _nextStateFactory;

    public TransitionAction(Func<IParserState> nextStateFactory)
    {
        _nextStateFactory = nextStateFactory;
    }

    public void Execute(ParserContext context, char c)
    {
        // Don't append the quote itself, just switch state
        context.CurrentState = _nextStateFactory();
    }
} */

// Special Action: Switches State
// We use a Func<> to resolve the singleton lazily to avoid circular static init issues.
public class TransitionAction : ICharAction
{
    private readonly Func<IParserState> _targetStateProvider;
    public TransitionAction(Func<IParserState> provider) => _targetStateProvider = provider;
    public void Execute(ParserContext ctx, char c) => ctx.CurrentState = _targetStateProvider();
}