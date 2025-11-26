using System.Diagnostics;
using System.IO;
class Program
{
    static void Main()
    {
        while (true)
        {
            // TODO: Uncomment the code below to pass the first stage
            Console.Write("$ ");

            try
            {
                // Captures the user's command in the "command" variable
                string? input = Console.ReadLine()?.Trim();


                string[] tokens = ShellParser.Parse(input!);
                string command = tokens[0];
                CommandsFactory.Handle(command, tokens.Skip(1).ToArray());
            }
            catch (DirectoryNotFoundException d)
            {
                Console.WriteLine($"{d.Message}");
            }
            catch (ArgumentException a)
            {
                Console.WriteLine($"{a.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}
