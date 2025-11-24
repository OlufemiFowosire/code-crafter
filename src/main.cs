class Program
{
    static void Main()
    {
        while (true)
        {
            // TODO: Uncomment the code below to pass the first stage
            Console.Write("$ ");

            // Captures the user's command in the "command" variable
            string? command = Console.ReadLine()?.Trim();

            if (string.IsNullOrEmpty(command) || command.ToLower() == "exit")
            {
                break;
            }

            if (command.Split(' ')[0] == "echo")
            {
                string echoOutput = command.Substring(5); // Get everything after "echo "
                Console.WriteLine(echoOutput);
                continue;
            }

            Console.WriteLine($"{command}: command not found");
        }
    }
}
