using System.Collections.Generic;
using System.Linq;

namespace Dungeon;

public static class BfsTask
{
	public static IEnumerable<SinglyLinkedList<Point>> FindPaths(Map map, Point start, Chest[] chests)
	{
		var chestLocs = chests.Select(c => c.Location).ToHashSet();
		var queue = new Queue<SinglyLinkedList<Point>>();
		queue.Enqueue(new(start));
		var track = new Dictionary<Point, Point>();
		track[start] = Point.Null;

		while (queue.Count != 0 && chestLocs.Count != 0)
		{
			var point = queue.Dequeue();
			if (chestLocs.Contains(point.Value))
			{
				chestLocs.Remove(point.Value);
				yield return point;
			}

			foreach (var neighbour in point.Value.GetNeighbours(map))
			{
				if (track.TryAdd(neighbour, point.Value))
				{
					queue.Enqueue(new(neighbour, point));
				}
			}
		}
	}

	private static IEnumerable<Point> GetNeighbours(this Point point, Map map) =>
		Walker.PossibleDirections
			.Select(dir => dir + point)
			.Where(p => map.InBounds(p) && map.Dungeon[p.X, p.Y] == MapCell.Empty);
}