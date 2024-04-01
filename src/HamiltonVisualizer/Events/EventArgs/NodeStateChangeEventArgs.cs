using System.Windows;

namespace HamiltonVisualizer.Events.EventArgs
{
    public record class NodeStateChangeEventArgs(Point? NewPosition = null, NodeState? State = null);

    public enum NodeState
    {
        Moving,
        Idle
    }
}
