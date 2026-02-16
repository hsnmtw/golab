namespace baby_json_parser;

using System;
using System.Diagnostics;

internal enum TokenKind {
    NULL,
    BOOLEAN,
    NUMBER,
    STRING,
    COLON,
    COMMA
}

internal sealed record Token(int Position, TokenKind Kind, string Value);

internal sealed class Tokenizer {
    private readonly string json;
    private int _position;

    public Tokenizer(string _json) {
        if (string.IsNullOrEmpty(_json)) _json = "";
        _json = _json.Trim();
        if (_json.Length < 2 || !(_json[0] == '{' && _json[^1] == '}')) _json = "";
        json = _json.Trim('{','}');
        _position = 0;
    }

    public Token? NextToken() {
        int start = 0;
        for(; _position < json.Length; ++_position) {
            switch (json[_position]) {
                case ' ' or '\t' or '\n' or '\r' or '\0': break;
                case '{' or '[': {
                    char open = json[_position];
                    char close = open == '{' ? '}' : ']';
                    // find matching closing brace
                    int braceCount = 1;
                    start = _position;
                    while (braceCount > 0 && ++_position < json.Length) {
                        if (json[_position] == open) {
                            braceCount++;
                        } else if (json[_position] == close) {
                            braceCount--;
                        }
                    }
                    if (braceCount == 0) {
                        return (new Token(start, TokenKind.STRING, json[start..(_position+1)]));
                    }        
                } break;
                case '"' or '\'': {
                    //string literal, find closing quote, should handle escaped quotes, and should handle unicode escapes
                    //should not allow newlines in string literals
                    char quote = json[_position];
                    start = _position + 1;
                    int escapeCount = 0;
                    while (++_position < json.Length) {
                        if (json[_position] == '\\') {
                            escapeCount++;
                        } else if (json[_position] == quote && escapeCount % 2 == 0) {
                            break;
                        } else {
                            escapeCount = 0;
                        }
                        if (json[_position] == '\n' || json[_position] == '\r') {
                            // newlines are not allowed in string literals
                            return null;
                        }        
                    }
                    if(_position < json.Length && json[_position] == quote) {
                        return (new Token(start, TokenKind.STRING, ParseUnicodeEscapes(json[start.._position++])));
                    }                    
                } break;
                case ':': return (new Token(_position++, TokenKind.COLON, ":")); 
                case ',': return (new Token(_position++, TokenKind.COMMA, ",")); 
                default: {
                    if (char.IsDigit(json[_position]) || json[_position] == '-') {
                        start = _position;
                        while (++_position < json.Length && (char.IsDigit(json[_position]) || json[_position] == '.' || json[_position] == 'e' || json[_position] == 'E' || json[_position] == '+' || json[_position] == '-'));
                        return (new Token(start, TokenKind.NUMBER, json[start.._position]));
                    } else if (_position+4 <= json.Length && json[_position..(_position+4)] == "true") {
                        _position += 4;
                        return (new Token(_position-4, TokenKind.BOOLEAN, "true"));
                    } else if (_position+5 <= json.Length && json[_position..(_position+5)] == "false") {
                        _position += 5;
                        return (new Token(_position-5, TokenKind.BOOLEAN, "false"));
                    } else if (_position+4 <= json.Length && json[_position..(_position+4)] == "null") {
                        _position += 4;
                        return (new Token(_position-4, TokenKind.NULL, "null"));
                    }
                } break;
            }
        }
        return null;
    }

    public static string ParseUnicodeEscapes(string v)
    {
        // should handle unicode escapes in the form of \uXXXX where X is a hex digit
        // should also handle escaped backslashes and escaped quotes
        return System.Text.RegularExpressions.Regex.Replace(v, @"\\(u[0-9a-fA-F]{4}|.)", match => {
            string escape = match.Groups[1].Value;
            if (escape.Length == 5 && escape[0] == 'u') {
                string hex = escape[1..];
                if (int.TryParse(hex, System.Globalization.NumberStyles.HexNumber, null, out int codePoint)) {
                    return char.ConvertFromUtf32(codePoint);
                }
            } else {
                return escape switch {
                    "\"" => "\"",
                    "\\" => "\\",
                    "/" => "/",
                    "b" => "\b",
                    "f" => "\f",
                    "n" => "\n",
                    "r" => "\r",
                    "t" => "\t",
                    _ => match.Value
                };
            }
            return match.Value;
        });
    }
}

public static class JsonParser {    
    public static void Parse(string json, Action<string, string> onKeyValue) {
        Tokenizer tokenizer = new(json);
        Token? token;
        while ((token = tokenizer.NextToken()) != null) {
            Debug.WriteLine("Token: {0} ({1}) @ {2}", token.Value, token.Kind, token.Position);
            if (token.Kind != TokenKind.STRING) break;
            string key = token.Value;
            token = tokenizer.NextToken();
            if (token == null || token.Kind != TokenKind.COLON) {
                Debug.WriteLine("Expected colon after key at position {0}, but got {1} ({2})", token?.Position, token?.Kind.ToString() ?? "null", token?.Value ?? "null");
                break;
            }
            token = tokenizer.NextToken();
            if (token == null || (token.Kind != TokenKind.STRING && token.Kind != TokenKind.NUMBER && token.Kind != TokenKind.BOOLEAN && token.Kind != TokenKind.NULL)) {
                Debug.WriteLine("Expected value after colon at position {0}", token?.Position);
                break;
            }
            string value = token.Value;
            onKeyValue(key, value);
            token = tokenizer.NextToken();
            if (token == null) break;
            if (token.Kind != TokenKind.COMMA) {
                Debug.WriteLine("Expected comma or end of object at position {0}", token.Position);
                break;            
            } 
        }
    }
}