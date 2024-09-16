using System;

namespace Compliator
{
    class Program
    {
        static void Main(string[] args)
        {
            if(Lexer.Check())// лексический анализ
            {
                for (int i = 0; i < Lexer.result.Count; i++)
                {
                    Console.WriteLine(Lexer.result[i]);
                }
                if(Parser.Check(Lexer.result, Lexer.ID, Lexer.CONSTANT, Lexer.RESERVED_WORD, Lexer.LIMITER)) // синтаксический анализ
                {
                    CodeGenerator.Generate(Parser.aTable, AST.HOST, AST.LEAF); // генерация кода
                }
            }
        }
    }
}
