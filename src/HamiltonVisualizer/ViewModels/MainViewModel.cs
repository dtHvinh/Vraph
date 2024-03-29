using CSLibraries.DataStructure.Graph.Implements;
using HamiltonVisualizer.Core;
using HamiltonVisualizer.GraphUIComponents;
using System.Windows.Shapes;

namespace HamiltonVisualizer.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        private readonly DirectedGraph<string> _graph = new();

        private List<Node> _nodes = []; // nodes in the list are guaranteed to be unique due to the duplicate check in view
        private List<Line> _edges = [];

        public void ProvideRef(List<Node> nodes, List<Line> edges)
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

        public void VM_AddNewNode()
        {
            OnPropertyChanged(nameof(NoV));
        }

        /// <summary>
        /// Update counter.
        /// </summary>
        /// 
        /// <remarks>
        /// Affect:
        /// <list type="bullet">
        /// <item>Decrease NoV counter => Update UI.</item>
        /// </list>
        /// </remarks>
        public void VM_RemoveNode()
        {
            OnPropertyChanged(nameof(NoV));
        }

        public void VM_AddNewEdge(Node u, Node v)
        {
            _graph.AddEdge(u.NodeLabel.Text, v.NodeLabel.Text);
            OnPropertyChanged(nameof(NoE));
        }

        public void VM_RemoveEdge(Node u, Node v)
        {
            _graph.RemoveEdge(u.NodeLabel.Text, v.NodeLabel.Text);
            OnPropertyChanged(nameof(NoE));
        }
    }
}
