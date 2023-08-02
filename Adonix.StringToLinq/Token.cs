namespace Adonix.StringToLinq;

internal enum TokenType
{
    Operator,
    Arithmetic,
    Logical,
    Literal,
    Function,
    Comma,
    Variable,
    Collection,
    Parenthesis,
    EOF
}

internal class Token
{
    internal Token(TokenType type, string value)
    {
        Value = value;
        Type = type;
    }

    internal string Value { get; set; }
    internal TokenType Type { get; set; }
}