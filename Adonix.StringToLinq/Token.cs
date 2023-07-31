﻿namespace Adonix.StringToLinq;

internal enum TokenType
{
    Operator,
    Arithmetic,
    Logical,
    Literal,
    Variable,
    Parenthesis,
    EOF
}

internal class Token
{
    public Token()
    {
    }

    public Token(string value, TokenType type)
    {
        Value = value;
        Type = type;
    }

    public string Value { get; set; }
    public TokenType Type { get; set; }
}