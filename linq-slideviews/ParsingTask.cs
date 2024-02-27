using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace linq_slideviews;

public static class ParsingTask
{
    /// <param name="lines">все строки файла, которые нужно распарсить. Первая строка заголовочная.</param>
    /// <returns>Словарь: ключ — идентификатор слайда, значение — информация о слайде</returns>
    /// <remarks>Метод должен пропускать некорректные строки, игнорируя их</remarks>
    public static IDictionary<int, SlideRecord> ParseSlideRecords(IEnumerable<string> lines) =>
        lines.Skip(1)
            .Select(ParseSlide)
            .Where(s => s != null)
            .ToDictionary(s => s.SlideId);

    private static SlideRecord ParseSlide(string line)
    {
        var cells = line.Split(';').ToList();
        if (cells.Count != 3) return null;
        var (id, type, title) = (cells[0], cells[1], cells[2]);
        var valid = int.TryParse(id, out _) && type.ToEnum<SlideType>().HasValue;
        return valid ? new SlideRecord(int.Parse(id), type.ToEnum<SlideType>()!.Value, title) : null;
    }
    
    private static T? ToEnum<T>(this string value) where T : struct, Enum =>
        Enum.TryParse(value, true, out T result) ? result : null;

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
                UserId: int.Parse(GetUserId(l)),
                Slide: slides[int.Parse(GetSlideId(l))],
                Date: ToDateTime(GetDate(l))
            ))
            .Select(l => new VisitRecord(l.UserId, l.Slide.SlideId, l.Date.Value, l.Slide.SlideType));

    private static string GetUserId(List<string> visitLine) => visitLine[0];
    private static string GetSlideId(List<string> visitLine) => visitLine[1];
    private static string GetDate(List<string> visitLine) => $"{visitLine[2]} {visitLine[3]}";

    private static FormatException CreateException(string line) =>
        new FormatException($"Wrong line [{line}]");

    private static bool IsValid(List<string> line, IDictionary<int, SlideRecord> slides) =>
        line.Count == 4 &&
        int.TryParse(GetUserId(line), out _) &&
        int.TryParse(GetSlideId(line), out _) &&
        ToDateTime(GetDate(line)).HasValue &&
        slides.ContainsKey(int.Parse(GetSlideId(line)));

    private static DateTime? ToDateTime(string date)
    {
        const string format = "yyyy-MM-dd HH:mm:ss";
        var culture = CultureInfo.InvariantCulture;
        const DateTimeStyles style = DateTimeStyles.None;
        return DateTime.TryParseExact(date, format, culture, style, out var dt) ? dt : default(DateTime?);
    }
}