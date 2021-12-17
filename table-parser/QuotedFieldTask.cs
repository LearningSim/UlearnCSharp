using System.Text;
using NUnit.Framework;

namespace TableParser
{
    [TestFixture]
    public class QuotedFieldTaskTests
    {
        [TestCase("''", 0, "", 2)]
        [TestCase("'a'", 0, "a", 3)]
        [TestCase("'a b'", 0, "a b", 5)]
        public void ParseToken_SimpleVal(string line, int startIndex, string expectedValue, int expectedLength) =>
            ParseToken_Success(line, startIndex, expectedValue, expectedLength);

        [TestCase("'a b", 0, "a b", 4)]
        public void ParseToken_NoTrailingQuote(string line, int startIndex, string expectedValue, int expectedLength) =>
            ParseToken_Success(line, startIndex, expectedValue, expectedLength);

        [TestCase("\"a 'b' 'c' d\"", 0, "a 'b' 'c' d", 13)] // ["a 'b' 'c' d"] > [a 'b' 'c' d]
        [TestCase("'\"1\" \"2\" \"3\"'", 0, "\"1\" \"2\" \"3\"", 13)] // ['"1" "2" "3"'] > ["1" "2" "3"]
        [TestCase("'\"1\"'", 0, "\"1\"", 5)] // ['"1" "2" "3"'] > ["1" "2" "3"]
        public void ParseToken_NestedQuotes(string line, int startIndex, string expectedValue, int expectedLength) =>
            ParseToken_Success(line, startIndex, expectedValue, expectedLength);

        [TestCase(" 'a' b", 1, "a", 3)]
        public void ParseToken_HasSurroundings(string line, int startIndex, string expectedValue, int expectedLength) =>
            ParseToken_Success(line, startIndex, expectedValue, expectedLength);


        [TestCase(@"'a \''", 0, "a '", 6)] // [a \'] > [a ']
        [TestCase(@"'a \\'", 0, @"a \", 6)] // [a \\] > [a \]
        [TestCase(@"'a \\\' a'", 0, @"a \' a", 9)] // [a \\\' a] > [a \' a]
        public void ParseToken_HasEscaping(string line, int startIndex, string expectedValue, int expectedLength) =>
            ParseToken_Success(line, startIndex, expectedValue, expectedLength);

        private void ParseToken_Success(string line, int startIndex, string expectedValue, int expectedLength)
        {
            var actualToken = QuotedFieldTask.ReadQuotedField(line, startIndex);
            Assert.AreEqual(new Token(expectedValue, startIndex, expectedLength), actualToken);
        }
    }

    class QuotedFieldTask
    {
        public static Token ReadQuotedField(string line, int startIndex)
        {
            var quote = line[startIndex];
            var endIndex = 0;
            var val = new StringBuilder();
            var escaping = false;
            for (int i = startIndex + 1; i < line.Length; i++)
            {
                if ((line[i] == '\\' || line[i] == quote) && escaping)
                {
                    val.Append(line[i]);
                    escaping = false;
                }
                else if (line[i] == '\\')
                {
                    escaping = true;
                }
                else if (line[i] == quote)
                {
                    endIndex = i;
                    break;
                }
                else
                {
                    val.Append(line[i]);
                }
            }

            if (endIndex == 0)
            {
                endIndex = line.Length - 1;
            }

            var len = endIndex - startIndex + 1;
            return new Token(val.ToString(), startIndex, len);
        }
    }
}