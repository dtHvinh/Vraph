using HamiltonVisualizer.Constants;
using HamiltonVisualizer.Contracts;
using HamiltonVisualizer.Core.Base;
using HamiltonVisualizer.Core.Collections;
using HamiltonVisualizer.Core.CustomControls.WPFBorder;
using HamiltonVisualizer.Events.EventArgs.ForNode;
using HamiltonVisualizer.Mathematic;
using System.Windows;
using System.Windows.Threading;

namespace HamiltonVisualizer.Core.Functionality;

/// <summary>
/// Represent the physic of the <paramref name="node"/>.
/// </summary>
internal sealed class ObjectPhysic : IPhysicInteraction
{
    private readonly MovableObject _node;
    private readonly NodeCollection _nodes;
    private readonly Dispatcher _dispatcher;
    private readonly int _impactForce = 1;
    private readonly double _radius = ConstantValues.ControlSpecifications.NodeWidth / 2;

    public ObjectPhysic(MovableObject node, NodeCollection nodes)
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
                     ApplyForce(node, _impactForce, CornerCheck);
                 });
             }
         });
    }

    private static void CornerCheck(MovableObject other)
    {
        if (ObjectPosition.IfStuckAtCorner(other.Origin))
        {
            // TODO: improve
            other.Origin = new Point(Random.Shared.Next(17, (int)ConstantValues.ControlSpecifications.DrawingCanvasSidesWidth - 17),
                Random.Shared.Next(17, (int)ConstantValues.ControlSpecifications.DrawingCanvasSidesHeight - 17));
        }
    }

    public bool HasCollisions()
    {
        foreach (Node n in _nodes)
        {
            var dis = TwoDimensional.Distance(_node.Origin.X, _node.Origin.Y, n.Origin.X, n.Origin.Y);
            if (dis >= 0 && dis < 2 * _radius)
            {
                return true;
            }
        }
        return false;
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

    public IEnumerable<Node> DetectCollisionOnMove()
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

    private void ApplyForceCore(MovableObject other, double force)
    {
        Vector fs = other.Origin - _node.Origin;
        fs.Normalize();
        var secondNewPos = other.Origin + fs * force;
        other.Origin = secondNewPos;
    }

    private void ApplyForceFromCore(MovableObject source, double force)
    {
        Vector fs = _node.Origin - source.Origin;
        fs.Normalize();
        var newPos = _node.Origin + fs * force;
        _node.Origin = newPos;
    }

    public void ApplyForce(MovableObject other, double force, Action<MovableObject> callback)
    {
        ApplyForceCore(other, force);
        callback?.Invoke(other);
    }

    public void ApplyForce(MovableObject other, double force)
    {
        ApplyForceCore(other, force);
    }

    public void ApplyForceFrom(MovableObject source, double force)
    {
        ApplyForceFromCore(source, force);
    }
}
