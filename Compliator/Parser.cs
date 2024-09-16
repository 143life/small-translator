using System;
using System.Collections.Generic;
using System.Text;

namespace Compliator
{
    class Parser
    {
        public static AST aTable; // Abstract Tree Абстрактное синтаксическое дерево - итог анализа

        static bool log = false;   // 4 части цикла for: условие - есть - true; нет - false
        static bool init = false;  // инициализация
        static bool iter = false;  // итерация
        static bool body = false;  // тело
        struct host
        {
            public AST topBranch; // родительский узел
            public host(AST t)
            {
                topBranch = t;
            }

        }
        static host preLastHost;
        static AST nowHost;
        static List<AST> logs;

        public static int inCSkobka = 0;
        public static int outCSkobka = 0;
        public static int inQSkobka = 0;
        public static int outQSkobka = 0;

        static string[] nonTerminalSymbols = { "S\'", "S\'\'", "S\'\'\'", "S\'\'\'\'", "rav", "rav\'",
            "oper", "oper\'", "oper\'\'", "oper*", "H", "H\'", "G", "M", "F", "D"}; 
        // arifm = +\-, log = >\or\^
        static string[] terminalWords = { "for", "log", "(", "=", "BP", "const", "substr", "Id", "[", "]", "arifm", ")", ",", "$" };

        static string[,] M =
            {
            { "S\'\'for", "S\'\'foroper\'D", "S\'\'for)oper\'D(", "1", "1", "1", "1", "1", "1", "1", "1", "1", "1", "" },         // S'
            { "1", "1", "1", "S\'\'\'rav", "1", "1", "1", "1", "1", "1", "1", "1", "1", "" },                                     // S''
            { "1", "1", "1", "S\'\'\'\'rav", "1", "1", "1", "1", "1", "1", "1", "1", "1", "" },                                   // S'''
            { "1", "1", "1", "rav", "1", "1", "1", "1", "1", "1", "1", "1", "1", "" },                                            // S''''
            { "1", "1", "1", "rav\'=", "1", "1", "1", "1", "1", "1", "1", "1", "1", "1" },                                        // rav
            { "1", "1", "1", "1", "operBP", "1", "1", "operH\'Id", "1", "1", "1", "1", "1", "1" },                                // rav'
            { "1", "oper\'D", ")oper(", "1", "BP", "const", ")M,M(substr", "H\'Id", "1", "1", "oper\'\'F", "1", "1", "1" },       // oper
            { "1", "1", "oper)oper(", "1", "HH", "HH", "HH", "HH", "1", "1", "1", "1", "1", "1" },                                // oper'
            { "1", "oper\'D", "oper*)oper(", "1", "oper*H", "oper*H", "oper*H", "oper*H", "1", "1", "oper\'\'F", "1", "1", "1" }, // oper''
            { "", "1", "1", "", "oper*H", "oper*H", "oper*H", "oper*H", "1", "", "1", "", "1", "" },                              // oper*
            { "1", "1", "1", "1", "BP", "const", ")M,M(substr", "H\'Id", "1", "1", "1", "1", "1", "1" },                          // H
            { "", "", "", "", "", "", "", "", "]oper[", "", "", "", "", "" },                                                     // H' 
            { "1", "1", "1", "1", "BP", "1", "1", "H\'Id", "1", "1", "1", "1", "1", "1" },                                        // G
            { "1", "1", "1", "1", "1", "1", "1", "H\'Id", "1", "1", "1", "1", "1", "1" },                                         // M
            { "1", "1", "1", "1", "1", "1", "1", "1", "1", "1", "arifm", "1", "1", "1" },                                         // F
            { "1", "log", "1", "1", "1", "1", "1", "1", "1", "1", "1", "1", "1", "1" }                                            // D
            };

        static void Error()
        {
            Console.WriteLine("ERROR: Синтаксическая ошибка!");
        }

