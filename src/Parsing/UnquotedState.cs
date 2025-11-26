// --- UNQUOTED ---
public class UnquotedState : SingletonState<UnquotedState>
{
    // Initialize Map Once
    protected override Dictionary<char, ICharAction> ActionMap { get; } = new()
    {
        { ' ',  Actions.Commit },
        { '\\', Actions.EscapeAll },
        { '\'', new TransitionAction(() => SingleQuoteState.Instance) },
        { '"',  new TransitionAction(() => DoubleQuoteState.Instance) }
    };
}