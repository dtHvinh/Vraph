using HamiltonVisualizer.Core.CustomControls.WPFCanvas;
using HamiltonVisualizer.Events.EventArgs;
using HamiltonVisualizer.Events.EventHandlers;
using HamiltonVisualizer.Helpers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace HamiltonVisualizer.Core.CustomControls.WPFBorder
{
    /// <summary>
    /// A node that can move on a canvas
    /// </summary>
    public abstract class NodeBase : Border
    {
        private Point _dragCurPos;
        private bool _isDragging;
        private Point _origin;

        private readonly DrawingCanvas _attachCanvas; // the canvas to which this element attach.
        private readonly List<LinePolygonWrapperAttachInfo> _adjacent; // when this element move its position, move other related movable obj

        public Point Origin
        {
            get
            {
                return _origin;
            }
            set
            {
                UpdateNodePositionOnCanvas();
                _origin = value;
            }
        }
        public List<LinePolygonWrapperAttachInfo> Adjacent => _adjacent;

        public event NodeStateChangedEventHandler? OnNodeStateChangedPosition;

        public NodeBase(DrawingCanvas parent, Point position)
        {
            Origin = position;
            _attachCanvas = parent;
            _adjacent = [];

            ImplementMoveCapability();
            SubscribeEvents();
        }

        /// <summary>
        /// This method should be called when Origin is updated
        /// </summary>
        public void UpdateNodePositionOnCanvas()
        {
            Canvas.SetLeft(this, Origin.X - Width / 2);
            Canvas.SetTop(this, Origin.Y - Height / 2);
        }

        private void SubscribeEvents()
        {
            OnNodeStateChangedPosition += (sender, e) =>
            {
                if (e.NewPosition is not null)
                    _adjacent.ForEach(line =>
                    {
                        LinePolygonWrapperRepositionHelper.Move(e.NewPosition.Value, line);
                    });
            };
        }

        public void Attach(LinePolygonWrapperAttachInfo attachInfo)
        {
            _adjacent.Add(attachInfo);
        }

        public void ImplementMoveCapability()
        {
            MouseDown += (sender, e) =>
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    _dragCurPos = e.GetPosition(null);
                    _isDragging = true;
                    Mouse.Capture((IInputElement)sender);
                }
            };

            MouseMove += (sender, e) =>
            {
                if (e.LeftButton == MouseButtonState.Pressed && _isDragging)
                {
                    Point curPos = e.GetPosition(_attachCanvas);
                    Origin = curPos;
                    double deltaX = curPos.X - _dragCurPos.X;
                    double deltaY = curPos.Y - _dragCurPos.Y;

                    double newLeft = Canvas.GetLeft(this) + deltaX;
                    double newTop = Canvas.GetTop(this) + deltaY;

                    Canvas.SetLeft(this, newLeft);
                    Canvas.SetTop(this, newTop);

                    OnNodeStateChangedPosition?.Invoke(this, new NodeStateChangeEventArgs(Origin, NodeState.Moving));
                    _dragCurPos = curPos;
                }
            };

            MouseUp += (sender, e) =>
            {
                if (_isDragging)
                {
                    _isDragging = false;
                    Origin = e.GetPosition(_attachCanvas);
                    Mouse.Capture(null);
                    OnNodeStateChangedPosition?.Invoke(this, new NodeStateChangeEventArgs(State: NodeState.Idle));
                }
            };
        }
    }
}
