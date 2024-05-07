using System.Linq;
using NUnit.Framework;

namespace Autocomplete;

[TestFixture]
public class Tests
{
    [TestCase(new[] { "a", "a" }, "a", 2)]
    [TestCase(new[] { "a", "b" }, "a", 1)]
    [TestCase(new[] { "b", "b" }, "a", 0)]
    [TestCase(new[] { "a", "b", "b", "b" }, "a", 1)]
    [TestCase(new[] { "ab", "b", "b", "b" }, "a", 1)]
    [TestCase(new[] { "ab", "b", "b", "b" }, "ab", 1)]
    [TestCase(new string[] { }, "a", 0)]
    [TestCase(new[] { "a", "ab", "abc" }, "aa", 1)]
    [TestCase(new[] { "ab", "ab", "ab", "ab" }, "a", 4)]
    [TestCase(new[] { "ab", "ab", "ab", "ab" }, "aa", 0)]
    public void TestRightBorder(string[] phrases, string prefix, int expectedResult)
    {
        var actual = RightBorderTask.GetRightBorderIndex(phrases, prefix, -1, phrases.Length);
        Assert.AreEqual(expectedResult, actual);
    }
}