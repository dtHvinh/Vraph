using HamiltonVisualizer.Core.CustomControls.WPFBorder;

namespace HamiltonVisualizer.Events.EventArgs.NodeEventArg
{
    /// <summary>
    /// Contain details about delete event associate with node.
    /// </summary>
    public class NodeDeleteEventArgs(Node node)
    {
        public Node Node { get; set; } = node;
    }
}