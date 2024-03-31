using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace HamiltonVisualizer.Core.CustomControls.WPFBorder
{
    public class DraggableBorder : Border
    {
        private Point _startPoint;
        private bool _isDragging;

        public DraggableBorder()
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
                    Point currentPosition = e.GetPosition(null);
                    double deltaX = currentPosition.X - _startPoint.X;
                    double deltaY = currentPosition.Y - _startPoint.Y;

                    double newLeft = Canvas.GetLeft(this) + deltaX;
                    double newTop = Canvas.GetTop(this) + deltaY;

                    Canvas.SetLeft(this, newLeft);
                    Canvas.SetTop(this, newTop);

                    _startPoint = currentPosition;
                }
            };

            MouseUp += (sender, e) =>
            {
                if (_isDragging)
                {
                    _isDragging = false;
                    Mouse.Capture(null);
                }
            };
        }
    }
}
