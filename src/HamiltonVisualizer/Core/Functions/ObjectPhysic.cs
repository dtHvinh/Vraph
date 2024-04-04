using CSLibraries.Mathematic.Geometry;
using HamiltonVisualizer.Constants;
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
    public class ObjectPhysic
    {
        private readonly NodeBase _node;
        private readonly ReadOnlyCollection<Node> _nodes;

        #region Constructors
        public ObjectPhysic(
            NodeBase node,
            List<Node> nodes)
        {
            _node = node;
            _nodes = nodes.AsReadOnly();
        }

        public ObjectPhysic(
            NodeBase node,
            ReadOnlyCollection<Node> nodes)
        {
            _node = node;
            _nodes = nodes;
        }

        #endregion Constructors

        /// <summary>
        /// No collision detected on this element.
        /// </summary>
        /// <returns></returns>
        public bool IsNoCollide()
        {
            foreach (Node n in _nodes)
            {
                if (CollisionHelper.IsCircleCollide(MathPoint.ConvertFrom(_node.Origin),
                                                    MathPoint.ConvertFrom(n.Origin),
                                                    ConstantValues.ControlSpecifications.NodeWidth / 2))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
