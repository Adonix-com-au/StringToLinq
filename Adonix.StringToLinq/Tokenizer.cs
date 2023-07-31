namespace Adonix.StringToLinq;

internal static class Tokenizer
{
    internal static List<Token> Parse(string input)
    {
        string[] comparisonOperators = {"eq", "ne", "lt", "gt", "le", "ge", "in", "has"};
        string[] logicalOperators = {"and", "or", "and"};

        var tokens = new List<Token>();

        var index = 0;
        while (index < input.Length)
        {
            var token = string.Empty;

            if (char.IsWhiteSpace(input[index]))
            {
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
                token += input[index];
                index++;
            }

            if (comparisonOperators.Contains(token))
            {
                tokens.Add(new Token {Type = TokenType.Operator, Value = token});
            }
            else if (logicalOperators.Contains(token))
            {
                tokens.Add(new Token {Type = TokenType.Logical, Value = token});
            }
            else if (double.TryParse(token, out var number))
            {
                tokens.Add(new Token {Type = TokenType.Literal, Value = token});
            }
            else
            {
                tokens.Add(new Token {Type = TokenType.Variable, Value = token});
            }
        }

        return tokens;
    }
}