using CSLibraries.DataStructure.Graph.Implements;
using HamiltonVisualizer.Core;
using HamiltonVisualizer.Extensions;
using HamiltonVisualizer.GraphUIComponents;

namespace HamiltonVisualizer.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        private readonly DirectedGraph<int> _graph = new();
        private readonly PointMap _map = new();

        private List<Node> _nodes = []; // nodes in the list are guaranteed to be unique due to the duplicate check in view
        private List<LinePolygonWrapper> _edges = [];

        public void ProvideRef(List<Node> nodes, List<LinePolygonWrapper> edges)
        {
            _nodes = nodes;
            _edges = edges;
        }

        public int NoE // for view binding do not rename!
        {
            get
            {
                return _edges.Count;
            }
        }

        public int NoV // for view binding do not rename!
        {
            get
            {
                return _nodes.Count;
            }
        }

        public void VM_NodeAdded()
        {
            OnPropertyChanged(nameof(NoV));
        }

        /// <summary>
        /// Update counter. Return a collection of <see cref="LinePolygonWrapper"> objects need to be remove.
        /// </summary>
        /// 
        /// <remarks>
        /// Affect:
        /// <list type="bullet">
        /// <item>Decrease NoV counter => Update UI.</item>
        /// </list>
        /// </remarks>
        /// 
        ///  <param name="pendingRemove">The <see cref="LinePolygonWrapper"/> objects that related to this object.</param>
        public void VM_NodeRemoved(Node node, out List<LinePolygonWrapper> pendingRemove)
        {
            OnPropertyChanged(nameof(NoV));
            pendingRemove = _edges.Where(e => e.From.TolerantEquals(node.Origin) || e.To.TolerantEquals(node.Origin)).ToList();
        }

        public void VM_EdgeAdded(LinePolygonWrapper line)
        {
            OnPropertyChanged(nameof(NoE));

            var u = _map.LookUp(line.From);
            var v = _map.LookUp(line.To);

            _graph.AddEdge(u, v);
        }

        public void VM_EdgeRemoved()
        {
            OnPropertyChanged(nameof(NoE));
        }
    }
}
