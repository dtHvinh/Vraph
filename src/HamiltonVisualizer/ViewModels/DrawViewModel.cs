using HamiltonVisualizer.Core;
using HamiltonVisualizer.GraphUIComponents;
using System.Windows.Input;

namespace HamiltonVisualizer.ViewModels
{
    class DrawViewModel : ObservableObject
    {
        public List<Node> Nodes { get; set; } = [];

        public ICommand AddNodeCommand { get; set; }

        public ICommand ConnectCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        public ICommand ModifyCommand { get; set; }

        public DrawViewModel()
        {
            AddNodeCommand = new RelayCommand(AddNode);
        }

        private void AddNode(object? obj)
        {
            Node node = obj as Node ?? throw new ArgumentNullException(nameof(obj));
            Nodes.Add(node);
        }
    }
}
