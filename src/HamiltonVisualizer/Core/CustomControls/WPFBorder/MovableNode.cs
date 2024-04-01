using HamiltonVisualizer.Core.CustomControls.WPFCanvas;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace HamiltonVisualizer.Core.CustomControls.WPFBorder
{
    /// <summary>
    /// A node that can move on a canvas
    /// </summary>
    public abstract class MovableNode : Border
    {
        private readonly DrawingCanvas _attachCanvas; // the canvas to which this element attach.
        private readonly Collection<LinePolygonWrapperAttachInfo> _adjacent; // when this element move its position, move other related movable obj

        private Point _startPoint;
        private bool _isDragging;

        public Point Origin { get; set; }

        public MovableNode(DrawingCanvas parent, Point position)
        {
            Origin = position;
            _attachCanvas = parent;
            _adjacent = [];

            ImplementMoveCapability();
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
                    _startPoint = e.GetPosition(null);
                    _isDragging = true;
                    Mouse.Capture((IInputElement)sender);
                }
            };

            MouseMove += (sender, e) =>
            {
                if (_isDragging)
                {
                    Point curPos = e.GetPosition(null);
                    double deltaX = curPos.X - _startPoint.X;
                    double deltaY = curPos.Y - _startPoint.Y;

                    double newLeft = Canvas.GetLeft(this) + deltaX;
                    double newTop = Canvas.GetTop(this) + deltaY;

                    Canvas.SetLeft(this, newLeft);
                    Canvas.SetTop(this, newTop);

                    _startPoint = curPos;
                }
            };

            MouseUp += (sender, e) =>
            {
                if (_isDragging)
                {
                    _isDragging = false;
                    Origin = e.GetPosition(_attachCanvas);
                    Mouse.Capture(null);
                }
            };
        }
    }
}
