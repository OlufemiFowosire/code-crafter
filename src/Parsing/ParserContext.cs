using System.Text;

public class ParserContext
{
    private readonly string _input;
    private int _position;
    private readonly StringBuilder _buffer = new();

    public List<string> Arguments { get; } = new();

    // The State is now a Singleton reference
    public IParserState CurrentState { get; set; }

    public ParserContext(string input, IParserState initialState)
    {
        _input = input;
        CurrentState = initialState;
    }

    public bool HasMore() => _position < _input.Length;
    public char Consume() => _position < _input.Length ? _input[_position++] : '\0';
    public char Peek() => _position < _input.Length ? _input[_position] : '\0';
    public void Append(char c) => _buffer.Append(c);

    public void CommitArg()
    {
        if (_buffer.Length > 0)
        {
            Arguments.Add(_buffer.ToString());
            _buffer.Clear();
        }
    }

    public void Flush()
    {
        if (_buffer.Length > 0) Arguments.Add(_buffer.ToString());
    }
}