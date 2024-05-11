using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Autocomplete;

internal static class AutocompleteTask
{
    /// <returns>
    /// Возвращает первую фразу словаря, начинающуюся с prefix.
    /// </returns>
    /// <remarks>
    /// Эта функция уже реализована, она заработает, 
    /// как только вы выполните задачу в файле LeftBorderTask
    /// </remarks>
    public static string? FindFirstByPrefix(this IReadOnlyList<string> phrases, string prefix)
    {
        var index = LeftBorderTask.GetLeftBorderIndex(phrases, prefix, -1, phrases.Count) + 1;
        if (index < phrases.Count && phrases[index].StartsWith(prefix, StringComparison.InvariantCultureIgnoreCase))
            return phrases[index];

        return null;
    }

    /// <returns>
    /// Возвращает первые в лексикографическом порядке count (или меньше, если их меньше count) 
    /// элементов словаря, начинающихся с prefix.
    /// </returns>
    /// <remarks>Эта функция должна работать за O(log(n) + count)</remarks>
    public static string[] GetTopByPrefix(this IReadOnlyList<string> phrases, string prefix, int count)
    {
        var realCount = Math.Min(count, phrases.GetCountByPrefix(prefix));
        var left = LeftBorderTask.GetLeftBorderIndex(phrases, prefix, -1, phrases.Count);
        return phrases.GetRange(left + 1, realCount).ToArray();
    }

    private static IEnumerable<T> GetRange<T>(this IReadOnlyList<T> self, int start, int count)
    {
        for (int i = start; i < start + count; i++)
        {
            yield return self[i];
        }
    }

    /// <returns>
    /// Возвращает количество фраз, начинающихся с заданного префикса
    /// </returns>
    public static int GetCountByPrefix(this IReadOnlyList<string> phrases, string prefix)
    {
        var len = phrases.Count;
        var left = LeftBorderTask.GetLeftBorderIndex(phrases, prefix, -1, len);
        return phrases.FindFirstByPrefix(prefix) == null ? 0 :
            RightBorderTask.GetRightBorderIndex(phrases, prefix, -1, len) - left - 1;
    }
}

[TestFixture]
public class AutocompleteTests
{
    [Test]
    public void TopByPrefix_IsEmpty_WhenNoPhrases()
    {
        var actualTopWords = new List<string>().GetTopByPrefix("", 1);
        CollectionAssert.IsEmpty(actualTopWords);
    }

    [TestCase(new[] { "a", "ab", "bf" }, "a", 0, new string[] { })]
    [TestCase(new[] { "a", "ab", "bf" }, "c", 5, new string[] { })]
    [TestCase(new[] { "a", "ab", "bf" }, "", 5, new[] { "a", "ab", "bf" })]
    [TestCase(new[] { "a", "ab", "bf" }, "a", 1, new[] { "a" })]
    [TestCase(new[] { "a", "ab", "bf" }, "a", 5, new[] { "a", "ab" })]
    [TestCase(new[] { "a", "b", "bb", "cf" }, "b", 5, new[] { "b", "bb" })]
    [TestCase(new[] { "a", "b", "bb", "bc", "cf" }, "b", 2, new[] { "b", "bb" })]
    [TestCase(new[] { "a", "ab", "bf" }, "a", 5, new[] { "a", "ab" })]
    public void TopByPrefix(string[] phrases, string prefix, int count, string[] expected) =>
        CollectionAssert.AreEqual(expected, phrases.GetTopByPrefix(prefix, count));

    [TestCase(new string[] { }, "a", 0)]
    [TestCase(new[] { "a", "ab", "bf" }, "", 3)]
    [TestCase(new[] { "a", "ab", "bf" }, "a", 2)]
    [TestCase(new[] { "a", "ab", "bf" }, "c", 0)]
    [TestCase(new[] { "a", "b", "bb", "cf" }, "b", 2)]
    public void CountByPrefix(string[] phrases, string prefix, int expectedCount) =>
        Assert.AreEqual(expectedCount, phrases.GetCountByPrefix(prefix));
}