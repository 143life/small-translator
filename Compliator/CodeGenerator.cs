using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Compliator
{
    class CodeGenerator
    { 
        static string res = "";

        //{ "", "for", "init", "log", "iter", "body", "=", ">", "or", "^", "+", "-"}; // узел
        //{ "Id", "BP", "(", ")", "[", "]", "const", "substr", ",", "$" };

        public static void Generate(AST table, string[] host, string[] leaf)
        {
            StreamWriter result = new StreamWriter
                (@"..\\..\\..\\Result.txt");
            result.WriteLine();
            table.Write();
            res += table.word + "(";
            foreach(AST next in table.BRANCHES)
            {
                if(next.function == "init")
                {
                    recInit(next);
                }
            }
            res += "; ";
            foreach (AST next in table.BRANCHES)
            {
                if (next.function == "log")
                {
                    recInit(next);
                }
            }
            res += "; ";
            foreach (AST next in table.BRANCHES)
            {
                if (next.function == "iter")
                {
                    recInit(next);
                }
            }
            res += ")\n";
            res += "{";
            res += "    ";
            foreach (AST next in table.BRANCHES)
            {
                if (next.function == "body")
                {
                    recInit(next);
                }
            }
            res +=";\n}";
            res = res.Replace(" ^ ", "^");
            res = res.Replace(" or ", " || ");
            result.WriteLine(res);
            result.Close();
        }
        static void recInit(AST next)
        {
            int id = 1;
            int lograv = 0;
            bool cIsClose = true;
            bool qIsClose = true;
            for (int ip = 0; ip < next.BRANCHES.Count; ip++)
            {
                AST n = next.BRANCHES[ip];
                switch (n.function)
                {
                    case "Id":
                        res += n.word;
                        break;
                    case "BP":
                        res += n.word;
                        break;
                    case "(":
                        cIsClose = false;
                        res += " " + n.word;
                        break;
                    case ")":
                        cIsClose = true;
                        res += n.word;
                        if (lograv == 0)
                        {
                            if (n.topBranch.word == ">" || n.topBranch.word == "or"
                            || n.topBranch.word == "^" || n.topBranch.word == "=")
                            {
                                res += " " + n.topBranch.word ;
                                lograv++;
                                break;
                            }
                        }
                        if (ip < next.BRANCHES.Count &&
                                (n.topBranch.word == "+" || n.topBranch.word == "-"))
                        {
                            res += " " + n.topBranch.word;
                            lograv++;
                        }
                        break;
                    case "[":
                        qIsClose = false;
                        res += n.word;
                        break;
                    case "]":
                        qIsClose = true;
                        res += n.word;
                        if (lograv == 0)
                        {
                            if (n.topBranch.word == ">" || n.topBranch.word == "or"
                            || n.topBranch.word == "^" || n.topBranch.word == "=")
                            {
                                res += " " + n.topBranch.word;
                                lograv++;
                                break;
                            }
                        }
                        if (ip < next.BRANCHES.Count &&
                                (n.topBranch.word == "+" || n.topBranch.word == "-"))
                        {
                            res += " " + n.topBranch.word ;
                            lograv++;
                        }
                        break;
                    case "const":
                        res += BinaryToDecimal(n.word);
                        break;
                    case "substr":
                        res += n.word;
                        break;
                    case ",":
                        res += n.word;
                        break;
                    case "$":
                        return;
                    default:
                        recInit(n);
                        break;
                }
                if (cIsClose && qIsClose)
                {
                    if (ip < next.BRANCHES.Count - 1)
                    {
                        if(next.BRANCHES[ip + 1].word != "[")
                        {
                            if (lograv == 0)
                            {
                                res += " " + next.word + " ";
                                lograv++;
                            }
                            else
                            {
                                if((n.topBranch.word == "+" || n.topBranch.word == "-") 
                                    && n.word != "]" && n.word != ")")
                                    res += " " + next.word + " ";
                                lograv++;
                            }
                        }
                    }
                }
            }
        }

        static uint BinaryToDecimal(string binaryNumber)
        {
            var exponent = 0;
            var result = 0u;
            for (var i = binaryNumber.Length - 1; i >= 0; i--)
            {
                if (binaryNumber[i] == '1')
                {
                    result += Convert.ToUInt32(Math.Pow(2, exponent));
                }
                exponent++;
            }

            return result;
        }
    }
}