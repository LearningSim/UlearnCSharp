namespace DijkstraWithPriorityQueue.Graphs;

public static class Extensions
{
    public static string ToPrettyString<T>(this IEnumerable<T> source) =>
        $"[{string.Join(", ", source)}]";
}