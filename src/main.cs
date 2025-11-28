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

        // [NEW] Save executed command to history
        HistoryService.Instance.Add(lineEditor.ReadLine("$ "));

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
                // It uses spaces to split, but keeps quoted sections together
                string[] allTokens = ShellParser.Parse(input!);

                // 2. Execute Pipeline
                // This handles Splitting by '|', Redirection, and Execution internally
                await pipelineExecutor.ExecutePipelineAsync(allTokens);
            }
            catch (DirectoryNotFoundException directoryEx)
            {
                Console.WriteLine($"{directoryEx.Message}");
            }
            catch (ArgumentException argEx)
            {
                Console.WriteLine($"{argEx.Message}");
            }
            catch (AggregateException a)
            {
                Console.WriteLine($"{a.Message}");
            }
            catch (Win32Exception windowsEx)
            {
                Console.WriteLine($"{windowsEx.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}