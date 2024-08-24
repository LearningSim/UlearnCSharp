using GraphPractice;

var graph = new Graph(15);
// граф отсюда: https://www.youtube.com/watch?v=fA_xvuqzuGs
var weights = new Dictionary<Edge, double>
{
    [graph.Connect(7, 6)] = 40,
    [graph.Connect(7, 2)] = 70,
    [graph.Connect(7, 8)] = 30,
    [graph.Connect(7, 12)] = 60,
    [graph.Connect(6, 1)] = 0,
    [graph.Connect(6, 10)] = 10,
    [graph.Connect(0, 1)] = 15,
    [graph.Connect(0, 5)] = 20,
    [graph.Connect(10, 5)] = 30,
    [graph.Connect(10, 11)] = 10,
    [graph.Connect(12, 11)] = 35,
    [graph.Connect(12, 13)] = 70,
    [graph.Connect(12, 8)] = 5,
    [graph.Connect(8, 13)] = 70,
    [graph.Connect(8, 3)] = 50,
    [graph.Connect(2, 3)] = 10,
    [graph.Connect(2, 1)] = 25,
    [graph.Connect(9, 4)] = 10,
    [graph.Connect(9, 14)] = 25,
};

Console.WriteLine(FindShortestPathWithDijkstra(graph, weights, graph[7], graph[9]).ToPrettyString());

List<Node> FindShortestPathWithDijkstra(Graph graph, Dictionary<Edge, double> weights, Node start, Node end)
{
    var node = start;
    var track = new Dictionary<Node, DijkstraData>();
    track[start] = new(0, null);
    var explored = new HashSet<Node> { start };
    var beingAnalyzed = new HashSet<Node>();
    while (node != end)
    {
        foreach (var incidentNode in node.IncidentNodes.Except(explored))
        {
            var edge = graph[node, incidentNode];
            var price = track[node].Price + weights[edge];
            if (!track.ContainsKey(incidentNode) || price < track[incidentNode].Price)
            {
                track[incidentNode] = new(price, node);
            }

            beingAnalyzed.Add(incidentNode);
        }

        beingAnalyzed.Remove(node);
        explored.Add(node);
        node = beingAnalyzed.MinBy(n => track[n].Price);
        if (node == null) return [];
    }

    var path = new List<Node>();
    while (node != null)
    {
        path.Add(node);
        node = track[node].Previous;
    }

    return path.AsEnumerable().Reverse().ToList();
}

record DijkstraData(double Price, Node? Previous);