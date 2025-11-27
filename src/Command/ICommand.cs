using System.IO;
using System.Threading.Tasks;

public interface ICommand
{
    string Name { get; }
    // Now accepts streams for input/output and returns a Task for async execution
    Task ExecuteAsync(string[] args, Stream? stdin, Stream? stdout, Stream? stderr);
}