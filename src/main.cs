class Program
{
    static void Main()
    {
        do
        {
            // TODO: Uncomment the code below to pass the first stage
            Console.Write("$ ");

            // Captures the user's command in the "command" variable
            string command = Console.ReadLine();
            Console.WriteLine($"{command}: command not found");
        } while (true);
    }
}
