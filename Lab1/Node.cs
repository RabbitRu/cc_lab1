namespace Lab1
{
    public class Node
    {
        public TokenType Type { get; set; }
        public char Value { get; set; }
        public Node Left { get; set; } = null;
        public Node Right { get; set; } = null;
        public int Pos { get; set; }

        public Node(Node leftNode, char value, TokenType type, Node rightNode, int pos)
        {
            Left = leftNode;
            Value = value;
            Type = type;
            Right = rightNode;
            Pos = pos;
        }

        public Node()
        {
        }
    }
}
