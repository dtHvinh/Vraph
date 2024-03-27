#nullable disable
using CSLibraries.DataStructure.Graph.Implements;
using HamiltonVisualizer.Core;
using HamiltonVisualizer.GraphUIComponents;

namespace HamiltonVisualizer.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        private readonly DirectedGraph<string> _graph = new();

        public List<Node> Nodes { get; } = []; // nodes in the list are guaranteed to be unique due to the duplicate check in view


        // Bind to view && Do not rename!
        public int NoE { get => _graph.EdgeCount; }
        public int NoV { get => Nodes.Count; }

        public void VM_AddNewNode(Node node)
        {
            Nodes.Add(node);
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
        public void VM_RemoveNode(Node node)
        {
            Nodes.Remove(node);
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
