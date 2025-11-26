// --- DOUBLE QUOTE ---
public class DoubleQuoteState : SingletonState<DoubleQuoteState>
{
    protected override Dictionary<char, ICharAction> ActionMap { get; } = new()
    {
        { '"',  new TransitionAction(() => UnquotedState.Instance) },
        { '\\', Actions.EscapeComplex }
    };
}