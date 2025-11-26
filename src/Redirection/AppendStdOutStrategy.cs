// Strategy for ">>"
public class AppendStdOutStrategy : IRedirectionStrategy
{
    public void Apply(RedirectionConfig config, string filePath)
    {
        config.StdOutPath = filePath;
        config.AppendStdOut = true;
    }
}