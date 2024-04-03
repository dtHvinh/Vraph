using HamiltonVisualizer.Core.Base;
using HamiltonVisualizer.Core.CustomControls.WPFBorder;
using System.Collections.ObjectModel;

namespace HamiltonVisualizer.Core.Functions
{
    /// <summary>
    /// Represent the physic of the <paramref name="node"/>.
    /// </summary>
    /// <param name="node">The node.</param>
    /// <param name="nodes">The nodes on the canvas.</param>
    public class ObjectPhysic(
        NodeBase node,
        List<Node> nodes)
    {
        private readonly NodeBase _node = node;
        private readonly ReadOnlyCollection<Node> _nodes = nodes.AsReadOnly();
    }
}
