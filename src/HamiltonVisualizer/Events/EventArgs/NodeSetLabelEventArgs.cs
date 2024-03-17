using HamiltonVisualizer.GraphUIComponents;

namespace HamiltonVisualizer.Events.EventArgs
{
    public class NodeSetLabelEventArgs(Node node, string text)
    {
        public Node Node { get; set; } = node;
        public string? Text { get; set; } = text;
    }
}
