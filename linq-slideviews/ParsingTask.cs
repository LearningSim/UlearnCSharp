using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace linq_slideviews
{
    public class ParsingTask
    {
        /// <param name="lines">все строки файла, которые нужно распарсить. Первая строка заголовочная.</param>
        /// <returns>Словарь: ключ — идентификатор слайда, значение — информация о слайде</returns>
        /// <remarks>Метод должен пропускать некорректные строки, игнорируя их</remarks>
        public static IDictionary<int, SlideRecord> ParseSlideRecords(IEnumerable<string> lines) =>
            lines.Skip(1)
                .Select(l => l.Split(';').ToList())
                .Where(l => l.Count == 3 && int.TryParse(l[0], out _) && l[1].ToEnum<SlideType>().HasValue)
                .Select(l => new SlideRecord(int.Parse(l[0]), l[1].ToEnum<SlideType>().Value, l[2]))
                .ToDictionary(s => s.SlideId);

        /// <param name="lines">все строки файла, которые нужно распарсить. Первая строка — заголовочная.</param>
        /// <param name="slides">Словарь информации о слайдах по идентификатору слайда. 
        /// Такой словарь можно получить методом ParseSlideRecords</param>
        /// <returns>Список информации о посещениях</returns>
        /// <exception cref="FormatException">Если среди строк есть некорректные</exception>
        public static IEnumerable<VisitRecord> ParseVisitRecords(
            IEnumerable<string> lines, IDictionary<int, SlideRecord> slides) =>
            lines.Skip(1)
                .Select(l => (Split: l.Split(';').ToList(), Orig: l))
                .Select(l => IsValid(l.Split, slides) ? l.Split : throw CreateException(l.Orig))
                .Select(l => (
                    UserId: int.Parse(UserId(l)),
                    Slide: slides[int.Parse(SlideId(l))],
                    Date: ToDateTime(Date(l))
                ))
                .Select(l => new VisitRecord(l.UserId, l.Slide.SlideId, l.Date.Value, l.Slide.SlideType));

        private static string UserId(List<string> visitLine) => visitLine[0];
        private static string SlideId(List<string> visitLine) => visitLine[1];
        private static string Date(List<string> visitLine) => $"{visitLine[2]} {visitLine[3]}";

        private static FormatException CreateException(string line) =>
            new FormatException($"Wrong line [{line}]");

        private static bool IsValid(List<string> line, IDictionary<int, SlideRecord> slides) =>
            line.Count == 4 &&
            int.TryParse(UserId(line), out _) &&
            int.TryParse(SlideId(line), out _) &&
            ToDateTime(Date(line)).HasValue &&
            slides.ContainsKey(int.Parse(SlideId(line)));

        private static DateTime? ToDateTime(string date)
        {
            const string format = "yyyy-MM-dd HH:mm:ss";
            var culture = CultureInfo.InvariantCulture;
            const DateTimeStyles style = DateTimeStyles.None;
            return DateTime.TryParseExact(date, format, culture, style, out var dt) ? dt : default(DateTime?);
        }
    }

    public static class EnumExtensions
    {
        public static T? ToEnum<T>(this string value) where T : struct, IConvertible =>
            Enum.TryParse(value, true, out T result) ? result : default(T?);
    }
}