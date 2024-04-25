using HamiltonVisualizer.Constants;
using HamiltonVisualizer.Core.Base;
using HamiltonVisualizer.Core.Collections;
using HamiltonVisualizer.Core.CustomControls.WPFBorder;
using HamiltonVisualizer.Events.EventArgs.ForNode;
using HamiltonVisualizer.Extensions;
using HamiltonVisualizer.Mathematic;
using System.Windows;
using System.Windows.Threading;

namespace HamiltonVisualizer.Core.Functionality
{
    /// <summary>
    /// Represent the physic of the <paramref name="node"/>.
    /// </summary>
    public class ObjectPhysic
    {
        private readonly NodeBase _node;
        private readonly GraphNodeCollection _nodes;
        private readonly double _radius = ConstantValues.ControlSpecifications.NodeWidth / 2;
        private readonly Dispatcher _dispatcher;
        private readonly int _impactForce = 1;

        public ObjectPhysic(NodeBase node, GraphNodeCollection nodes)
        {
            _node = node;
            _nodes = nodes;
            _dispatcher = Dispatcher.CurrentDispatcher;
            _node.OnNodePositionChanged += Node_OnNodePositionChanged;
        }

        private async void Node_OnNodePositionChanged(object? sender, NodePositionChangedEventArgs e)
        {
            await Task.Run(() =>
             {
                 foreach (var node in e.CollideNodes)
                 {
                     _dispatcher.InvokeAsync(() =>
                     {
                         MoveAway(_node, node);
                     });
                 }
             });
        }

        public bool HasNoCollide()
        {
            foreach (Node n in _nodes)
            {
                var dis = TwoDimensional.Distance(_node.Origin.X, _node.Origin.Y, n.Origin.X, n.Origin.Y);
                if (dis >= 0 && dis < 2 * _radius)
                {
                    return false;
                }
            }
            return true;
        }

        public static bool HasNoCollide(Point point, IEnumerable<Node> nodes)
        {
            foreach (Node n in nodes)
            {
                var dis = TwoDimensional.Distance(point.X, point.Y, n.Origin.X, n.Origin.Y);
                if (dis > 0 && dis < ConstantValues.ControlSpecifications.NodeWidth)
                {
                    return false;
                }
            }
            return true;
        }

        public IEnumerable<Node> DetectCollision()
        {
            foreach (Node node in _nodes)
            {
                var dis = TwoDimensional.Distance(_node.Origin.X, _node.Origin.Y, node.Origin.X, node.Origin.Y);
                if (dis > 0 && dis < 2 * _radius)
                {
                    yield return node;
                }
            }
        }

        /// <summary>
        /// Move _nodes that collide with the <paramref name="first"/> node. <br/>
        /// If <paramref name="second"/> node is moved to the corner of the canvas, it will be move to the
        /// almost center of its.
        /// </summary>
        private void MoveAway(NodeBase first, NodeBase second)
        {
            Vector fs = new(second.Origin.X - first.Origin.X, second.Origin.Y - first.Origin.Y);
            fs.Normalize();
            Vector fs_2 = fs * _impactForce;
            var secondNewPos = new Point(second.Origin.X + fs_2.X, second.Origin.Y + fs_2.Y);
            second.Origin = secondNewPos;

            var newPos = ObjectPosition.IfStuckAtCorner(second.Origin);

            if (!second.Origin.TolerantEquals(newPos))
            {
                second.Origin = newPos;
            }
        }
    }
}
