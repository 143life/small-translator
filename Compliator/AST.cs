using System;
using System.Collections.Generic;
using System.Text;

namespace Compliator
{
    class AST
    {
        public static string[] HOST = { "", "for", "init", "log", "iter", "body", "=", ">", "or", "^", "+", "-"}; // узел
        public static string[] LEAF = { "Id", "BP", "(", ")", "[", "]", "const", "substr", ",", "$" }; // лист
          
        public string function; // код слова в токенах
        public string word; // значение слова

        public int inCircleKol;
        public int outCircleKol;
        public int inQKol;
        public int outQKol;

        public List<AST> BRANCHES = new List<AST>();
        public AST topBranch; // родительский узел
        public AST thisBranch;
        public AST(string code, string w)
        {
            for(int i = 0; i < HOST.Length; i++)
            {
                if (code == HOST[i])
                    function = code;
            }
            for(int i = 0; i < LEAF.Length; i++)
            {
                if (code == LEAF[i])
                    function = code;
            }
            word = w;
            inCircleKol = 0;
            outCircleKol = 0;
            inQKol = 0;
            outQKol = 0;
            thisBranch = this;
        }
        public void TryStick(string host, AST branch) // если есть подходящий узел, прикрепить к нему данный узел
        {
            if(host == "")
            {
                for(int i = 0; i < HOST.Length; i++)
                {
                    if(function == HOST[i])
                    {
                        for (int j = 0; j < HOST.Length; j++)
                        {
                            if (word == HOST[j])
                            {
                                if (branch.word == ")")
                                {
                                    if (inCircleKol > outCircleKol)
                                    {
                                        outCircleKol++;
                                        BRANCHES.Add(branch);
                                        branch.topBranch = this;
                                        InSk(Parser.inCSkobka, Parser.outCSkobka, Parser.inQSkobka, Parser.outQSkobka);
                                        Parser.inCSkobka = 0;
                                        Parser.outCSkobka = 0;
                                        Parser.inQSkobka = 0;
                                        Parser.outQSkobka = 0;
                                        return;
                                    }
                                    else
                                    {
                                        if (topBranch != null)
                                            topBranch.TryStick(host, branch);
                                        return;
                                    }
                                }
                                if (branch.word == "]")
                                {
                                    if (inQKol > outQKol)
                                    {
                                        outQKol++;
                                        BRANCHES.Add(branch);
                                        branch.topBranch = this;
                                        return;
                                    }
                                    else
                                    {
                                        if (topBranch != null)
                                            topBranch.TryStick(host, branch);
                                        return;
                                    }
                                }
                                if (branch.word == "(")
                                {
                                    inCircleKol++;
                                    BRANCHES.Add(branch);
                                    branch.topBranch = this;
                                    InSk(Parser.inCSkobka, Parser.outCSkobka, Parser.inQSkobka, Parser.outQSkobka);
                                    Parser.inCSkobka = 0;
                                    Parser.outCSkobka = 0;
                                    Parser.inQSkobka = 0;
                                    Parser.outQSkobka = 0;
                                    return;
                                }
                                if (branch.word == "[")
                                {
                                    inQKol++;
                                    BRANCHES.Add(branch);
                                    branch.topBranch = this;
                                    return;
                                }

                                BRANCHES.Add(branch);
                                branch.topBranch = this;
                                return;
                            }
                        }
                    }
                }
                if(topBranch != null)
                    topBranch.TryStick(host, branch);
                return;
            }
            else
            {
                if (host == word || host == function)
                {
                    BRANCHES.Add(branch);
                    branch.topBranch = this;
                }
                else
                {
                    topBranch.TryStick(host, branch);
                }
            }
        }
        public void Write()
        {
            Console.WriteLine("    " + word);
            for(int i = 0; i < BRANCHES.Count; i++)
            {
                Console.Write(BRANCHES[i].word + "  ");
            }
            Console.WriteLine();
            for(int i = 0; i < BRANCHES.Count; i++)
            {
                BRANCHES[i].Write();
            }
        }
        public void InSk(int iC, int oC, int iQ, int oQ)
        {
            inCircleKol += iC;
            outCircleKol += oC;
            inQKol += iQ;
            outQKol += oQ;

        }
    }
}