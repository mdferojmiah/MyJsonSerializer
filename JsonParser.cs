namespace MyJsonSerializer;

public class JsonParser
{
    private readonly string _json;
    private int _position;
    public JsonParser(string json)
    {
        _json = json;
        _position = 0;
    }

    public object? Parse()
    {
        SkipWhiteSpace();
        var result = ParseValue();
        SkipWhiteSpace();

        if (_position < _json.Length)
            throw new Exception($"Unexpected character at position {_position}");

        return result;
    }

    private object? ParseValue()
    {
        SkipWhiteSpace();

        var ch = Peek();

        return ch switch
        {
            // '{' => ParseObject(),
            // '[' => ParseCollection(),
            // '"' => ParseString(),
            'f' or 't' => ParseBoolean(),
            'n' => ParseNull(),
            //_ when char.IsDigit(ch) || ch == '-' => PaseNumber(),
            _ => throw new Exception($"Unexpected character at position {_position}: '{ch}'")
        };
    }

    private bool ParseBoolean()
    {
        if(_position + 5 <= _json.Length && _json.Substring(_position, 5) == "false")
        {
            _position += 5;
            return false;
        }
        if(_position + 4 <= _json.Length && _json.Substring(_position, 4) == "true")
        {
            _position += 4;
            return true;
        }

        throw new Exception($"Expect 'true' or 'false' at positon {_position}");
    }

    private object? ParseNull()
    {
        if (_position + 4 <= _json.Length && _json.Substring(_position, 4) == "null")
        {
            _position += 4;
            return null;
        }
        
        throw new Exception($"Expected 'null' at position {_position}");
    }



    // helper methods
    private char Peek() => _json[_position];
    private char Next() => _json[_position++];
    private void Expect(char ch)
    {
        SkipWhiteSpace();

        if (_position >= _json.Length)
            throw new Exception($"Expected {ch} but reached end of input!");

        if (_json[_position] != ch)
            throw new Exception($"Expected {ch} but got {_json[_position]} at postion {_position}");
        
        _position++;
    }
    private void SkipWhiteSpace()
    {
        while(_position < _json.Length && char.IsWhiteSpace(_json[_position]))
        {
            _position++;
        }
    }
}