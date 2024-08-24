namespace GraphPractice;

public class Graph
{
    private Node[] nodes;

    public Graph(int nodesCount) =>
        nodes = Enumerable.Range(0, nodesCount).Select(z => new Node(z)).ToArray();

    public int Length => nodes.Length;

    public Node this[int index] => nodes[index];

    public IEnumerable<Node> Nodes => nodes.Select(n => n);

    public Edge this[int from, int to] =>
        this[from].IncidentEdges.First(e => e.OtherNode(this[from]).NodeNumber == to);

    public Edge this[Node from, Node to] =>
        from.IncidentEdges.First(e => e.OtherNode(from).NodeNumber == to.NodeNumber);

    public Edge GetEdge(int from, int to) =>
        this[from].IncidentEdges.First(e => e.OtherNode(this[from]).NodeNumber == to);

    public Edge GetEdge(Node from, Node to) =>
        from.IncidentEdges.First(e => e.OtherNode(from).NodeNumber == to.NodeNumber);

    public Edge Connect(int index1, int index2) =>
        Node.Connect(nodes[index1], nodes[index2], this);

    public void Delete(Edge edge) => Node.Disconnect(edge);

    public IEnumerable<Edge> Edges => nodes.SelectMany(n => n.IncidentEdges).Distinct();

    public static Graph MakeGraph(params int[] incidentNodes)
    {
        var graph = new Graph(incidentNodes.Max() + 1);
        for (int i = 0; i < incidentNodes.Length - 1; i += 2)
            graph.Connect(incidentNodes[i], incidentNodes[i + 1]);
        return graph;
    }
}