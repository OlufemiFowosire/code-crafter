public interface ICompletionSource
{
    // Returns all possible values (e.g., all builtins, or all files in PATH)
    IEnumerable<string> GetOptions();
}