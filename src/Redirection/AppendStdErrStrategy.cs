// Strategy for "2>>"
public class AppendStdErrStrategy : IRedirectionStrategy
{
    public void Apply(RedirectionConfig config, string filePath)
    {
        config.StdErrPath = filePath;
        config.AppendStdErr = true;
    }
}