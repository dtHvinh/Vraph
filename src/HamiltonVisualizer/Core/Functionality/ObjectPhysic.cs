using CSLibraries.Mathematic.Geometry;
using HamiltonVisualizer.Constants;
using HamiltonVisualizer.Core.Base;
using HamiltonVisualizer.Core.Collections;
using HamiltonVisualizer.Core.CustomControls.WPFBorder;
using HamiltonVisualizer.Events.EventArgs.ForNode;
using System.Windows;

namespace HamiltonVisualizer.Core.Functions
{
    /// <summary>
    /// Represent the physic of the <paramref name="node"/>.
    /// </summary>
    public class ObjectPhysic
    {
        private readonly NodeBase _node;
        private readonly GraphNodeCollection _nodes;

        private readonly double _radius = ConstantValues.ControlSpecifications.NodeWidth / 2;

        public ObjectPhysic(NodeBase node, GraphNodeCollection nodes)
        {
            _node = node;
            _nodes = nodes;

            _node.OnNodePositionChanged += Node_OnNodePositionChanged;
        }

        private void Node_OnNodePositionChanged(object? sender, NodePositionChangedEventArgs e)
        {
            foreach (var node in e.CollideNodes)
            {
                MoveAway(_node, node);
            }
        }

        /// <summary>
        /// No collision detected on this element.
        /// </summary>
        /// <returns></returns>
        public bool IsNoCollide()
        {
            foreach (Node n in _nodes)
            {
                var dis = PointHelper.Distance(_node.Origin.X, _node.Origin.Y, n.Origin.X, n.Origin.Y);
                if (dis > 0 && dis < 2 * _radius)
                {
                    return false;
                }
            }
            return true;
        }

        public IEnumerable<Node> CollideDetect()
        {
            foreach (Node node in _nodes)
            {
                var dis = PointHelper.Distance(_node.Origin.X, _node.Origin.Y, node.Origin.X, node.Origin.Y);
                if (dis > 0 && dis < 2 * _radius)
                {
                    yield return node;
                }
            }
        }

        private static void MoveAway(NodeBase first, NodeBase second)
        {
            var newX = 2 * second.Origin.X - first.Origin.X;
            var newY = 2 * second.Origin.Y - first.Origin.Y;
            var secondNewPos = new Point(newX, newY);

            second.Origin = secondNewPos;
        }
    }
}
