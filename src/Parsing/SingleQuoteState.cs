// --- SINGLE QUOTE ---
public class SingleQuoteState : SingletonState<SingleQuoteState>
{
    protected override Dictionary<char, ICharAction> ActionMap { get; } = new()
    {
        { '\'', new TransitionAction(() => UnquotedState.Instance) }
        // Default is Append (Literal)
    };
}