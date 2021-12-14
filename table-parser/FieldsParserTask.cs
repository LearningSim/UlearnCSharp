using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace TableParser
{
    [TestFixture]
    public class FieldParserTaskTests
    {
        [TestCase("text", new[] {"text"})]
        [TestCase("text ", new[] {"text"})]
        [TestCase("hello world", new[] {"hello", "world"})]
        [TestCase("hello  world", new[] {"hello", "world"})]
        [TestCase("''", new[] {""})]
        [TestCase("'a d'", new[] {"a d"})]
        [TestCase("'a", new[] {"a"})]
        [TestCase("\"a 'b'\"", new[] {"a 'b'"})]
        [TestCase("'\"1\"'", new[] {"\"1\""})]
        [TestCase(" 'a' b", new[] {"a", "b"})]
        [TestCase("v 'a'", new[] {"v", "a"})]
        [TestCase("v'a'", new[] {"v", "a"})]
        [TestCase(@"'a \''", new[] {"a '"})]
        [TestCase(@"'a \\'", new[] {@"a \"})]
        [TestCase("\" \\\" \"", new[] {" \" "})]
        [TestCase("", new string[0])]
        [TestCase("' ", new[] {" "})]
        public static void RunTests(string input, string[] expectedOutput)
        {
            Test(input, expectedOutput);
        }

        public static void Test(string input, string[] expectedResult)
        {
            var actualResult = FieldsParserTask.ParseLine(input);
            Assert.AreEqual(expectedResult.Length, actualResult.Count);
            for (int i = 0; i < expectedResult.Length; ++i)
            {
                Assert.AreEqual(expectedResult[i], actualResult[i].Value);
            }
        }
    }

    public class FieldsParserTask
    {
        public static List<Token> ParseLine(string line)
        {
            var tokens = new List<Token>();
            for (int i = 0; i < line.Length; i++)
            {
                if (line[i] == ' ')
                {
                    continue;
                }

                var token = ReadToken(line, i);
                tokens.Add(token);
                i += token.Length - 1;
            }

            return tokens;
        }

        private static Token ReadToken(string line, int startIndex)
        {
            if (line[startIndex] == '"' || line[startIndex] == '\'')
            {
                return ReadQuotedField(line, startIndex);
            }

            return ReadField(line, startIndex);
        }

        private static Token ReadField(string line, int startIndex)
        {
            var len = line.Length - startIndex;
            for (int i = startIndex + 1; i < line.Length; i++)
            {
                if (new[] {' ', '\'', '"'}.Contains(line[i]))
                {
                    len = i - startIndex;
                    break;
                }
            }

            return new Token(line.Substring(startIndex, len), startIndex, len);
        }

        public static Token ReadQuotedField(string line, int startIndex)
        {
            return QuotedFieldTask.ReadQuotedField(line, startIndex);
        }
    }
}