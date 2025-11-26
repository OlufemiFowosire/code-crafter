using System.ComponentModel;
using System.Diagnostics;
using System.IO;
class Program
{
    static void Main()
    {
        // 1. Setup Autocomplete System
        var sources = new List<ICompletionSource>
        {
            new BuiltinCompletionSource(),
            new ExecutableCompletionSource()
        };
        var engine = new CompletionEngine(sources);
        var lineEditor = new LineEditor(engine);
        while (true)
        {
            // REPLACED: Console.Write("$ "); string input = Console.ReadLine();
            // WITH:
            string input = lineEditor.ReadLine("$ ");

            if (string.IsNullOrWhiteSpace(input)) continue;
            // Use your existing quote parser to split string
            string[] rawArgs = ShellParser.Parse(input!);

            try
            {
                // 3. Extract Redirection info using the new Parser
                RedirectionConfig config = RedirectionParser.Parse(rawArgs);

                if (config.Arguments.Count == 0) continue;

                string cmdName = config.Arguments[0];
                string[] cmdArgs = config.Arguments.Skip(1).ToArray();

                // 4. Execute
                ICommand command = CommandFactory.Create(cmdName);

                using (new OutputRedirectionContext(config))
                {
                    command.Execute(cmdArgs);
                }
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
