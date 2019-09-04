using System;
using System.Collections.Generic;
using System.Linq;

namespace Lab1
{
    public static class DFSABuilder
    {
        public static ((string poses, bool ending)[] States, Dictionary<string, Dictionary<char, string>> Trans, char[] InputDict)
            Build(Node root, int maxPos)
        {
            List<(List<int> poses, bool marked)> DStates = new List<(List<int>, bool marked)>();
            Dictionary<List<int>, Dictionary<char, List<int>>> DTrans =
                new Dictionary<List<int>, Dictionary<char, List<int>>>();
            int endingPos = -1;
            DStates.Add((TreeOps.Firstpos(root), false));
            DTrans.Add(DStates.First().poses, new Dictionary<char, List<int>>());
            List<Node> poses = new List<Node>();
            List<char> inputDictionary = new List<char>();
            for (int i = 1; i <= maxPos; i++)
            {
                poses.Add(TreeOps.TreePosSearch(root, i));
                if (poses.Last().Value == '#')
                    endingPos = i;
                else
                    inputDictionary.Add(poses.Last().Value);
            }

            inputDictionary = inputDictionary.Distinct().ToList();
            var fp = TreeOps.Followpos(root, maxPos);

            while (DStates.Any(x => x.marked == false))
            {
                var S = DStates.First(x => x.marked == false);
                DStates.Remove(S);
                S.marked = true;
                DStates.Add(S);
                foreach (var letter in inputDictionary)
                {
                    var matchingPoses = poses.Where(x => x.Value == letter && S.poses.Any(y => y == x.Pos));
                    List<int> U = new List<int>();
                    foreach (var pos in matchingPoses)
                    {
                        U.AddRange(fp[pos.Pos]);
                    }

                    U = U.Distinct().OrderBy(i => i).ToList();
                    if (!DStates.Any(x => x.poses.SequenceEqual(U)))
                    {
                        DStates.Add((U, false));
                        DTrans.Add(U, new Dictionary<char, List<int>>());
                    }

                    DTrans[S.poses].Add(letter, U);
                }
            }

            List<(string poses, bool ending)> DStatesWOMarks = new List<(string poses, bool ending)>();
            foreach (var state in DStates)
            {
                DStatesWOMarks.Add((Helpers.ArrayToString(state.poses), state.poses.Contains(endingPos)));
            }

            Dictionary<string, Dictionary<char, string>> DTransArray =
                new Dictionary<string, Dictionary<char, string>>();
            foreach (var trans in DTrans)
            {
                Dictionary<char, string> insides = new Dictionary<char, string>();
                foreach (var letter in trans.Value)
                {
                    insides.Add(letter.Key, Helpers.ArrayToString(letter.Value));
                }

                DTransArray.Add(Helpers.ArrayToString(trans.Key), insides);
            }

            return (DStatesWOMarks.ToArray(), DTransArray, inputDictionary.ToArray());
        }
    }
}