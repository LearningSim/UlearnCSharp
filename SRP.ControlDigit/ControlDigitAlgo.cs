using System.Collections.Generic;
using System.Linq;

namespace SRP.ControlDigit
{
    public static class Extensions
    {
        // Вспомогательные методы-расширения поместите в этот класс.
        // Они должны быть понятны и потенциально полезны вне контекста задачи расчета контрольных разрядов.
        public static IEnumerable<(T value, int index)> Enumerate<T>(this IEnumerable<T> source) =>
            source.Select((value, i) => (value, i));

        public static IEnumerable<int> SplitReversed(this long number)
        {
            do
            {
                yield return (int)(number % 10);
                number /= 10;
            } while (number > 0);
        }

        public static int GetComplementToMultiple(this int number, int factor) =>
            (factor - number % factor) % factor;

        public static int ToChar(this int digit) => digit + '0';
        public static int SumDigits(this int number) => ((long)number).SplitReversed().Sum();
    }

    public static class ControlDigitAlgo
    {
        public static int Upc(long number)
        {
            var digits = number.SplitReversed().ToList();
            var oddSum = digits.Where((d, i) => (i + 1) % 2 == 1).Sum();
            var evenSum = digits.Where((d, i) => (i + 1) % 2 == 0).Sum();
            var sum = oddSum * 3 + evenSum;
            return sum.GetComplementToMultiple(10);
        }

        public static int Isbn10(long number)
        {
            var sum = 0;
            foreach (var (digit, i) in number.SplitReversed().Enumerate())
            {
                var numberPosition = i + 2;
                sum += digit * numberPosition;
            }

            var result = sum.GetComplementToMultiple(11);
            return result == 10 ? 'X' : result.ToChar();
        }

        public static int Luhn(long number)
        {
            var sum = number.SplitReversed()
                .Select(CalculateLuhnAddend)
                .Sum();

            return sum.GetComplementToMultiple(10);
        }

        private static int CalculateLuhnAddend(int digit, int digitIndex)
        {
            var addend = digit;
            if (digitIndex % 2 == 0)
            {
                addend *= 2;
            }

            return addend.SumDigits();
        }
    }
}