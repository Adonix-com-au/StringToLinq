namespace Adonix.StringToLinq;

internal class Node
{
    internal Node(Token token, TokenType type, Node left = null, Node right = null)
    {
        Token = token;
        Type = type;
        Left = left;
        Right = right;
    }

    public Token Token { get; }
    public TokenType Type { get; }
    public Node Left { get; set; }
    public Node Right { get; set; }
}