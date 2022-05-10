using System;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace FluentApi.Graph
{
    public class DotGraphBuilder : INodeBuilder, IEdgeBuilder
    {
        private readonly Graph graph;

        private DotGraphBuilder(Graph graph) =>
            this.graph = graph;

        public static IDotGraphBuilder DirectedGraph(string graphName) =>
            new DotGraphBuilder(new Graph(graphName, true, true));

        public static IDotGraphBuilder UndirectedGraph(string graphName) =>
            new DotGraphBuilder(new Graph(graphName, false, true));

        public INodeBuilder AddNode(string name)
        {
            graph.AddNode(name);
            return this;
        }

        public IEdgeBuilder AddEdge(string from, string to)
        {
            graph.AddEdge(from, to);
            return this;
        }

        public string Build() => graph.ToDotFormat();

        public IDotGraphBuilder With(Action<NodeAttributes> addAttributes)
        {
            addAttributes(new NodeAttributes(graph.Nodes.Last()));
            return this;
        }

        public IDotGraphBuilder With(Action<EdgeAttributes> addAttributes)
        {
            addAttributes(new EdgeAttributes(graph.Edges.Last()));
            return this;
        }
    }

    public interface IDotGraphBuilder
    {
        INodeBuilder AddNode(string name);
        IEdgeBuilder AddEdge(string from, string to);
        string Build();
    }

    public interface INodeBuilder : IDotGraphBuilder
    {
        IDotGraphBuilder With(Action<NodeAttributes> addAttributes);
    }

    public interface IEdgeBuilder : IDotGraphBuilder
    {
        IDotGraphBuilder With(Action<EdgeAttributes> addAttributes);
    }

    public class NodeAttributes
    {
        private readonly GraphNode node;

        public NodeAttributes(GraphNode node) => this.node = node;

        public NodeAttributes Color(string value) =>
            AddAttribute(MethodBase.GetCurrentMethod(), value);

        public NodeAttributes Shape(NodeShape value) =>
            AddAttribute(MethodBase.GetCurrentMethod(), value.ToString().ToLower());

        public NodeAttributes FontSize(int value) =>
            AddAttribute(MethodBase.GetCurrentMethod(), value.ToString());

        public NodeAttributes Label(string value) =>
            AddAttribute(MethodBase.GetCurrentMethod(), value);

        private NodeAttributes AddAttribute(MethodBase method, string value)
        {
            node.Attributes.Add(method.Name.ToLower(), value);
            return this;
        }
    }

    public class EdgeAttributes
    {
        private readonly GraphEdge edge;

        public EdgeAttributes(GraphEdge edge) => this.edge = edge;

        public EdgeAttributes Color(string value) =>
            AddAttribute(MethodBase.GetCurrentMethod(), value);

        public EdgeAttributes FontSize(int value) =>
            AddAttribute(MethodBase.GetCurrentMethod(), value.ToString());

        public EdgeAttributes Label(string value) =>
            AddAttribute(MethodBase.GetCurrentMethod(), value);

        public EdgeAttributes Weight(double value) =>
            AddAttribute(MethodBase.GetCurrentMethod(), value.ToString(CultureInfo.InvariantCulture));

        private EdgeAttributes AddAttribute(MethodBase method, string value)
        {
            edge.Attributes.Add(method.Name.ToLower(), value);
            return this;
        }
    }

    public enum NodeShape
    {
        Box,
        Ellipse
    }
}