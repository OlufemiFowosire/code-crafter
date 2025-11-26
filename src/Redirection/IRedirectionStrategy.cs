interface IRedirectionStrategy
{
    // Applies the redirection logic to the configuration object
    void Apply(RedirectionConfig config, string filePath);
}