        public static bool Check(List<string> tokens, List<string> ID, List<string> CONSTANT, List<string> RESERVED_WORD, List<string> LIMITER)
        {
            int ip = 0; // указатель [ip] на начало входного буфера
            string stack = "$S\'"; // [stack]
            Console.WriteLine("----------------------------------------------------------");
            for(; ip < tokens.Count; )
            {
                int isTerm = 0; // 0 - если терминал, 1 - нетерминал (речь об [X])
                string X = null; // символ на вершине стека [stack]
                for (int i = 0; i < nonTerminalSymbols.Length; i++)
                {
                    if(stack.EndsWith(nonTerminalSymbols[i]))
                    {
                        X = nonTerminalSymbols[i];
                        isTerm = 1;
                    }
                }                // если [X] - нетерминал, присваиваем нетерминал
                if (isTerm == 0) // иначе присваиваем терминал
                {
                    for (int i = 0; i < terminalWords.Length; i++)
                    {
                        if (stack.EndsWith(terminalWords[i]))
                        {
                            X = terminalWords[i];
                        }
                    }
                }
                string a = tokens[ip]; // символ, на который указывает [ip] (токен)
                int ind;
                string A = null;
                string index = "";
                for (int i = 2; i < a.Length; i++)
                {
                    index += a[i];
                }
                int.TryParse(index, out ind);
                switch (a[0] - 48)
                {
                    case 1: // ID
                        A = ID[ind];
                        break;
                    case 2: // CONSTANT
                        A = CONSTANT[ind];
                        break;
                    case 3: // RESERVED_WORD
                        A = RESERVED_WORD[ind];
                        break;
                    case 4: // LIMITER
                        A = LIMITER[ind];
                        break;
                }
                if (isTerm == 0) // если [X] - терминал или $ then
                {
                    switch (X)
                    {
                        case ("log"):
                            if (A == ">" || A == "or" || A == "^")
                            {
                                stack = stack.Substring(0, stack.Length - X.Length);
                                ip++;
                                Console.WriteLine(stack);
                                if(log)
                                {
                                    preLastHost = new host(nowHost.thisBranch);
                                    nowHost = new AST(A, A);
                                    preLastHost.topBranch.TryStick("", nowHost);
                                    if(nowHost.topBranch == null)
                                    {
                                        if (logs == null)
                                        {
                                            logs = new List<AST>();
                                        }
                                        logs.Add(nowHost);
                                    }
                                }
                                else
                                {
                                    logs = new List<AST>();
                                    nowHost = new AST("log", A);
                                    logs.Add(nowHost);
                                    log = true;
                                }
                            }
                            else
                            {
                                Error();
                                return false;
                            }
                            break;
                        case ("BP"):
                            if (A.IndexOf(".") != -1 && a[0] - 48 == 1)
                            {
                                stack = stack.Substring(0, stack.Length - X.Length);
                                ip++;
                                Console.WriteLine(stack);
                                preLastHost = new host(nowHost.thisBranch);
                                nowHost = new AST("Id", A);
                                preLastHost.topBranch.TryStick("", nowHost);
                            }
                            else
                            {
                                Error();
                                return false;
                            }
                            break;
                        case ("const"):
                            if (a[0] - 48 == 2 && A[0] != '\"')
                            {
                                stack = stack.Substring(0, stack.Length - X.Length);
                                ip++;
                                Console.WriteLine(stack);
                                preLastHost = new host(nowHost.thisBranch);
                                nowHost = new AST("const", A);
                                preLastHost.topBranch.TryStick("", nowHost);
                            }
                            else
                            {
                                Error();
                                return false;
                            }
                            break;
                        case ("Id"):
                            if (A.IndexOf(".") == -1 && a[0] - 48 == 1)
                            {
                                stack = stack.Substring(0, stack.Length - X.Length);
                                ip++;
                                Console.WriteLine(stack);
                                preLastHost = new host(nowHost.thisBranch);
                                nowHost = new AST("Id", A);
                                preLastHost.topBranch.TryStick("", nowHost);

                            }
                            else
                            {
                                Error();
                                return false;
                            }
                            break;
                        case ("arifm"):
                            if (A == "-" || A == "+")
                            {
                                stack = stack.Substring(0, stack.Length - X.Length);
                                ip++;
                                Console.WriteLine(stack);
                                preLastHost = new host(nowHost.thisBranch);
                                nowHost = new AST(A, A);
                                preLastHost.topBranch.TryStick("", nowHost);
                            }
                            else
                            {
                                Error();
                                return false;
                            }
                            break;
                        case ("for"):
                            if (A == "for")
                            {
                                stack = stack.Substring(0, stack.Length - X.Length);
                                ip++;

                                Console.WriteLine(stack);
                                nowHost = new AST(A, A);
                                for(int l = 0; l < logs.Count; l++)
                                {
                                    nowHost.TryStick(A, logs[l]);
                                }
                                aTable = nowHost;
                                preLastHost = new host(nowHost.thisBranch);
                            }
                            else
                            {
                                Error();
                                return false;
                            }
                            break;
                        case ("("):
                            if (A == "(")
                            {
                                stack = stack.Substring(0, stack.Length - X.Length);
                                ip++;
                                Console.WriteLine(stack);
                                if (log)
                                {
                                    preLastHost = new host(nowHost.thisBranch);
                                    nowHost = new AST(A, A);
                                    preLastHost.topBranch.TryStick("", nowHost);
                                }
                                else
                                {
                                    inCSkobka++;
                                    if(logs == null)
                                        logs = new List<AST>();
                                    nowHost = new AST("log", A);
                                    logs.Add(nowHost);
                                    log = true;
                                }
                            }
                            else
                            {
                                Error();
                                return false;
                            }
                            break;
                        case ("="):
                            if (A == "=")
                            {
                                stack = stack.Substring(0, stack.Length - X.Length);
                                ip++;
                                Console.WriteLine(stack);
                                if (init)
                                {
                                    if (iter)
                                    {
                                        if (body)
                                        {
                                            preLastHost = new host(nowHost.thisBranch);
                                            nowHost = new AST(A, A);
                                            preLastHost.topBranch.TryStick("", nowHost);
                                            
                                            break;
                                        }
                                        else
                                        {
                                            preLastHost = new host(nowHost.thisBranch);
                                            nowHost = new AST("body", A);
                                            preLastHost.topBranch.TryStick("for", nowHost);
                                            body = true;
                                            
                                        }
                                    }
                                    else
                                    {
                                        preLastHost = new host(nowHost.thisBranch);
                                        nowHost = new AST("iter", A);
                                        preLastHost.topBranch.TryStick("for", nowHost);
                                        iter = true;
                                    }
                                    
                                }
                                else
                                {
                                    preLastHost = new host(nowHost.thisBranch);
                                    nowHost = new AST("init", A);
                                    preLastHost.topBranch.TryStick("for", nowHost);
                                    init = true;
                                    
                                    break;
                                }
                            }
                            else
                            {
                                Error();
                                return false;
                            }
                            break;
                        case ("substr"):
                            if (A == "substr")
                            {
                                stack = stack.Substring(0, stack.Length - X.Length);
                                ip++;
                                Console.WriteLine(stack);
                                preLastHost = new host(nowHost.thisBranch);
                                nowHost = new AST(A, A);
                                preLastHost.topBranch.TryStick("", nowHost);
                            }
                            else
                            {
                                Error();
                                return false;
                            }
                            break;
                        case ("["):
                            if (A == "[")
                            {
                                stack = stack.Substring(0, stack.Length - X.Length);
                                ip++;
                                Console.WriteLine(stack);
                                preLastHost = new host(nowHost.thisBranch);
                                nowHost = new AST(A, A);
                                preLastHost.topBranch.TryStick("", nowHost);
                            }
                            else
                            {
                                Error();
                                return false;
                            }
                            break;
                        case ("]"):
                            if (A == "]")
                            {
                                stack = stack.Substring(0, stack.Length - X.Length);
                                ip++;
                                Console.WriteLine(stack);
                                preLastHost = new host(nowHost.thisBranch);
                                nowHost = new AST(A, A);
                                preLastHost.topBranch.TryStick("", nowHost);
                            }
                            else
                            {
                                Error();
                                return false;
                            }
                            break;
                        case (")"):
                            if (A == ")")
                            {
                                stack = stack.Substring(0, stack.Length - X.Length);
                                ip++;
                                Console.WriteLine(stack);
                                preLastHost = new host(nowHost.thisBranch);
                                
                                nowHost = new AST(A, A);
                                if(log)
                                {
                                    if (aTable == null)
                                    {
                                        if (inCSkobka > outCSkobka)
                                        {
                                            logs.Add(nowHost);
                                            outCSkobka++;
                                        }
                                    }
                                }
                                preLastHost.topBranch.TryStick("", nowHost);
                            }
                            else
                            {
                                Error();
                                return false;
                            }
                            break;
                        case (","):
                            if (A == ",")
                            {
                                stack = stack.Substring(0, stack.Length - X.Length);
                                ip++;
                                Console.WriteLine(stack);
                                preLastHost = new host(nowHost.thisBranch);
                                nowHost = new AST(A, A);
                                preLastHost.topBranch.TryStick("", nowHost);
                            }
                            else
                            {
                                Error();
                                return false;
                            }
                            break;
                        case ("$"):
                            if (A == "$")
                            {
                                stack = stack.Substring(0, stack.Length - X.Length);
                                ip++;
                                Console.WriteLine(stack);
                                preLastHost = new host(nowHost.thisBranch);
                                nowHost = new AST(A, A);
                                preLastHost.topBranch.TryStick("", nowHost);
                            }
                            else
                            {
                                Error();
                                return false;
                            }
                            break;
                    }

                }
                else // [X] - НЕТЕРМИНАЛ, [terminalWords], [M], [A], [nonTerminalSymbols]
                { // индекс [A] в [terminalWords] - столбец таблицы [M]; индекс [X] в [nonTerminalSymbols] - строка таблицы [M]
                    int stolbec = -1;
                    int stroka = -1;
                    for (int i = 0; i < terminalWords.Length; i++)
                    {
                        if (A == terminalWords[i] || (terminalWords[i] == "Id" && A.IndexOf(".") == -1 && a[0] - 48 == 1)
                            || (terminalWords[i] == "const" && a[0] - 48 == 2 && A[0] != '\"')
                            || (terminalWords[i] == "BP" && (A.IndexOf(".") != -1 && a[0] - 48 == 1)) 
                            || ((terminalWords[i] == "log" && (A == "or" || A == "^" || A == ">"))
                            || (terminalWords[i] == "arifm" && (A == "+" || A == "-"))))
                        {
                            stolbec = i;
                            break;
                        }
                    }
                    for (int i = 0; i < nonTerminalSymbols.Length; i++)
                    {
                        if (X == nonTerminalSymbols[i])
                        {
                            stroka = i;
                            break;
                        }
                    }
                    if (M[stroka, stolbec] == "1")
                    {
                        Error();
                        return false;
                    }
                    else
                    {
                        stack = stack.Substring(0, stack.Length - X.Length);
                        
                        stack += M[stroka, stolbec];
                        Console.WriteLine(stack);
                        if (A == "$")
                            break;
                    }
                }
                inCSkobka = 0;
                outCSkobka = 0;
                inQSkobka = 0;
                outQSkobka = 0;
            }
            return true;
        }
    }
}
