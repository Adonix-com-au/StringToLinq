namespace Adonix.StringToLinq;

internal static class Tokenizer
{
    internal static List<Token> Parse(string input)
    {
        var tokens = new List<Token>();

        var addComma = false;
        var index = 0;
        while (index < input.Length)
        {
            var token = string.Empty;

            if (char.IsWhiteSpace(input[index]))
            {
                index++;
                continue;
            }

            if (input[index] == ',')
            {
                tokens.Add(new Token {Type = TokenType.Comma, Value = ","});
                index++;
                continue;
            }

            if (input[index] == '(' || input[index] == ')')
            {
                tokens.Add(new Token
                {
                    Type = TokenType.Parenthesis,
                    Value = input[index].ToString()
                });
                index++;
                continue;
            }

            if (input[index] == '[')
            {
                index++;
                while (index < input.Length && input[index] != ']')
                {
                    token += input[index];
                    index++;
                }

                if (index < input.Length)
                {
                    index++;
                }

                tokens.Add(new Token {Type = TokenType.Collection, Value = token});
                continue;
            }

            if (input[index] == '"')
            {
                index++;
                while (index < input.Length && input[index] != '"')
                {
                    token += input[index];
                    index++;
                }

                if (index < input.Length)
                {
                    index++;
                }

                tokens.Add(new Token {Type = TokenType.Literal, Value = token});
                continue;
            }

            while (index < input.Length && !char.IsWhiteSpace(input[index]) && input[index] != '(' &&
                   input[index] != ')')
            {
                if (input[index] == ',')
                {
                    addComma = true;
                    index++;
                    continue;
                }

                token += input[index];
                index++;
            }

            if (Operators.Functions.collection.Contains(token))
            {
                tokens.Add(new Token {Type = TokenType.Function, Value = token});
            }
            else if (Operators.Comparison.collection.Contains(token))
            {
                tokens.Add(new Token {Type = TokenType.Operator, Value = token});
            }
            else if (Operators.Logical.collection.Contains(token))
            {
                tokens.Add(new Token {Type = TokenType.Logical, Value = token});
            }
            else if (Operators.Arithmetic.collection.Contains(token))
            {
                tokens.Add(new Token {Type = TokenType.Arithmetic, Value = token});
            }
            else if (double.TryParse(token, out var _))
            {
                tokens.Add(new Token {Type = TokenType.Literal, Value = token});
            }
            else
            {
                tokens.Add(new Token {Type = TokenType.Variable, Value = token});
            }

            if (addComma)
            {
                addComma = false;
                tokens.Add(new Token {Type = TokenType.Comma, Value = ","});
            }
        }

        return tokens;
    }
}