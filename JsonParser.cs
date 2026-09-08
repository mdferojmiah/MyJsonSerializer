using System.Globalization;
using System.Text;

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
            '{' => ParseObject(),
            '[' => ParseCollection(),
            '"' => ParseString(),
            'f' or 't' => ParseBoolean(),
            'n' => ParseNull(),
            _ when char.IsDigit(ch) || ch == '-' => ParseNumber(),
            _ => throw new Exception($"Unexpected character at position {_position}: '{ch}'")
        };
    }

    private Dictionary<string, object> ParseObject()
    {
        var dictionary = new Dictionary<string, object>();
        Expect('{');
        while (true)
        {
            var key = ParseString();
            Expect(':');
            var value = ParseValue();
            dictionary[key] = value!;

            if(Peek() == '}')
            {
                _position++;
                return dictionary;
            }
            Expect(',');
        }
    }

    private List<object?> ParseCollection()
    {
        var resultList = new List<object?>();
        Expect('[');

        while (true)
        {
            SkipWhiteSpace();
            var value = ParseValue();
            resultList.Add(value);
            
            if(Peek() == ']')
            {
                _position++;
                return resultList;
            }
            Expect(',');
        }
    }

    private string ParseString()
    {
        StringBuilder result = new StringBuilder();
        Expect('"');
        while (true)
        {
            var ch = Next();
            if (ch == '"')
            {
                return result.ToString();
            }

            if(ch == '\\')
            {
                var next = Next();
                ch = next switch
                {
                    '"' => '"',
                    '\\' => '\\',
                    '/' => '/',
                    'b' => '\b',
                    'f' => '\f',
                    'n' => '\n',
                    'r' => '\r',
                    't' => '\t',
                    _ => throw new Exception($"Invalid escape sequence at position {_position}")
                };
            }
            result.Append(ch);
        }
    }

    private decimal ParseNumber()
    {
        var start = _position;

        if (_json[_position] == '-') _position++;

        if (_position >= _json.Length || !char.IsDigit(_json[_position]))
            throw new Exception($"Expected digit at position {_position}");

        while (_position < _json.Length && char.IsDigit(_json[_position]))
            _position++;
        
        if(_position < _json.Length && _json[_position] == '.')
        {
            _position++;
            
            if (_position >= _json.Length || !char.IsDigit(_json[_position]))
                throw new Exception($"Expected digit after decimal at position {_position}");
            
            while (_position < _json.Length && char.IsDigit(_json[_position]))
                _position++;
        }

        var numberString = _json.Substring(start, _position - start);
        if (!decimal.TryParse(numberString, NumberStyles.Float, CultureInfo.InvariantCulture, out decimal result))
            throw new Exception($"Invalid Number format: {numberString}");

        return result;
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