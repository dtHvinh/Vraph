using HamiltonVisualizer.Core.CustomControls.WPFBorder;

namespace HamiltonVisualizer.Events.EventArgs.ForNode
{
    public class NodeSetLabelEventArgs(Node node, string text)
    {
        public Node Node { get; set; } = node;
        public string? Text { get; set; } = text;
    }
}
