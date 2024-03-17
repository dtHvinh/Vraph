using HamiltonVisualizer.GraphUIComponents;

namespace HamiltonVisualizer.Events.EventArgs
{
    /// <summary>
    /// Contain details about delete event associate with node.
    /// </summary>
    public class NodeDeleteEventArgs(Node node)
    {
        public Node Node { get; set; } = node;
    }
}