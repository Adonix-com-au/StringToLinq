namespace Adonix.StringToLinq;

internal class Node
{
    internal Node(Token token, Node left, Node right)
    {
        Token = token;
        Children = new[] {left, right};
    }

    internal Node(Token token, params Node[] nodes)
    {
        Token = token;
        Children = nodes;
    }

    public Token Token { get; }

    public Node Left => Children.Length > 0 ? Children[0] : null;
    public Node Right => Children.Length > 1 ? Children[1] : null;

    public Node[] Children { get; }
}