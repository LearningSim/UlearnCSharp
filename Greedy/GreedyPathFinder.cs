using System.Collections.Generic;
using System.Linq;
using Greedy.Architecture;

namespace Greedy;

public class GreedyPathFinder : IPathFinder
{
    public List<Point> FindPathToCompleteGoal(State state)
    {
        var totalPath = new List<Point>();
        var chests = state.Chests.ToHashSet();
        var pos = state.Position;
        var energy = state.Energy;
        for (int i = 0; i < state.Goal; i++)
        {
            var path = GetPathToFirstChest(state, pos, chests);
            energy -= path?.Cost ?? 0;
            if (path == null || energy < 0)
            {
                return new List<Point>();
            }
            
            totalPath.AddRange(path.Path.Skip(1));
            pos = path.Path.Last();
            chests.Remove(pos);
        }

        return totalPath;
    }

    private PathWithCost? GetPathToFirstChest(State state, Point start, IEnumerable<Point> chests) =>
        new DijkstraPathFinder().GetPathsByDijkstra(state, start, chests)
            .FirstOrDefault();
}