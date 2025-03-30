using DijkstraWithPriorityQueue;
using DijkstraWithPriorityQueue.Graphs;

var graph = new Graph(4);
var weights = new Dictionary<Edge, double>();
weights[graph.Connect(0, 1)] = 1;
weights[graph.Connect(0, 2)] = 2;
weights[graph.Connect(0, 3)] = 6;
weights[graph.Connect(1, 3)] = 4;
weights[graph.Connect(2, 3)] = 2;

Console.WriteLine(
    Dijkstra<HeapPriorityQueue<Node>>(weights, graph[0], graph[3]).ToPrettyString()
); // 0, 2, 3

List<Node> Dijkstra<TPriorityQueue>(Dictionary<Edge, double> weights, Node start, Node end)
    where TPriorityQueue : IPriorityQueue<Node>, new()
{
    var track = new Dictionary<Node, Node?>();
    track[start] = null;
    var queue = new TPriorityQueue();
    queue.Add(start, 0);

    while (true)
    {
        var toOpenPair = queue.PopMin();
        if (toOpenPair == null) return [];
        var (toOpen, price) = toOpenPair.Value;
        if (toOpen == end) break;

        foreach (var e in toOpen.IncidentEdges.Where(e => e.From == toOpen))
        {
            var currentPrice = price + weights[e];
            var nextNode = e.OtherNode(toOpen);
            if (queue.AddOrUpdate(nextNode, currentPrice))
            {
                track[nextNode] = toOpen;
            }
        }
    }
    return GetPathTo(end, track);
}

List<Node> GetPathTo(Node end, Dictionary<Node, Node?> track)
{
    var result = new List<Node>();
    while (end != null)
    {
        result.Add(end);
        end = track[end];
    }
    result.Reverse();
    return result;
}