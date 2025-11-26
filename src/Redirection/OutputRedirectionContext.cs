public class OutputRedirectionContext : IDisposable
{
    private readonly TextWriter _originalOut = Console.Out;
    private readonly TextWriter _originalErr = Console.Error;
    private readonly StreamWriter? _newOut;
    private readonly StreamWriter? _newErr;

    public OutputRedirectionContext(RedirectionConfig config)
    {
        if (config.StdOutPath != null)
        {
            var mode = config.AppendStdOut ? FileMode.Append : FileMode.Create;
            var fs = new FileStream(config.StdOutPath, mode, FileAccess.Write);
            _newOut = new StreamWriter(fs) { AutoFlush = true };
            Console.SetOut(_newOut);
        }

        if (config.StdErrPath != null)
        {
            var mode = config.AppendStdErr ? FileMode.Append : FileMode.Create;
            var fs = new FileStream(config.StdErrPath, mode, FileAccess.Write);
            _newErr = new StreamWriter(fs) { AutoFlush = true };
            Console.SetError(_newErr);
        }
    }

    public void Dispose()
    {
        Console.SetOut(_originalOut);
        Console.SetError(_originalErr);
        _newOut?.Dispose();
        _newErr?.Dispose();
    }
}