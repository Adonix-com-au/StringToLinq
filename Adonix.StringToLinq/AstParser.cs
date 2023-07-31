namespace Adonix.StringToLinq;

internal class AstParser
{
    private readonly List<Token> _tokens;
    private int _index;

    public AstParser(List<Token> tokens)
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
        return ParseExpression();
    }

    private Node ParseExpression()
    {
        var expr = ParseTerm();

        while (GetCurrent().Type == TokenType.Logical)
        {
            var op = GetCurrent();
            EatToken();
            var right = ParseTerm();
            expr = new Node(op, op.Type, expr, right);
        }

        return expr;
    }

    private Node ParseTerm()
    {
        var term = ParseFactor();

        while (GetCurrent().Type == TokenType.Operator)
        {
            var op = GetCurrent();
            EatToken();
            var right = ParseFactor();
            term = new Node(op, op.Type, term, right);
        }

        return term;
    }

    private Node ParseFactor()
    {
        var token = GetCurrent();
        if (token.Type == TokenType.Variable || token.Type == TokenType.Literal)
        {
            EatToken();
            return new Node(token, token.Type);
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
}