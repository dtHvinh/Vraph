using HamiltonVisualizer.Core.Base;
using HamiltonVisualizer.Core.CustomControls.WPFCanvas;
using HamiltonVisualizer.Events.EventArgs.NodeEventArg;
using HamiltonVisualizer.Events.EventHandlers.ForNode;
using System.Windows;
using System.Windows.Controls;
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

    private readonly NodeStateChangedEventHandler? _nodeStateChanged;

    public ObjectPosition(
        NodeBase node,
        CustomCanvas canvas,
        NodeStateChangedEventHandler? eventHandler)
    {
        _node = node;
        _drawingCanvas = canvas;
        _nodeStateChanged = eventHandler;

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
                _node.Origin = GetValidPosition(curPos);
                _nodeStateChanged?.Invoke(_node, new NodeStateChangeEventArgs(_node.Origin, NodeState.Moving));
            }
        };

        _node.MouseUp += (sender, e) =>
        {
            if (_isDragging)
            {
                _isDragging = false;
                Mouse.Capture(null);
                _nodeStateChanged?.Invoke(_node, new NodeStateChangeEventArgs(State: NodeState.Idle));
            }
        };
    }

    public static Point GetValidPosition(Point point)
    {
        double lowerBound = ES.NodeWidth / 2;
        double upperBound = ES.DrawingCanvasSidesLength - ES.NodeWidth / 2;

        double X;
        double Y;

        if (point.X < lowerBound)
        {
            X = lowerBound;
        }
        else if (point.X > upperBound)
        {
            X = upperBound;
        }
        else
        {
            X = point.X;
        }

        if (point.Y < lowerBound)
        {
            Y = lowerBound;
        }
        else if (point.Y > upperBound)
        {
            Y = upperBound;
        }
        else
        {
            Y = point.Y;
        }

        return new Point(X, Y);
    }

    /// <summary>
    /// This method should be called when Origin is updated
    /// </summary>
    public void OnOriginChanged()
    {
        Canvas.SetLeft(_node, _node.Origin.X - 34 / 2);
        Canvas.SetTop(_node, _node.Origin.Y - 34 / 2);
    }
}