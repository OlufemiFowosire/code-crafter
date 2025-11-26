internal class RedirectionParser
{
    private static readonly Dictionary<string, IRedirectionStrategy> _strategies;

    static RedirectionParser()
    {
        // Register the tokens to their strategies
        _strategies = new Dictionary<string, IRedirectionStrategy>
        {
            { ">",   new RedirectStdOutStrategy() },
            { "1>",  new RedirectStdOutStrategy() }, // Handle explicit descriptor
            { ">>",  new AppendStdOutStrategy() },
            { "1>>", new AppendStdOutStrategy() },
            { "2>",  new RedirectStdErrStrategy() },
            { "2>>", new AppendStdErrStrategy() }
        };
    }

    public static RedirectionConfig Parse(string[] rawArgs)
    {
        var config = new RedirectionConfig();

        for (int i = 0; i < rawArgs.Length; i++)
        {
            string token = rawArgs[i];

            // 1. Check if the current token is a known redirection operator
            if (_strategies.TryGetValue(token, out var strategy))
            {
                // Ensure there is a filename after the operator
                if (i + 1 >= rawArgs.Length)
                {
                    throw new ArgumentException($"Missing filename after {token}");
                }

                string filePath = rawArgs[i + 1];

                // Execute the strategy
                strategy.Apply(config, filePath);

                // Skip the next element (the filename) since we consumed it
                i++;
            }
            else
            {
                // 2. It's a normal argument, add it to the list
                config.Arguments.Add(token);
            }
        }

        return config;
    }
}