using System.Linq;

namespace Lab1
{
    public class TreeBuilder
    {
        private char currentToken;
        public int pos = 0;
        private string source;

        private void getNextToken()
        {
            currentToken = source.First();
            source = StringPrecut(source);
        }
        
        public Node StartParse(string s)
        {
            source = s;
            pos = 0;
            getNextToken();
            return ParseAll();
        }

        public Node ParseAll()
        {
            var n = new Node();
            while (currentToken != '#' && currentToken != ')')
            {
                n = GetNode(n);
                getNextToken();
            }

            if (currentToken == '#')
            {
                pos++;
                return new Node(n, '.', TokenType.Cat, new Node(null, currentToken, TokenType.Value, null, pos), -1);
            }

            return n;
        }

        public Node GetNode(Node root)
        {
            switch (currentToken)
            {
                case '(':
                    getNextToken();
                    return ParseAll();
                case '*':
                    return new Node(root, '*', TokenType.Star, null, -1);
                case '.':
                    getNextToken();
                    return new Node(root, '.', TokenType.Cat, GetNode(new Node()), -1);
                case '|':
                    getNextToken();
                    return new Node(root, '|', TokenType.Or, GetNode(new Node()), -1);
                case ')':
                    return root;
                default:
                    pos++;
                    return new Node(null, currentToken, TokenType.Value, null, pos);
            }
        }

        public string StringPrecut(string s)
        {
            return s.Substring(1, s.Length - 1);
        }
    }
}