using CSLibraries.Mathematic.Geometry;
using HamiltonVisualizer.Constants;
using HamiltonVisualizer.Core.Base;
using HamiltonVisualizer.Core.Collections;
using HamiltonVisualizer.Core.CustomControls.WPFBorder;

namespace HamiltonVisualizer.Core.Functions
{
    /// <summary>
    /// Represent the physic of the <paramref name="node"/>.
    /// </summary>
    /// <param name="node">The node.</param>
    /// <param name="nodes">The nodes on the canvas.</param>
    public class ObjectPhysic(
        NodeBase node,
        GraphNodeCollection nodes)
    {
        private readonly NodeBase _node = node;
        private readonly GraphNodeCollection _nodes = nodes;

        /// <summary>
        /// No collision detected on this element.
        /// </summary>
        /// <returns></returns>
        public bool IsNoCollide()
        {
            foreach (Node n in _nodes)
            {
                if (CollisionHelper.IsCircleCollide(_node.Origin.X, _node.Origin.Y,
                                                    n.Origin.X, n.Origin.Y,
                                                    ConstantValues.ControlSpecifications.NodeWidth / 2))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
