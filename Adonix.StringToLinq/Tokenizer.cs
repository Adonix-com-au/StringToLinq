namespace Adonix.StringToLinq;

internal static class Tokenizer
{
    internal static List<Token> Parse(string input)
    {
        var tokens = new List<Token>();

        var shouldAddCommaAfter = false;
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
                tokens.Add(new Token(TokenType.Comma, ","));
                index++;
                continue;
            }

            if (input[index] == '(' || input[index] == ')')
            {
                tokens.Add(new Token(TokenType.Parenthesis, input[index].ToString()));
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

                tokens.Add(new Token(TokenType.Collection, token));
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

                tokens.Add(new Token(TokenType.Literal, token));
                continue;
            }

            while (index < input.Length && !char.IsWhiteSpace(input[index]) && input[index] != '(' &&
                   input[index] != ')')
            {
                if (input[index] == ',')
                {
                    shouldAddCommaAfter = true;
                    index++;
                    continue;
                }

                token += input[index];
                index++;
            }

            if (Operators.Functions.collection.Contains(token))
            {
                tokens.Add(new Token(TokenType.Function, token));
            }
            else if (Operators.Comparison.collection.Contains(token))
            {
                tokens.Add(new Token(TokenType.Operator, token));
            }
            else if (Operators.Logical.collection.Contains(token))
            {
                tokens.Add(new Token(TokenType.Logical, token));
            }
            else if (Operators.Arithmetic.collection.Contains(token))
            {
                tokens.Add(new Token(TokenType.Arithmetic, token));
            }
            else if (double.TryParse(token, out var _))
            {
                tokens.Add(new Token(TokenType.Literal, token));
            }
            else
            {
                tokens.Add(new Token(TokenType.Variable, token));
            }

            if (shouldAddCommaAfter)
            {
                shouldAddCommaAfter = false;
                tokens.Add(new Token(TokenType.Comma, ","));
            }
        }

        return tokens;
    }
}