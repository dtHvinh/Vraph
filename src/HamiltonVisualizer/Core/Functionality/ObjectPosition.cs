using HamiltonVisualizer.Core.Base;
using HamiltonVisualizer.Core.CustomControls.WPFCanvas;
using HamiltonVisualizer.Events.EventArgs.ForNode;
using HamiltonVisualizer.Events.EventHandlers.ForNode;
using HamiltonVisualizer.Extensions;
using System.Windows;
using System.Windows.Input;
using ES = HamiltonVisualizer.Constants.ConstantValues.ControlSpecifications;


namespace HamiltonVisualizer.Core.Functionality;

/// <summary>
/// Manage object poistion. And raise <see cref="NodeStateChangedEventHandler"/> event.
/// </summary>
/// <param name="node">The node to manage movement.</param>
/// <param name="canvas">The canvas to which the <paramref name="node"/> attach.</param>
/// <param name="eventHandler">The event handler to raise.</param>
internal sealed class ObjectPosition
{
    private bool _isDragging;

    private readonly NodeBase _node;
    private readonly CustomCanvas _drawingCanvas;

    private const double _maxX = ES.DrawingCanvasSidesWidth - ES.NodeWidth / 2;
    private const double _maxY = ES.DrawingCanvasSidesHeight - ES.NodeWidth / 2;
    private const double _minX = ES.NodeWidth / 2;
    private const double _minY = ES.NodeWidth / 2;

    public ObjectPosition(
        NodeBase node,
        CustomCanvas canvas)
    {
        _node = node;
        _drawingCanvas = canvas;

        ImplementMoveCapability();
    }

    public void ImplementMoveCapability()
    {
        _node.MouseDown += (sender, e) =>
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                _isDragging = true;
                Mouse.Capture((IInputElement)sender);
                _node.MouseMove += Node_MouseMove;
                Mouse.OverrideCursor = Cursors.SizeAll;
            }
        };

        _node.MouseUp += (sender, e) =>
        {
            if (_isDragging)
            {
                _isDragging = false;
                Mouse.Capture(null);
                _node.MouseMove -= Node_MouseMove;

                _node.OnStateChanged(null, state: NodeState.Idle);
                Mouse.OverrideCursor = Cursors.Arrow;
            }
        };
    }

    private void Node_MouseMove(object sender, MouseEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed && _isDragging)
        {
            Point curPos = e.GetPosition(_drawingCanvas);
            _node.Origin = curPos;
        }
    }

    public static Point TryStayInBound(Point point)
    {
        double lowerBound = ES.NodeWidth / 2;
        double upperBoundX = ES.DrawingCanvasSidesWidth - ES.NodeWidth / 2;
        double upperBoundY = ES.DrawingCanvasSidesHeight - ES.NodeWidth / 2;

        double X;
        double Y;

        if (point.X < lowerBound)
            X = lowerBound;
        else if (point.X > upperBoundX)
            X = upperBoundX;
        else
            X = point.X;

        if (point.Y < lowerBound)
            Y = lowerBound;
        else if (point.Y > upperBoundY)
            Y = upperBoundY;
        else
            Y = point.Y;
        return new Point(X, Y);
    }

    public static bool IfStuckAtCorner(Point point)
    {
        var bottomLeft = new Point(_minX, _minY);
        var bottomRight = new Point(_maxX, _minY);
        var topLeft = new Point(_minX, _maxY);
        var topRight = new Point(_maxX, _maxY);

        return point.TolerantEquals(bottomLeft)
            || point.TolerantEquals(bottomRight)
            || point.TolerantEquals(topLeft)
            || point.TolerantEquals(topRight);
    }
}