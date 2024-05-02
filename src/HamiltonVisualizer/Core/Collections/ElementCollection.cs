using HamiltonVisualizer.Core.CustomControls.WPFBorder;
using HamiltonVisualizer.Core.CustomControls.WPFLinePolygon;

namespace HamiltonVisualizer.Core.Collections
{
    internal sealed class ElementCollection
    {
        private readonly NodeCollection _nodes = [];
        private readonly LineCollection _edges = [];

        public NodeCollection Nodes { get => _nodes; }
        public LineCollection Edges { get => _edges; }

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
