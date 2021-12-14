using System.Linq;

namespace Recognizer
{
	public static class ThresholdFilterTask
	{
		public static double[,] ThresholdFilter(double[,] original, double whitePixelsFraction)
        {
            var flattened = original.Cast<double>().OrderByDescending(n => n).ToList();
            var whiteNumber = (int) (flattened.Count * whitePixelsFraction);
            var threshold = whiteNumber == 0 ? double.PositiveInfinity : flattened[whiteNumber - 1];
            var w = original.GetLength(0);
            var h = original.GetLength(1);
            var filtered = new double[w, h];
            for (int x = 0; x < w; x++)
            {
                for (int y = 0; y < h; y++)
                {
                    filtered[x, y] = original[x, y] >= threshold ? 1 : 0;
                }
            }
            return filtered;
        }
	}
}