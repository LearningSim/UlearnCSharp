namespace GraphPractice;

public class Edge
{
    public readonly Node From;
    public readonly Node To;

    public Edge(Node first, Node second)
    {
        From = first;
        To = second;
    }

    public bool IsIncident(Node node) => From == node || To == node;

    public Node OtherNode(Node node)
    {
        if (!IsIncident(node)) throw new ArgumentException();
        return From == node ? To : From;
    }

    public override string ToString() => $"({From}, {To})";
}