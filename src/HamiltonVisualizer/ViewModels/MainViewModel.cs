#nullable disable
using HamiltonVisualizer.Core;
using HamiltonVisualizer.GraphUIComponents;
using System.Windows.Shapes;

namespace HamiltonVisualizer.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        public List<Node> Nodes { get; } = [];
        public List<Line> Edges { get; } = [];

        // Bind to view && Do not rename!
        public int NoE { get => Edges.Count; }
        public int NoV { get => Nodes.Count; }

        public void VM_AddNewNode(Node node)
        {
            Nodes.Add(node);
            OnPropertyChanged(nameof(NoV));
        }

        public void VM_RemoveNode(Node node)
        {
            Nodes.Remove(node);
            OnPropertyChanged(nameof(NoV));
        }

        public void VM_AddNewEdge(Line edge)
        {
            Edges.Add(edge);
            OnPropertyChanged(nameof(NoE));
        }

        public void VM_RemoveEdge(Line edge)
        {
            Edges.Remove(edge);
            OnPropertyChanged(nameof(NoE));
        }
    }
}
