namespace Adonix.StringToLinq;

internal class Node
{
    internal Node(Token token, Node left = null, Node right = null)
    {
        Token = token;
        Left = left;
        Right = right;
        Args = new List<Node>();
    }

    public Token Token { get; }
    public Node Left { get; set; }
    public List<Node> Args { get; set; } // Arguments for a function call

    public Node Right { get; set; }
}