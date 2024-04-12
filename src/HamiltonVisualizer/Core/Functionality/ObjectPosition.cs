using HamiltonVisualizer.Core.Base;
using HamiltonVisualizer.Core.CustomControls.WPFCanvas;
using HamiltonVisualizer.Events.EventArgs.NodeEventArg;
using HamiltonVisualizer.Events.EventHandlers.ForNode;
using HamiltonVisualizer.Extensions;
using System.Windows;
using System.Windows.Input;
using ES = HamiltonVisualizer.Constants.ConstantValues.ControlSpecifications;


namespace HamiltonVisualizer.Core.Functions;

/// <summary>
/// Manage object poistion. And raise <see cref="NodeStateChangedEventHandler"/> event.
/// </summary>
/// <param name="node">The node to manage movement.</param>
/// <param name="canvas">The canvas to which the <paramref name="node"/> attach.</param>
/// <param name="eventHandler">The event handler to raise.</param>
public class ObjectPosition
{
    private bool _isDragging;

    private readonly NodeBase _node;
    private readonly CustomCanvas _drawingCanvas;

    private const double _maxX = ES.DrawingCanvasSidesLength - ES.NodeWidth / 2;
    private const double _maxY = ES.DrawingCanvasSidesLength - ES.NodeWidth / 2;
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
            }
        };

        _node.MouseMove += (sender, e) =>
        {
            if (e.LeftButton == MouseButtonState.Pressed && _isDragging)
            {
                Point curPos = e.GetPosition(_drawingCanvas);
                _node.Origin = curPos;
            }
        };

        _node.MouseUp += (sender, e) =>
        {
            if (_isDragging)
            {
                _isDragging = false;
                Mouse.Capture(null);

                _node.OnStateChanged(null, state: NodeState.Idle);
            }
        };
    }

    public static Point TryStayInBound(Point point)
    {
        double lowerBound = ES.NodeWidth / 2;
        double upperBound = ES.DrawingCanvasSidesLength - ES.NodeWidth / 2;

        double X;
        double Y;

        if (point.X < lowerBound)
            X = lowerBound;
        else if (point.X > upperBound)
            X = upperBound;
        else
            X = point.X;

        if (point.Y < lowerBound)
            Y = lowerBound;
        else if (point.Y > upperBound)
            Y = upperBound;
        else
            Y = point.Y;
        return new Point(X, Y);
    }

    /// <summary>
    /// Check if <paramref name="point"/> is the position of one of the corner, if true
    /// return the center of the canvas base on constant values specified in <see cref="Constants.ConstantValues.ControlSpecifications"/>
    /// </summary>
    public static Point IfStuckAtCorner(Point point)
    {
        var bottomLeft = new Point(_minX, _minY);
        var bottomRight = new Point(_maxX, _minY);
        var topLeft = new Point(_minX, _maxY);
        var topRight = new Point(_maxX, _maxY);

        if (point.TolerantEquals(bottomLeft)
            || point.TolerantEquals(bottomRight)
            || point.TolerantEquals(topLeft)
            || point.TolerantEquals(topRight))
        {
            return new Point(_maxX / 2, _maxY / 2);
        }

        return point;
    }
}