using HamiltonVisualizer.Constants;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace HamiltonVisualizer.Core.CustomControls.WPFLinePolygon
{
    public class LinePolygonWrapper(Line body, Polygon head)
    {
        private readonly Line _body = body;
        private readonly Polygon _head = head;

        /// <summary>
        /// One of the line head.
        /// </summary>
        public Point From => new(_body.X1, _body.Y1);

        /// <summary>
        /// One of the line head.
        /// </summary>
        public Point To => new(_body.X2, _body.Y2);

        public Line Body => _body;
        public Polygon Head => _head;

        public void ChangeColor(Brush color)
        {
            _head.Stroke = color;
            _body.Fill = color;
        }

        public void ChangeColor(Brush head, Brush body)
        {
            _head.Stroke = head;
            _body.Fill = body;
        }

        public static LinePolygonWrapper Create(Point from, Point to, double headLength = 25, double headWidth = 7.5)
        {
            var body = CreateLine(from, to);
            var head = CreateArrowHead(from, to, headLength, headWidth);
            return new LinePolygonWrapper(body, head);
        }

        private static Line CreateLine(Point node1, Point node2)
        {
            Line line = new()
            {
                Stroke = Brushes.Black,
                X1 = node1.X,
                X2 = node2.X,
                Y1 = node1.Y,
                Y2 = node2.Y,
                StrokeThickness = 2,
            };
            line.SetValue(RenderOptions.EdgeModeProperty, EdgeMode.Aliased);
            Panel.SetZIndex(line, ConstantValues.ZIndex.Line);

            return line;
        }

        private static Polygon CreateArrowHead(Point from, Point to, double height, double sideWidth)
        {
            var ah = new Polygon()
            {
                Fill = Brushes.Black,
                Points = [new Point(to.X, to.Y), new Point(to.X + sideWidth, to.Y + height), new Point(to.X - sideWidth, to.Y + height)]
            };
            Panel.SetZIndex(ah, ConstantValues.ZIndex.Line);// Has the same z index with the obj this head attach to

            // rotate
            var angle = 90 + Math.Atan2(to.Y - from.Y, to.X - from.X) * (180 / Math.PI);
            ah.RenderTransform = new RotateTransform(angle, to.X, to.Y);

            return ah;
        }
    }
}
