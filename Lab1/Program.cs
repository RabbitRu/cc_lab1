using System;
using System.Collections.Generic;

namespace Lab1
{
    class Program
    {
        static void Main(string[] args)
        {
            var x = "(a|b)*.a.b.b#";
            x = Console.ReadLine();
            var l = new TreeBuilder();
            Node n = l.StartParse(x);
            Console.Write("Total pos:" + l.pos + "\n");
            var aaa = TreeOps.Lastpos(n.Left);
            var bbb = TreeOps.Firstpos(n.Left);
            var ccc = TreeOps.Nullable(n);
            var followpos = TreeOps.Followpos(n, l.pos);
            var inputDictionary01 = new[] { '0', '1' };
            var DFSA = DFSABuilder.Build(n, l.pos);
            {
                //((string poses, bool ending)[] DStates, Dictionary<string, Dictionary<char, string>> DTrans) testDFSA;
                //(string poses, bool ending)[] states = new[]
                //{
                //    ("A", false),//a
                //    ("B", false),//b
                //    ("C", false),//c
                //    ("D", false),//d
                //    ("E", false),//e
                //    ("F", true),//f
                //    ("G", true),//g
                //    ("H", false)//h
                //};
                //Dictionary<string, Dictionary<char, string>> trans = new Dictionary<string, Dictionary<char, string>>
                //{
                //    {"A", new Dictionary<char, string>
                //        {
                //            {'0', "H"},
                //            {'1', "B"}
                //        }
                //    },
                //    {"B", new Dictionary<char, string>
                //        {
                //            {'0', "H"},
                //            {'1', "A"}
                //        }
                //    },
                //    {"C", new Dictionary<char, string>
                //        {
                //            {'0', "E"},
                //            {'1', "F"}
                //        }
                //    },
                //    {"D", new Dictionary<char, string>
                //        {
                //            {'0', "E"},
                //            {'1', "F"}
                //        }
                //    },
                //    {"E", new Dictionary<char, string>
                //        {
                //            {'0', "F"},
                //            {'1', "G"}
                //        }
                //    },
                //    {"F", new Dictionary<char, string>
                //        {
                //            {'0', "F"},
                //            {'1', "F"}
                //        }
                //    },
                //    {"G", new Dictionary<char, string>
                //        {
                //            {'0', "G"},
                //            {'1', "F"}
                //        }
                //    },
                //    {"H", new Dictionary<char, string>
                //        {
                //            {'0', "C"},
                //            {'1', "C"}
                //        }
                //    }
                //};
                //testDFSA.DStates = states;
                //testDFSA.DTrans = trans;
            }
            var minDfsa =DFSAMinimizer.Minimize(DFSA.States, DFSA.Trans, DFSA.InputDict);
            var model = new DFSAModel(minDfsa.DStates,minDfsa.DTrans);
            string consoleInput = "";
            while (consoleInput!="end")
            {
                consoleInput = Console.ReadLine();
                var answer = model.Model(consoleInput);
                Console.WriteLine(answer + "\n");

            }
            Console.Write("That's All Folks");
        }
    }
}
