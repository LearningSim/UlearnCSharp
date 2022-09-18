using System;
using System.Collections.Generic;
using System.Linq;

namespace linq_slideviews
{
	public static class ExtensionsTask
	{
		/// <summary>
		/// Медиана списка из нечетного количества элементов — это серединный элемент списка после сортировки.
		/// Медиана списка из четного количества элементов — это среднее арифметическое 
        /// двух серединных элементов списка после сортировки.
		/// </summary>
		/// <exception cref="InvalidOperationException">Если последовательность не содержит элементов</exception>
		public static double Median(this IEnumerable<double> items)
		{
			var sorted = items.OrderBy(i => i).ToList();
			if (sorted.Count == 0) throw new InvalidOperationException();
			
			var center = sorted.Count / 2;
			return sorted.Count % 2 == 1 ? sorted[center] : (sorted[center - 1] + sorted[center]) / 2;
		}

		/// <returns>
		/// Возвращает последовательность, состоящую из пар соседних элементов.
		/// Например, по последовательности {1,2,3} метод должен вернуть две пары: (1,2) и (2,3).
		/// </returns>
		public static IEnumerable<Tuple<T, T>> Bigrams<T>(this IEnumerable<T> items)
		{
			var start = true;
			T prev = default;
			foreach (var item in items)
			{
				if (!start)
				{
					yield return Tuple.Create(prev, item);
				}

				start = false;
				prev = item;
			}
		}
	}
}