using System.Collections.Generic;
using System.Linq;

namespace Lab1
{
    public static class TreeOps
    {
        public static bool Nullable(Node n)
        {
            switch (n.Type)
            {
                case TokenType.Star:
                    return true;
                case TokenType.Value:
                    return false;
                case TokenType.Or:
                    return Nullable(n.Left) || Nullable(n.Right);
                case TokenType.Cat:
                    return Nullable(n.Left) && Nullable(n.Right);
            }

            return false;
        }

        public static List<int> Firstpos(Node n)
        {
            List<int> temp;
            switch (n.Type)
            {
                case TokenType.Star:
                    return Firstpos(n.Left).OrderBy(i => i).ToList();
                case TokenType.Value:
                    var result = new List<int>();
                    result.Add(n.Pos);
                    return result.OrderBy(i => i).ToList();
                case TokenType.Or:
                    temp = Firstpos(n.Left);
                    temp.AddRange(Firstpos(n.Right));
                    return temp.OrderBy(i => i).ToList();
                case TokenType.Cat:
                    if (Nullable(n.Left))
                    {
                        temp = Firstpos(n.Left);
                        temp.AddRange(Firstpos(n.Right));
                        return temp.OrderBy(i => i).ToList();
                    }
                    else
                        return Firstpos(n.Left).OrderBy(i => i).ToList();
            }

            return null;

        }

        public static List<int> Lastpos(Node n)
        {
            List<int> temp;
            switch (n.Type)
            {
                case TokenType.Star:
                    return Lastpos(n.Left).OrderBy(i => i).ToList();
                case TokenType.Value:
                    var result = new List<int>();
                    result.Add(n.Pos);
                    return result.OrderBy(i => i).ToList();
                case TokenType.Or:
                    temp = Lastpos(n.Left);
                    temp.AddRange(Lastpos(n.Right));
                    return temp.OrderBy(i => i).ToList();
                case TokenType.Cat:
                    if (Nullable(n.Right))
                    {
                        temp = Lastpos(n.Left);
                        temp.AddRange(Lastpos(n.Right));
                        return temp.OrderBy(i => i).ToList();
                    }
                    else
                        return Lastpos(n.Right).OrderBy(i => i).ToList();
            }

            return null;
        }

        public static Dictionary<int,List<int>> Followpos(Node n, int maxPos)
        {
            var result = new Dictionary<int,List<int>>();
            for (int i = 1; i < maxPos; i++)
            {
                result.Add(i,new List<int>());
            }
            followposAllNodes(n,ref result);
            for (int i = 1; i < maxPos; i++)
            {
                result[i] = result[i].OrderBy(x => x).ToList();
            }
            return result;
        }

        private static void followposAllNodes(Node n, ref Dictionary<int, List<int>> d)
        {
            if(n==null)
                return;
            followposNodeAdd(n, ref d);
            followposAllNodes(n.Right, ref d);
            followposAllNodes(n.Left,ref d);
        }

        private static void followposNodeAdd(Node n, ref Dictionary<int, List<int>> d)
        {
            switch (n.Type)
            {
                case TokenType.Cat:
                    foreach (var i in Lastpos(n.Left))
                    {
                        d[i].AddRange(Firstpos(n.Right));
                    }
                    return;
                case TokenType.Star:
                    foreach (var i in Lastpos(n))
                    {
                        d[i].AddRange(Firstpos(n.Left));
                    }
                    return;
            }
        }

        public static Node TreePosSearch(Node n, int p)
        {
            if (n == null)
                return null;
            if (n.Pos == p)
                return n;
            var lSearch = TreePosSearch(n.Left, p);
            if (lSearch != null)
                return lSearch;
            var rSearch = TreePosSearch(n.Right, p);
            return rSearch;
        }
    }
}