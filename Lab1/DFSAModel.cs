using System.Collections.Generic;
using System.Linq;

namespace Lab1
{
    public class DFSAModel
    {
        public (string poses, bool ending)[] DStates { get; set; }
        public Dictionary<string, Dictionary<char, string>> DTrans { get; set; }

        public DFSAModel((string poses, bool ending)[] states, Dictionary<string, Dictionary<char, string>> trans)
        {
            DStates = states;
            DTrans = trans;
        }

        public bool Model(string input)
        {
            var currentState = DStates.First().poses;
            for (int i = 0; i < input.Length; i++)
            {
                currentState = DTrans[currentState][input[i]];
            }

            return DStates[Helpers.IndexOfHelper(DStates, currentState)].ending;

        }
    }
}