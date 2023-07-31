namespace Adonix.StringToLinq;

internal class Node
{
    internal Node(Token token, Node left = null, Node right = null)
    {
        Token = token;
        Left = left;
        Right = right;
    }

    public Token Token { get; }
    public Node Left { get; set; }
    public Node Right { get; set; }
}