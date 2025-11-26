public class BuiltinCompletionSource : ICompletionSource
{
    // We access your existing Registry logic here
    public IEnumerable<string> GetOptions() => CommandRegistry.GetAllBuiltinNames();
}