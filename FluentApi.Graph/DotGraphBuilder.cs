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

        public NodeWithBuilder AddNode(string name) =>
            new NodeWithBuilder(graph.AddNode(name), this);

        public EdgeWithBuilder AddEdge(string from, string to) =>
            new EdgeWithBuilder(graph.AddEdge(from, to), this);

        public string Build() => graph.ToDotFormat();
    }

    public abstract class BuilderContainer
    {
        protected DotGraphBuilder Builder;

        public NodeWithBuilder AddNode(string name) =>
            Builder.AddNode(name);

        public EdgeWithBuilder AddEdge(string from, string to) =>
            Builder.AddEdge(from, to);

        public string Build() => Builder.Build();
    }

    public class NodeWithBuilder : BuilderContainer
    {
        private readonly GraphNode node;

        public NodeWithBuilder(GraphNode node, DotGraphBuilder builder)
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

    public class EdgeWithBuilder : BuilderContainer
    {
        private readonly GraphEdge edge;

        public EdgeWithBuilder(GraphEdge edge, DotGraphBuilder builder)
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

        public NodeAttributes Shape(string value) =>
            AddAttribute(MethodBase.GetCurrentMethod(), value);

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

    public static class NodeShape
    {
        public const string Box = "box";
        public const string Ellipse = "ellipse";
    }
}