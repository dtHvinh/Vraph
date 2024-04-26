using HamiltonVisualizer.Core.CustomControls.WPFBorder;

namespace HamiltonVisualizer.Events.EventArgs.ForNode;

internal sealed class NodeSetLabelEventArgs(Node node, string text)
{
    public Node Node { get; set; } = node;
    public string? Text { get; set; } = text;
}
