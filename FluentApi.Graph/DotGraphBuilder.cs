using System;
using System.Globalization;
using System.Reflection;

namespace FluentApi.Graph
{
    public class DotGraphBuilder
    {
        private readonly Graph graph;

        private DotGraphBuilder(Graph graph) =>
            this.graph = graph;

        public static DotGraphBuilder DirectedGraph(string graphName) =>
            new DotGraphBuilder(new Graph(graphName, true, true));

        public static DotGraphBuilder UndirectedGraph(string graphName) =>
            new DotGraphBuilder(new Graph(graphName, false, true));

        public NodeBuilder AddNode(string name) =>
            new NodeBuilder(graph.AddNode(name), this);

        public EdgeBuilder AddEdge(string from, string to) =>
            new EdgeBuilder(graph.AddEdge(from, to), this);

        public string Build() => graph.ToDotFormat();
    }

    public abstract class ItemBuilder
    {
        protected DotGraphBuilder Builder;

        public NodeBuilder AddNode(string name) =>
            Builder.AddNode(name);

        public EdgeBuilder AddEdge(string from, string to) =>
            Builder.AddEdge(from, to);

        public string Build() => Builder.Build();
    }

    public class NodeBuilder : ItemBuilder
    {
        private readonly GraphNode node;

        public NodeBuilder(GraphNode node, DotGraphBuilder builder)
        {
            this.node = node;
            Builder = builder;
        }

        public DotGraphBuilder With(Action<NodeAttributes> addAttributes)
        {
            addAttributes(new NodeAttributes(node));
            return Builder;
        }
    }

    public class EdgeBuilder : ItemBuilder
    {
        private readonly GraphEdge edge;

        public EdgeBuilder(GraphEdge edge, DotGraphBuilder builder)
        {
            this.edge = edge;
            Builder = builder;
        }

        public DotGraphBuilder With(Action<EdgeAttributes> addAttributes)
        {
            addAttributes(new EdgeAttributes(edge));
            return Builder;
        }
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