public abstract class SingletonState<T> : IParserState where T : SingletonState<T>, new()
{
    // The Singleton Instance
    public static readonly T Instance = new T();

    // The Logic Map
    protected abstract Dictionary<char, ICharAction> ActionMap { get; }

    public void HandleNext(ParserContext context)
    {
        char c = context.Consume();

        // Zero allocation lookup
        if (ActionMap.TryGetValue(c, out var action))
        {
            action.Execute(context, c);
        }
        else
        {
            Actions.Append.Execute(context, c);
        }
    }
}