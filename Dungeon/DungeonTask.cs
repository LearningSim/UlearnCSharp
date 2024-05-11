using System;
using System.Collections.Generic;
using System.Linq;

namespace Dungeon;

public static class DungeonTask
{
    public static MoveDirection[] FindShortestPath(Map map)
    {
        var pathsFromPlayer = BfsTask.FindPaths(map, map.InitialPosition, map.Chests).ToArray();
        if (pathsFromPlayer.Length == 0)
        {
            var start = new[] { new Chest(map.InitialPosition, 0) };
            return BfsTask.FindPaths(map, map.Exit, start)
                       .FirstOrDefault()
                       ?.ToMoveDirections()
                   ?? Array.Empty<MoveDirection>();
        }

        var pathsFromExit = BfsTask.FindPaths(map, map.Exit, map.Chests);
        var chests = map.Chests.ToDictionary(c => c.Location, c => c);
        var pathHalves = pathsFromPlayer.Join(pathsFromExit,
                p => p.Value,
                p => p.Value,
                (a, b) => (First: a, Second: b)
            )
            .OrderBy(ps => ps.First.Length + ps.Second.Length)
            .ThenByDescending(ps => chests[ps.First.Value].Value)
            .FirstOrDefault();
        var path = pathHalves.First?.Reverse().Concat(pathHalves.Second.Skip(1)).ToList();
        return path?.ToMoveDirections() ?? Array.Empty<MoveDirection>();
    }

    private static MoveDirection[] ToMoveDirections(this IEnumerable<Point> path) =>
        path.ToBigrams().Select(ps => ps.Item - ps.Prev)
            .Select(Walker.ConvertOffsetToDirection)
            .ToArray();

    private static IEnumerable<(T Prev, T Item)> ToBigrams<T>(this IEnumerable<T> items)
    {
        var start = true;
        T? prev = default;
        foreach (var item in items)
        {
            if (!start)
            {
                yield return (prev!, item);
            }

            start = false;
            prev = item;
        }
    }
}