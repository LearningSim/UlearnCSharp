using System.Collections.Generic;
using System.Linq;

namespace linq_slideviews
{
    public class StatisticsTask
    {
        public static double GetMedianTimePerSlide(List<VisitRecord> visits, SlideType slideType) =>
            visits.OrderBy(v => v.DateTime)
                .GroupBy(v => v.UserId)
                .SelectMany(gr => gr.ToList().Bigrams()
                    .Where(b => b.Item1.SlideType == slideType)
                )
                .Select(b => (b.Item2.DateTime - b.Item1.DateTime).TotalMinutes)
                .Where(t => t >= 1 && t <= 2 * 60)
                .SafeMedian();
    }

    public static class EnumerableExtensions
    {
        public static double SafeMedian(this IEnumerable<double> items)
        {
            var doubles = items.ToList();
            return doubles.Count > 0 ? doubles.Median() : 0;
        }
    }
}