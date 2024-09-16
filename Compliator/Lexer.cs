using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Compliator
{
    class Lexer
    {
        public static List<string> ID = new List<string> { };            //1 идентификаторы
        public static List<string> CONSTANT = new List<string> { };      //2 константы 
        public static List<string> RESERVED_WORD = new List<string> { }; //3 ключевые слова
        public static List<string> LIMITER = new List<string> { };       //4 разделители
        public static string[,] TABLE = { }; // таблица токенов

        static int type = 0;
        static int counter = 0;
        static string temp = null;
        static string text;

        public static List<string> result = new List<string>{ };

        static string[] reservedWord = { "for", "or", "substr" };
        static char[] limiter = { '(', ')', '[', ']',
            '+', '-', '>', '^', '='};
        static string Read()
        {
            StreamReader sr = new StreamReader("..\\..\\..\\Program.txt", Encoding.Default);
            string AllTextProgram = sr.ReadToEnd();
            sr.Close();
            return AllTextProgram;
        }
        public static bool Check()
        {
            text = Read();
            for(int i = 0; i < text.Length; i++)
            {
                if(!Analysis(text[i], i))
                    return false;
            }
            return true;
        }
        static bool Analysis(char nextChar, int i)
        {
            int asciiCode = (int)nextChar;
            // Символ - буква (часть идентификатора)?
            if((asciiCode >= 65) && (asciiCode <= 90) || (asciiCode >= 97) 
                && (asciiCode <= 122) || (asciiCode == 46))
            {
                if (temp == null)
                {
                    if (asciiCode == 46)
                    {
                        type = 0;
                        return false;
                    }
                    type = 1;
                }
                else
                {
                    if (i + 1 < text.Length)
                    {
                        int asciiTwo = (int)text[i + 1];
                        if ((asciiCode == 46) && ((type != 1) && (temp[0] != 34) || ((asciiTwo < 65)
                            || ((asciiTwo > 90) && (asciiTwo < 97)) || (asciiTwo > 122))))
                        {
                            type = 0;
                            return false;
                        }
                    }
                    if ((type == 2) && (counter != 1)) type = 0;
                }
                temp += nextChar;
                return true;
            }
            // Символ - число (константа)?
            if((asciiCode >= 48) && (asciiCode <= 49))
            {
                if (temp == null)
                    type = 2;
                else
                {
                    if (type == 1)
                        type = 0;
                }
                temp += nextChar;
                return true;
            }
            // Символ - лимитер (является ли разделителем)?
            foreach (char c in limiter)
            {
                if (nextChar == c)
                {
                    if (temp != null)
                    {
                        if (temp[0] != 34)
                        {
                            if (!Result(temp)) return false;
                        }
                        else
                        {
                            temp += nextChar;
                            return true;
                        }
                    }
                    type = 3;
                    temp = nextChar.ToString();
                    if (!Result(temp)) return false;
                    temp = null;
                    return true;
                }
            }
            // Символ - строка (константа)?
            if ((asciiCode >= 32) && (asciiCode <= 126))
            {
                if (temp == null)
                {
                    if (asciiCode == 34)
                    {
                        type = 2;
                        counter++;
                    }
                    if (asciiCode == 32)
                        return true;
                }
                else
                {
                    if (temp[0] != 34)
                    {
                        if(asciiCode == 32)
                        {
                            if (!Result(temp)) return false;
                            temp = null;
                            return true;
                        }
                        if (type != 0)
                        {
                            type = 0;
                            return true;
                        }
                    }
                    else
                    {
                        if((asciiCode == 34) && (counter == 1))
                        {
                            temp += nextChar;
                            if (!Result(temp)) return false;
                            counter = 0;
                            temp = null;
                            return true;
                        }
                    }
                }
                temp += nextChar;
                return true;
            }
            return true;
        }
        static bool Result(string temp)
        {
            if (type == 0)
            {
                Console.WriteLine("Лексическая ошибка");
                return false;
            }
            for (int j = 0; j < reservedWord.Length; j++)
            { // проверка идентификатор это или служебное слово
                if(temp == reservedWord[j])
                {
                    for(int i = 0; i < RESERVED_WORD.Count; i++)
                    {
                        if(temp == RESERVED_WORD[i])
                        {
                            result.Add("3" + " " + i); // 3 0 \n
                            type = 0;
                            return true;
                        }
                    }
                    RESERVED_WORD.Add(temp);
                    result.Add("3" + " " + (RESERVED_WORD.Count - 1));
                    type = 0;
                    return true;
                }
            }
            switch (type)
            {
                case 1:
                    for(int j = 0; j < ID.Count; j++)
                    {
                        if(temp == ID[j])
                        {
                            result.Add("1" + " " + j );
                            type = 0;
                            return true;
                        }
                    }
                    ID.Add(temp);
                    result.Add("1" + " " + (ID.Count - 1));
                    type = 0;
                    break;
                case 2:
                    for(int j = 0; j < CONSTANT.Count; j++)
                    {
                        if(temp == CONSTANT[j])
                        {
                            result.Add("2" + " " + j);
                            type = 0;
                            return true;
                        }
                    }
                    CONSTANT.Add(temp);
                    result.Add("2" + " " + (CONSTANT.Count - 1));
                    type = 0;
                    break;
                case 3:
                    for(int j = 0; j < LIMITER.Count; j++)
                    {
                        if(temp == LIMITER[j])
                        {
                            result.Add("4" + " " + j);
                            type = 0;
                            return true;
                        }
                    }
                    LIMITER.Add(temp);
                    result.Add("4" + " " + (LIMITER.Count - 1));
                    type = 0;
                    break;
            }
            return true;
        }
    }
}