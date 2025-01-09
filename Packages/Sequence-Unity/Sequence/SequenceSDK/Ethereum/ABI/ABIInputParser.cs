using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sequence.ABI
{
    public class EventDef
    {
        public string TopicHash { get; set; }  // the event topic hash, ie. 0xddf252ad1be2c89b69c2b068fc378daa952ba7f163c4a11628f55a4df523b3ef
        public string Name { get; set; }       // the event name, ie. Transfer
        public string Sig { get; set; }        // the event sig, ie. Transfer(address,address,uint256)
        public List<string> ArgTypes { get; set; } = new List<string>();   // the event arg types, ie. [address, address, uint256]
        public List<string> ArgNames { get; set; } = new List<string>();   // the event arg names, ie. [from, to, value] or ["","",""]
        public List<bool> ArgIndexed { get; set; } = new List<bool>();     // the event arg indexed flag, ie. [true, false, true]
        public int NumIndexed { get; set; }

        public override string ToString()
        {
            if (!(ArgTypes.Count == ArgIndexed.Count && ArgTypes.Count == ArgNames.Count))
            {
                return "<invalid event definition>";
            }

            var sb = new StringBuilder();
            for (int i = 0; i < ArgTypes.Count; i++)
            {
                sb.Append(ArgTypes[i]);
                if (ArgIndexed[i])
                {
                    sb.Append(" indexed");
                }
                // if (!string.IsNullOrEmpty(ArgNames[i]))
                // {
                //     sb.Append($" {ArgNames[i]}");
                // }
                if (i < ArgTypes.Count - 1)
                {
                    sb.Append(",");
                }
            }
            return $"{Name}({sb.ToString()})";
        }
    }

    public class EventParser
    {
        private static Exception ErrInvalid = new ArgumentException("Event format is invalid, expecting Method(arg1,arg2,..)");

        public static EventDef ParseEventDef(string eventString)
        {
            var eventDef = new EventDef();

            if (!eventString.Contains("(") || !eventString.Contains(")"))
            {
                throw ErrInvalid;
            }

            int openParensCount = eventString.Count(c => c == '(');
            int closeParensCount = eventString.Count(c => c == ')');
            if (openParensCount != closeParensCount || openParensCount < 1)
            {
                throw ErrInvalid;
            }

            int startIndex = eventString.IndexOf("(");
            int endIndex = eventString.LastIndexOf(")");

            string method = eventString.Substring(0, startIndex).Trim();
            eventDef.Name = method;

            string args = eventString.Substring(startIndex + 1, endIndex - startIndex - 1).Trim();

            if (string.IsNullOrEmpty(args))
            {
                eventDef.Sig = $"{method}()";
            }
            else
            {
                var tree = ParseEventArgs(args, 0);
                var (sig, typs, indexed, names) = GroupEventSelectorTree(tree, true);
                eventDef.Sig = $"{method}({sig})";
                eventDef.ArgTypes = typs;
                eventDef.ArgIndexed = indexed;
                eventDef.ArgNames = names.Select((name, index) => string.IsNullOrEmpty(name) ? $"arg{index + 1}" : name).ToList();
            }

            eventDef.NumIndexed = eventDef.ArgIndexed.Count(i => i);

            // Simulating Keccak256 hash and converting it to string.
            eventDef.TopicHash = Convert.ToBase64String(Keccak256Hash(Encoding.UTF8.GetBytes(eventDef.Sig)));

            return eventDef;
        }

        private static byte[] Keccak256Hash(byte[] input)
        {
            // Placeholder for Keccak256 hash logic. Use appropriate libraries for real implementation.
            return new byte[32];
        }

        private static EventSelectorTree ParseEventArgs(string eventArgs, int iteration)
        {
            string args = eventArgs.Trim();
            var outTree = new EventSelectorTree();

            if (string.IsNullOrEmpty(args))
            {
                return outTree;
            }

            if (!args.EndsWith(","))
            {
                args += ",";
            }

            int a = args.IndexOf("(");

            string p1 = "";
            string p2 = "";
            string p2ar = "";
            bool p2indexed = false;
            string p2name = "";
            string p3 = "";

            if (a < 0)
            {
                p1 = args;
            }
            else
            {
                p1 = args.Substring(0, a).Trim();
            }

            if (a >= 0)
            {
                int z = FindParensCloseIndex(args.Substring(a));
                z += a + 1;

                int x = args.Substring(z).IndexOf(",");
                if (x > 0)
                {
                    z += x + 1;
                }

                p2 = args.Substring(a, z - a).Trim();

                p3 = args.Substring(z).Trim();
            }

            // Parse p1 (left part)
            if (!string.IsNullOrEmpty(p1))
            {
                string[] p = p1.Split(',');
                string s = "";
                var p1indexed = new List<bool>();
                var p1names = new List<string>();

                foreach (var part1 in p)
                {
                    var arg = part1.Trim().Split(' ');

                    if (arg.Length > 3)
                    {
                        throw new Exception("Invalid event argument format");
                    }

                    if (arg.Length == 3 && arg[1] == "indexed")
                    {
                        p1indexed.Add(true);
                        p1names.Add(arg[2]);
                    }
                    else if (arg.Length == 3 && arg[1] != "indexed")
                    {
                        throw new Exception("Invalid event indexed argument format");
                    }
                    else if (arg.Length == 2 && arg[1] == "indexed")
                    {
                        p1indexed.Add(true);
                        p1names.Add("");
                    }
                    else if (arg.Length > 0 && !string.IsNullOrEmpty(arg[0]))
                    {
                        p1indexed.Add(false);
                        p1names.Add(arg.Length > 1 ? arg[1] : "");
                    }

                    string typ = arg[0].Trim();
                    if (!string.IsNullOrEmpty(typ))
                    {
                        s += typ + ",";
                    }
                }
                if (!string.IsNullOrEmpty(s))
                {
                    s = s.TrimEnd(',');
                }

                outTree.Left = s;
                outTree.Indexed = p1indexed;
                outTree.Names = p1names;
            }

            // Parse p2 (tuple)
            if (!string.IsNullOrEmpty(p2))
            {
                outTree.Tuple.Add(ParseEventArgs(p2, iteration + 1));
            }

            // Parse p3 (right part)
            if (!string.IsNullOrEmpty(p3))
            {
                outTree.Right.Add(ParseEventArgs(p3, iteration + 1));
            }

            return outTree;
        }

        private static (string, List<string>, List<bool>, List<string>) GroupEventSelectorTree(EventSelectorTree t, bool include)
        {
            var outStr = new StringBuilder();
            var typs = new List<string>();
            var indexed = new List<bool>();
            var names = new List<string>();

            if (!string.IsNullOrEmpty(t.Left))
            {
                outStr.Append(t.Left + ",");
                if (include)
                {
                    typs.AddRange(t.Left.Split(',').Where(s => !string.IsNullOrEmpty(s)));
                    indexed.AddRange(t.Indexed);
                    names.AddRange(t.Names);
                }
            }

            foreach (var child in t.Tuple)
            {
                var (s, _, _, _) = GroupEventSelectorTree(child, false);
                if (!string.IsNullOrEmpty(s))
                {
                    outStr.Append($"({s}),");
                }
            }

            foreach (var child in t.Right)
            {
                var (s, rTyps, rIndexed, rNames) = GroupEventSelectorTree(child, true);
                if (!string.IsNullOrEmpty(s))
                {
                    outStr.Append(s + ",");
                }

                typs.AddRange(rTyps);
                indexed.AddRange(rIndexed);
                names.AddRange(rNames);
            }

            return (outStr.ToString().TrimEnd(','), typs, indexed, names);
        }

        private static int FindParensCloseIndex(string args)
        {
            int n = 0;
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == '(')
                {
                    n++;
                }
                else if (args[i] == ')')
                {
                    n--;
                    if (n == 0)
                    {
                        return i;
                    }
                }
            }
            throw new Exception("Invalid function args, no closing parenthesis found");
        }
    }

    public class EventSelectorTree
    {
        public string Left { get; set; }
        public List<bool> Indexed { get; set; } = new List<bool>();
        public List<string> Names { get; set; } = new List<string>();
        public List<EventSelectorTree> Tuple { get; set; } = new List<EventSelectorTree>();
        public string TupleArray { get; set; }
        public bool TupleIndexed { get; set; }
        public string TupleName { get; set; }
        public List<EventSelectorTree> Right { get; set; } = new List<EventSelectorTree>();
    }
}