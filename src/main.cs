using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

class Program
{
    static async Task Main()
    {
        // 1. Setup Autocomplete System (Existing)
        var sources = new List<ICompletionSource>
        {
            new BuiltinCompletionSource(),
            new ExecutableCompletionSource()
        };
        var engine = new CompletionEngine(sources);
        var lineEditor = new LineEditor(engine);

        // 2. Setup Pipeline Executor (New)
        var pipelineExecutor = new PipelineExecutor();

        while (true)
        {
            // Interactive Read
            string input = lineEditor.ReadLine("$ ");

            if (string.IsNullOrWhiteSpace(input)) continue;

            try
            {
                // 1. Parse into tokens (preserves quotes)
                string[] allTokens = ShellParser.Parse(input!);

                // 2. Execute Pipeline
                // This handles Splitting by '|', Redirection, and Execution internally
                await pipelineExecutor.ExecutePipelineAsync(allTokens);
            }
            catch (DirectoryNotFoundException d)
            {
                Console.WriteLine($"{d.Message}");
            }
            catch (ArgumentException a)
            {
                Console.WriteLine($"{a.Message}");
            }
            catch (Win32Exception w)
            {
                Console.WriteLine($"{w.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}