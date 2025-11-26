// Strategy for "2>"
public class RedirectStdErrStrategy : IRedirectionStrategy
{
    public void Apply(RedirectionConfig config, string filePath)
    {
        config.StdErrPath = filePath;
        config.AppendStdErr = false;
    }
}