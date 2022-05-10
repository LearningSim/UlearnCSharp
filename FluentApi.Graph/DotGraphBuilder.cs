using System;
using System.Collections.Generic;
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

    public abstract class CommonAttributes<T> where T : CommonAttributes<T>
    {
        private readonly Dictionary<string, string> attributes;

        protected CommonAttributes(Dictionary<string, string> attributes) => this.attributes = attributes;

        public T Color(string value) =>
            AddAttribute(MethodBase.GetCurrentMethod(), value);

        public T FontSize(int value) =>
            AddAttribute(MethodBase.GetCurrentMethod(), value.ToString());

        public T Label(string value) =>
            AddAttribute(MethodBase.GetCurrentMethod(), value);

        protected T AddAttribute(MethodBase method, string value)
        {
            attributes.Add(method.Name.ToLower(), value);
            return (T)this;
        }
    }

    public class NodeAttributes : CommonAttributes<NodeAttributes>
    {
        public NodeAttributes(GraphNode node) : base(node.Attributes)
        {
        }

        public NodeAttributes Shape(NodeShape value) =>
            AddAttribute(MethodBase.GetCurrentMethod(), value.ToString().ToLower());
    }

    public class EdgeAttributes : CommonAttributes<EdgeAttributes>
    {
        public EdgeAttributes(GraphEdge edge) : base(edge.Attributes)
        {
        }

        public EdgeAttributes Weight(double value) =>
            AddAttribute(MethodBase.GetCurrentMethod(), value.ToString(CultureInfo.InvariantCulture));
    }

    public enum NodeShape
    {
        Box,
        Ellipse
    }
}