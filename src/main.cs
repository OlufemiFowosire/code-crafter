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


                string[] tokens = Argument.Parse(input!);
                string command = tokens[0];
                CommandsFactory.Handle(command, tokens.Skip(1).ToArray());
            }
            catch (DirectoryNotFoundException path)
            {
                Console.WriteLine($"cd: {path}: No such file or directory");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"{ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}
