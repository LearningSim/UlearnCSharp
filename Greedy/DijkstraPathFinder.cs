using System;
using System.Collections.Generic;
using System.Linq;
using Greedy.Architecture;

namespace Greedy;

public class DijkstraPathFinder
{
    public IEnumerable<PathWithCost> GetPathsByDijkstra(State state, Point start, IEnumerable<Point> targets)
    {
        var tgts = targets.ToList();
        if (tgts.Contains(start))
        {
            yield return new PathWithCost(0, Array.Empty<Point>());
        }
        
        var cell = start;
        var track = new Dictionary<Point, Cell> { [start] = new(0, null) };
        var explored = new HashSet<Point> { start };
        var beingAnalyzed = new HashSet<Point>();
        while (true)
        {
            foreach (var neighbour in cell.GetNeighbours(state).Except(explored))
            {
                var cost = track[cell].Cost + state.CellCost[neighbour.X, neighbour.Y];
                if (!track.ContainsKey(neighbour) || cost < track[neighbour].Cost)
                {
                    track[neighbour] = new Cell(cost, cell);
                }

                beingAnalyzed.Add(neighbour);
            }

            beingAnalyzed.Remove(cell);
            if (beingAnalyzed.Count == 0) break;
            
            explored.Add(cell);
            cell = beingAnalyzed.MinBy(p => track[p].Cost);
            if (tgts.Contains(cell))
            {
                yield return GetPath(cell, track);
            }
        }
    }

    private PathWithCost GetPath(Point targetCell, Dictionary<Point, Cell> track)
    {
        Point? cell = targetCell;
        var path = new List<Point>();
        while (cell != null)
        {
            path.Add(cell.Value);
            cell = track[cell.Value].Previous;
        }

        return new(track[targetCell].Cost, path.AsEnumerable().Reverse().ToArray());
    }
}

record Cell(int Cost, Point? Previous);

static class Extensions
{
    private static readonly IReadOnlyList<Point> PossibleDirections = new List<Point>
    {
        new(0, -1), new(0, 1), new(-1, 0), new(1, 0)
    };
    
    public static IEnumerable<Point> GetNeighbours(this Point point, State map) =>
        PossibleDirections
            .Select(dir => dir + point)
            .Where(p => map.InsideMap(p) && !map.IsWallAt(p));
}