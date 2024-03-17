#nullable disable
using HamiltonVisualizer.Core;
using HamiltonVisualizer.GraphUIComponents;
using Libraries.DataStructure.Graph.Component;
using System.Collections.ObjectModel;

namespace HamiltonVisualizer.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        private ObservableCollection<Node> NodeCollection { get; } = [];
        private ObservableCollection<Edge<string>> EdgeCollection { get; } = [];

        // Bind to view && Do not rename!
        public int NoE { get => EdgeCollection.Count; }
        public int NoV { get => NodeCollection.Count; }

        public MainViewModel()
        {
        }

        public void AddNewNode(Node node)
        {
            NodeCollection.Add(node);
            OnPropertyChanged(nameof(NoV));
        }

        public void RemoveNode(Node node)
        {
            NodeCollection.Remove(node);
            OnPropertyChanged(nameof(NoV));
        }

        public void AddNewEdge(Edge<string> edge)
        {
            EdgeCollection.Add(edge);
            OnPropertyChanged(nameof(NoE));
        }

        public void RemoveEdge(Edge<string> edge)
        {
            EdgeCollection.Remove(edge);
            OnPropertyChanged(nameof(NoE));
        }
    }
}
