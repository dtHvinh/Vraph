using HamiltonVisualizer.Core.CustomControls.WPFBorder;

namespace HamiltonVisualizer.Events.EventArgs.ForNode
{
    /// <summary>
    /// Contain details about delete event associate with node.
    /// </summary>
    internal class NodeDeleteEventArgs(Node node)
    {
        public Node Node { get; set; } = node;
    }
}