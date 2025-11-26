// Strategy for ">"
public class RedirectStdOutStrategy : IRedirectionStrategy
{
    public void Apply(RedirectionConfig config, string filePath)
    {
        config.StdOutPath = filePath;
        config.AppendStdOut = false;
    }
}