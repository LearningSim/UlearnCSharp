using System;
using System.Collections.Generic;
using System.Linq;

namespace Passwords
{
    public class CaseAlternatorTask
    {
        //Тесты будут вызывать этот метод
        public static List<string> AlternateCharCases(string lowercaseWord)
        {
            var result = new List<string>();
            AlternateCharCases(lowercaseWord.ToCharArray(), 0, result);
            return result;
        }

        static void AlternateCharCases(char[] word, int startIndex, List<string> result, bool ignore = false)
        {
            if (!ignore)
            {
                result.Add(new string(word));
            }

            if (startIndex == word.Length)
            {
                return;
            }

            AlternateCharCases(word, startIndex + 1, result, true);

            // добавляем в результат только измененное слово
            if (IsLetter(word[startIndex]))
            {
                var modified = word.ToArray();
                modified[startIndex] = char.ToUpper(modified[startIndex]);
                AlternateCharCases(modified, startIndex + 1, result);
            }
        }

        private static bool IsLetter(char symbol)
        {
            return char.IsLetter(symbol) && symbol != char.ToUpper(symbol);
        }
    }
}