using System;
using System.Collections.Generic;
using System.Linq;

namespace Recognizer
{
	internal static class MedianFilterTask
	{
		/* 
		 * Для борьбы с пиксельным шумом, подобным тому, что на изображении,
		 * обычно применяют медианный фильтр, в котором цвет каждого пикселя, 
		 * заменяется на медиану всех цветов в некоторой окрестности пикселя.
		 * https://en.wikipedia.org/wiki/Median_filter
		 * 
		 * Используйте окно размером 3х3 для не граничных пикселей,
		 * Окно размером 2х2 для угловых и 3х2 или 2х3 для граничных.
		 */
		public static double[,] MedianFilter(double[,] original)
		{
            var w = original.GetLength(0);
            var h = original.GetLength(1);
            var filtered = new double[w, h];
			for (int x = 0; x < w; x++)
            {
                for (int y = 0; y < h; y++)
                {
                    filtered[x, y] = GetMedian(x, y, original, w, h);
                }
            }
			return filtered;
		}

        private static double GetMedian(int x, int y, double[,] source, int w, int h)
        {
            var left = Math.Max(x - 1, 0);
            var right = Math.Min(x + 1, w - 1);
            var top = Math.Max(y - 1, 0);
            var bottom = Math.Min(y + 1, h - 1);
            var vals = new List<double>();
            for (int i = left; i < right + 1; i++)
            {
                for (int j = top; j < bottom + 1; j++)
                {
                    vals.Add(source[i, j]);
                }
            }

            var sorted = vals.OrderBy(v => v).ToList();
            var len = sorted.Count;
            return len % 2 == 1 ? sorted[len / 2] : (sorted[len / 2 - 1] + sorted[len / 2]) / 2;
        }
	}
}