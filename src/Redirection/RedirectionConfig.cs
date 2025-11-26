public class RedirectionConfig
{
    public string? StdOutPath { get; set; }
    public bool AppendStdOut { get; set; }

    public string? StdErrPath { get; set; }
    public bool AppendStdErr { get; set; }

    public List<string> Arguments { get; } = new();
}