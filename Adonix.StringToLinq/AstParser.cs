using Microsoft.Extensions.Logging;

namespace Adonix.StringToLinq;

internal class AstParser
{
    private readonly List<Token> _tokens;
    private int _index;
    private Node _root;

    internal AstParser(List<Token> tokens)
    {
        _tokens = tokens;
    }

    private Token GetToken(int offset)
    {
        var index = _index + offset;
        if (index >= _tokens.Count)
        {
            return new Token {Type = TokenType.EOF};
        }

        return _tokens[index];
    }

    private Token GetCurrent()
    {
        return GetToken(0);
    }

    private Token GetNext()
    {
        return GetToken(1);
    }

    private void EatToken()
    {
        _index++;
    }

    public Node Parse()
    {
        _root = ParseExpression();
        return _root;
    }

    private Node ParseExpression()
    {
        var expr = ParseTerm();

        while (GetCurrent().Type == TokenType.Logical)
        {
            var op = GetCurrent();
            EatToken();
            var right = ParseTerm();
            expr = new Node(op, expr, right);
        }

        return expr;
    }

    private Node ParseTerm()
    {
        var term = ParseArithmetic();

        while (GetCurrent().Type == TokenType.Operator)
        {
            var op = GetCurrent();
            EatToken();
            var right = ParseArithmetic();
            term = new Node(op, term, right);
        }

        return term;
    }

    private Node ParseArithmetic()
    {
        var factor = ParseFactor();

        while (GetCurrent().Type == TokenType.Arithmetic)
        {
            var op = GetCurrent();
            EatToken();
            var right = ParseFactor();
            factor = new Node(op, factor, right);
        }

        return factor;
    }

    private Node ParseFunctionCall()
    {
        var functionToken = GetCurrent();
        EatToken(); // Eat the function token
        EatToken(); // Eat the '(' token

        var args = new List<Node>();
        while (GetCurrent().Type != TokenType.Parenthesis || GetCurrent().Value != ")")
        {
            args.Add(ParseExpression());
            if (GetCurrent().Type == TokenType.Comma)
            {
                EatToken(); // Eat the comma
            }
        }

        EatToken(); // Eat the ')' token

        return new Node(functionToken, args.ToArray());
    }

    private Node ParseFactor()
    {
        var token = GetCurrent();
        if (token.Type == TokenType.Variable || token.Type == TokenType.Literal || token.Type == TokenType.Collection)
        {
            EatToken();
            return new Node(token);
        }

        if (token.Type == TokenType.Function)
        {
            return ParseFunctionCall();
        }

        if (token.Type == TokenType.Parenthesis && token.Value == "(")
        {
            EatToken(); // eat '('
            var node = ParseExpression();
            if (GetCurrent().Type == TokenType.Parenthesis && GetCurrent().Value == ")")
            {
                EatToken(); // eat ')'
                return node;
            }
        }

        throw new Exception("Expected a variable, literal or parenthesis, got " + GetCurrent().Type);
    }

#if DEBUG
    public void Print(ILogger logger)
    {
        Print(_root, logger);
    }

    private void Print(Node node, ILogger logger, int depth = 0)
    {
        if (node == null)
        {
            return;
        }

        var indent = new string('\t', depth);

        logger.LogInformation($"{indent}Node: {node.Token.Value}");

        if (node.Token.Type == TokenType.Function)
        {
            if (node.Children.Length > 0)
            {
                var value = $"{indent}Children: [";
                foreach (var arg in node.Children)
                {
                    value += $"{arg.Token.Value} ";
                }

                value = value.TrimEnd();

                value += "]";
                logger.LogInformation(value);
            }
        }
        else
        {
            if (node.Left != null)
            {
                logger.LogInformation($"{indent}Left:");
                Print(node.Left, logger, depth + 1);
            }

            if (node.Right != null)
            {
                logger.LogInformation($"{indent}Right:");
                Print(node.Right, logger, depth + 1);
            }
        }
    }
#endif
}