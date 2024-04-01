using HamiltonVisualizer.Core.Base;
using HamiltonVisualizer.Core.CustomControls.WPFCanvas;
using HamiltonVisualizer.Events.EventArgs;
using HamiltonVisualizer.Events.EventHandlers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace HamiltonVisualizer.Core.Functions;

/// <summary>
/// Manage object movement. And raise <see cref="NodeStateChangedEventHandler"/> event.
/// </summary>
/// <param name="node">The node to manage movement.</param>
/// <param name="canvas">The canvas to which the <paramref name="node"/> attach.</param>
/// <param name="eventHandler">The event handler to raise.</param>
public class ObjectMovement
{
    private bool _isDragging;

    private readonly NodeBase _node;
    private readonly DrawingCanvas _drawingCanvas;

    private readonly NodeStateChangedEventHandler? _nodeStateChanged;

    public ObjectMovement(
        NodeBase node,
        DrawingCanvas canvas,
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
                _node.Origin = curPos;
                _nodeStateChanged?.Invoke(_node, new NodeStateChangeEventArgs(_node.Origin, NodeState.Moving));
            }
        };

        _node.MouseUp += (sender, e) =>
        {
            if (_isDragging)
            {
                _isDragging = false;
                _node.Origin = e.GetPosition(_drawingCanvas);
                Mouse.Capture(null);
                _nodeStateChanged?.Invoke(_node, new NodeStateChangeEventArgs(State: NodeState.Idle));
            }
        };
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