using System.Windows;

namespace HamiltonVisualizer.Events.EventArgs.ForNode
{
    internal record class NodeStateChangeEventArgs(Point? NewPosition = null, NodeState? State = null);

    internal enum NodeState
    {
        Moving,
        Idle
    }
}
