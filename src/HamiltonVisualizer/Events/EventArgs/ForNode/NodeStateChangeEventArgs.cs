using System.Windows;

namespace HamiltonVisualizer.Events.EventArgs.ForNode;

internal sealed record class NodeStateChangeEventArgs(Point? NewPosition = null, NodeState? State = null);

internal enum NodeState
{
    Moving,
    Idle
}
