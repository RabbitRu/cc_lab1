using System.Collections.Generic;

namespace Lab1
{
    public static class Helpers
    {
        public static string ArrayToString<T>(IEnumerable<T> array)
        {
            string result = "";
            foreach (var i in array)
            {
                result += i.ToString();
            }

            return result;
        }

        public static int IndexOfHelper((string poses, bool ending)[] dStates, string state1)
        {
            for (int i = 0; i < dStates.Length; i++)
            {
                if (dStates[i].poses == state1)
                    return i;
            }

            return -1;
        }
    }
}