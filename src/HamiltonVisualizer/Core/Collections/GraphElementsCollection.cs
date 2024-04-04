using HamiltonVisualizer.Core.Contracts;
using HamiltonVisualizer.Core.CustomControls.WPFBorder;
using HamiltonVisualizer.Core.CustomControls.WPFLinePolygon;
using System.Collections.ObjectModel;

namespace HamiltonVisualizer.Core.Collections
{
    public class GraphElementsCollection : IReadOnlyRefGetter<ReadOnlyCollection<Node>, ReadOnlyCollection<GraphLine>>
    {
        private readonly List<Node> _nodes = [];
        private readonly List<GraphLine> _edges = [];

        public List<Node> Nodes { get => _nodes; }
        public List<GraphLine> Edges { get => _edges; }

        /// <summary>
        /// Get readonly collection of <see cref="Node"/> and <see cref="GraphLine"/>.
        /// </summary>
        /// <returns></returns>
        public (ReadOnlyCollection<Node>, ReadOnlyCollection<GraphLine>) GetReadOnlyRefs()
        {
            return (_nodes.AsReadOnly(), _edges.AsReadOnly());
        }

        public void Add(Node node)
        {
            _nodes.Add(node);
        }

        public void Add(GraphLine line)
        {
            _edges.Add(line);
        }

        public void Remove(Node node)
        {
            _nodes.Remove(node);
        }

        public void Remove(GraphLine line)
        {
            _edges.Remove(line);
        }

        public void ClearAll()
        {
            _nodes.Clear();
            _edges.Clear();
        }
    }
}
