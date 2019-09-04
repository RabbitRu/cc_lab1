using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Lab1
{
    public static class DFSAMinimizer
    {
        public static ((string poses, bool ending)[] DStates, Dictionary<string, Dictionary<char, string>> DTrans)
            Minimize(
                (string poses, bool ending)[] DStates,
                Dictionary<string, Dictionary<char, string>> DTrans,
                char[] dictionary)
        {
            bool[,] marked = new bool[DStates.Length + 1, DStates.Length + 1];
            Dictionary<string, Dictionary<char, List<string>>> BTrans =
                new Dictionary<string, Dictionary<char, List<string>>>();
            bool[] achivedFromStart = new bool[DStates.Length + 1];
            foreach (var state in DStates) //Шаг1
            {
                bool firstLetter = true;
                foreach (var letter in dictionary)
                {
                    if (firstLetter)
                    {
                        BTrans.Add(state.poses, new Dictionary<char, List<string>>());
                        BTrans.Last().Value.Add(letter, BTransBuilder(DTrans, state.poses, letter));
                        firstLetter = false;
                    }
                    else
                    {
                        BTrans.Last().Value.Add(letter, BTransBuilder(DTrans, state.poses, letter));
                    }
                }
            }

            BTrans.Add("0", new Dictionary<char, List<string>>()); //Вершина 0

            foreach (var letter in dictionary)
            {
                BTrans.Last().Value.Add(letter, new List<string>());
                BTrans[DStates.First().poses][letter].Add("0");
            }

            var DStatesWo = new Func<(string poses, bool ending)[]>(() =>
            {
                List<(string poses, bool ending)> temp = new List<(string poses, bool ending)>();
                temp.Add(("0", false));
                temp.AddRange(DStates);
                return temp.ToArray();
            }).Invoke();

            achivedFromStart = AchivedFromDFS(DStates, DTrans); //Шаг2;

            marked = buildTable(DStatesWo.Length, DStatesWo, BTrans, dictionary); //Шаг 3\4

            int[] component = new int[DStatesWo.Length];
            for (int i = 0; i < DStatesWo.Length; i++)
            {
                component[i] = -1;
            }

            for (int i = 0; i < DStatesWo.Length - 1; i++)
            {
                if (!marked[0, i])
                    component[i] = 0;
            }

            int componentsCount = 0;

            for (int i = 1; i < DStatesWo.Length; i++)
            {
                if (!achivedFromStart[i])
                    continue;
                if (component[i] == -1)
                {
                    componentsCount++;
                    component[i] = componentsCount;
                    for (int j = i + 1; j < DStatesWo.Length; j++)
                    {
                        if (!marked[i, j])
                            component[j] = componentsCount;
                    }
                }
            }

            Dictionary<int, List<string>> componentsToOld = new Dictionary<int, List<string>>();
            for (int i = 0; i < DStatesWo.Length; i++)
            {
                if (!componentsToOld.ContainsKey(component[i]))
                    componentsToOld.Add(component[i], new List<string>());
                componentsToOld[component[i]].Add(DStatesWo[i].poses);
            }

            (string poses, bool ending)[] minStates = new (string poses, bool ending)[componentsCount + 1];
            Dictionary<string, Dictionary<char, string>> minTrans = new Dictionary<string, Dictionary<char, string>>();
            for (int i = 0; i <= componentsCount; i++)
            {
                minStates[i] = (Helpers.ArrayToString(componentsToOld[i]),
                    DStatesWo[Helpers.IndexOfHelper(DStatesWo, componentsToOld[i].First())].ending);
                minTrans.Add(minStates[i].poses, new Dictionary<char, string>());
            }

            for (int i = 0; i <= componentsCount; i++)
            {
                foreach (var letter in dictionary)
                {
                    string oldTrans;
                    if (componentsToOld[i].First() == "0")
                    {
                        if (componentsToOld[i].Count == 1)
                        {
                            minTrans[minStates[i].poses].Add(letter, minStates[1].poses);
                            continue;
                        }

                        oldTrans = DTrans[componentsToOld[i].Last()][letter];
                    }
                    else
                        oldTrans = DTrans[componentsToOld[i].First()][letter];

                    var transToIndex = Helpers.IndexOfHelper(DStatesWo, oldTrans);

                    minTrans[minStates[i].poses].Add(letter, minStates[component[transToIndex]].poses);
                }
            }
            //Это костыль чтобы убрать 0 вершину
            var returnStates = new (string poses, bool ending)[minStates.Length - 1];
            for (int i = 1; i < minStates.Length; i++)
                returnStates[i - 1] = minStates[i];
            var returnTrans = new Dictionary<string, Dictionary<char, string>>();
            for(int i=1;i<minTrans.Count;i++)
                returnTrans.Add(minTrans.Keys.ToArray()[i],minTrans.Values.ToArray()[i]);
            return (returnStates, returnTrans);
        }

        private static bool[,] buildTable(int n, (string poses, bool ending)[] DStates,
            Dictionary<string, Dictionary<char, List<string>>> BTrans, char[] dictionary)
        {
            Queue<(int, int)> Q = new Queue<(int, int)>();
            bool[,] marked = new bool[n, n];
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (!marked[i, j] && DStates[i].ending != DStates[j].ending)
                    {
                        marked[i, j] = true;
                        marked[j, i] = true;
                        Q.Enqueue((i, j));
                    }
                }
            }

            while (Q.Count > 0)
            {
                var currentElem = Q.Dequeue();
                foreach (var letter in dictionary)
                {
                    foreach (var state1 in BTrans[DStates[currentElem.Item1].poses][letter])
                    {
                        foreach (var state2 in BTrans[DStates[currentElem.Item2].poses][letter])
                        {
                            int index1 = Helpers.IndexOfHelper(DStates, state1);
                            int index2 = Helpers.IndexOfHelper(DStates, state2);
                            if (!marked[index1, index2])
                            {
                                marked[index1, index2] = true;
                                marked[index2, index1] = true;
                                Q.Enqueue((index1, index2));
                            }
                        }
                    }
                }

            }

            return marked;
        }

        private static bool[] AchivedFromDFS( //Будем считать что первое состояние стартовое
            (string poses, bool ending)[] DStates, Dictionary<string, Dictionary<char, string>> DTrans)
        {
            Dictionary<string, bool> DStatesR = new Dictionary<string, bool>();
            foreach (var state in DStates.Select(x => x.poses).ToList())
            {
                DStatesR.Add(state, false);
            }

            bool[] reachability = new bool[DStatesR.Count + 1];
            reachability[0] = true;
            DFSReachability(ref DStatesR, DTrans, DStates.FirstOrDefault().poses);
            int i = 1;
            foreach (var state in DStatesR)
            {
                reachability[i] = state.Value;
                i++;
            }

            return reachability;
        }

        private static void DFSReachability(ref Dictionary<string, bool> DStatesR,
            Dictionary<string, Dictionary<char, string>> DTrans,
            string currentNode)
        {
            DStatesR[currentNode] = true;
            foreach (var letter in DTrans[currentNode].Keys)
            {
                if (!DStatesR[DTrans[currentNode][letter]])
                    DFSReachability(ref DStatesR, DTrans, DTrans[currentNode][letter]);
            }
        }

        private static List<string> BTransBuilder(Dictionary<string, Dictionary<char, string>> DTrans, string pos,
            char ch)
        {
            List<string> BTrans = new List<string>();
            foreach (var key in DTrans.Keys)
            {
                if (DTrans[key][ch] == pos)
                {
                    BTrans.Add(key);
                }
            }

            return BTrans;
        }
    }
}