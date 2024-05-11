using System.Collections.Generic;
using System.Linq;

namespace Rivals;

public static class RivalsTask
{
    private static readonly IReadOnlyList<Point> PossibleDirections = new[]
    {
        new Point(0, -1),
        new Point(0, 1),
        new Point(-1, 0),
        new Point(1, 0)
    };

    public static IEnumerable<OwnedLocation> AssignOwners(Map map)
    {
        var (track, queue, owned) = GetPlayersStartingPositions(map);
        var chests = map.Chests.ToHashSet();
        while (queue.Count != 0)
        {
            var point = queue.Dequeue();
            if (chests.Contains(point.Location)) continue;
            foreach (var neighbour in point.Location.GetNeighbours(map))
            {
                if (track.TryAdd(neighbour, point.Location))
                {
                    var newPoint = new OwnedLocation(point.Owner, neighbour, point.Distance + 1);
                    queue.Enqueue(newPoint);
                    owned.Add(newPoint);
                }
            }
        }

        return owned;
    }

    private static (Dictionary<Point, Point> track, Queue<OwnedLocation> queue, List<OwnedLocation> owned)
        GetPlayersStartingPositions(Map map)
    {
        var track = new Dictionary<Point, Point>();
        var queue = new Queue<OwnedLocation>();
        var owned = new List<OwnedLocation>();
        for (int player = 0; player < map.Players.Length; player++)
        {
            var start = new OwnedLocation(player, map.Players[player], 0);
            track[start.Location] = Point.Null;
            owned.Add(start);
            queue.Enqueue(start);
        }

        return (track, queue, owned);
    }

    private static IEnumerable<Point> GetNeighbours(this Point point, Map map)
    {
        foreach (var dir in PossibleDirections)
        {
            var neighbour = dir + point;
            if (map.InBounds(neighbour) && map.Maze[neighbour.X, neighbour.Y] == MapCell.Empty)
            {
                yield return neighbour;
            }
        }
    }
}