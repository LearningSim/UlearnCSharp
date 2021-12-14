using System;

namespace Recognizer
{
    internal static class SobelFilterTask
    {
        public static double[,] SobelFilter(double[,] image, double[,] win)
        {
            var width = image.GetLength(0);
            var height = image.GetLength(1);
            var result = new double[width, height];
            var offset = win.GetLength(0) / 2;
            for (int x = offset; x < width - offset; x++)
            {
                for (int y = offset; y < height - offset; y++)
                {
                    result[x, y] = FilterPixel(x, y, image, win);
                }
            }

            return result;
        }

        private static double FilterPixel(int x, int y, double[,] source, double[,] win)
        {
            var winSize = win.GetLength(0);
            var offset = winSize / 2;
            double resX = 0;
            double resY = 0;
            for (int i = 0; i < winSize; i++)
            {
                for (int j = 0; j < winSize; j++)
                {
                    resX += win[i, j] * source[x + i - offset, y + j - offset];
                    resY += win[j, i] * source[x + i - offset, y + j - offset];
                }
            }

            return Math.Sqrt(resX * resX + resY * resY);
        }
    }
}