public class ShellParser
{
    public static string[] Parse(string commandLine)
    {
        // Start with the Singleton UnquotedState
        var context = new ParserContext(commandLine, UnquotedState.Instance);

        while (context.HasMore())
        {
            context.CurrentState.HandleNext(context);
        }

        context.Flush();
        return context.Arguments.ToArray();
    }
}