using System;
using System.Collections.Generic;
using System.Linq;

namespace Autocomplete;

public static class RightBorderTask
{
    /// <returns>
    /// Возвращает индекс правой границы. 
    /// То есть индекс минимального элемента, который не начинается с prefix и большего prefix.
    /// Если такого нет, то возвращает items.Length
    /// </returns>
    /// <remarks>
    /// Функция должна быть НЕ рекурсивной
    /// и работать за O(log(items.Length)*L), где L — ограничение сверху на длину фразы
    /// </remarks>
    public static int GetRightBorderIndex(IReadOnlyList<string> phrases, string prefix, int left, int right)
    {
        while (true)
        {
            if (left == right - 1) return right;
            var m = left + (right - left) / 2;
            const StringComparison comparison = StringComparison.InvariantCultureIgnoreCase;
            var prefixLessMiddle = string.Compare(prefix, phrases[m], comparison) < 0 &&
                                   !phrases[m].StartsWith(prefix, StringComparison.InvariantCultureIgnoreCase);
            if (prefixLessMiddle)
            {
                right = m;
            }
            else
            {
                left = m;
            }
        }
    }
}