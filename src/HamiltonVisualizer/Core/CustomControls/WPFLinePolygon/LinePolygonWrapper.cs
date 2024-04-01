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

        private const double HeadLengthDefault = 25;
        private const double HeadWidthDefault = 7.5;

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

        public static LinePolygonWrapper Create(Point from, Point to, double headLength = HeadLengthDefault, double headWidth = HeadWidthDefault)
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

        private static Tuple<Point, Point, Point> CreateArrowHead(Point arrowHeadPos, double height = HeadLengthDefault, double sideWidth = HeadWidthDefault)
        {
            return Tuple.Create(arrowHeadPos, new Point(arrowHeadPos.X + sideWidth, arrowHeadPos.Y + height), new Point(arrowHeadPos.X - sideWidth, arrowHeadPos.Y + height));
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

        public void UpdateArrowHead(Point newArrowHead)
        {
            var angle = 90 + Math.Atan2(To.Y - From.Y, To.X - From.X) * (180 / Math.PI);
            var points = CreateArrowHead(newArrowHead);
            _head.Points[0] = points.Item1; // the head of the arrow alway be the first element in the collection.
            _head.Points[1] = points.Item2; // the head of the arrow alway be the first element in the collection.
            _head.Points[2] = points.Item3; // the head of the arrow alway be the first element in the collection.
            _head.RenderTransform = new RotateTransform(angle, To.X, To.Y);
        }

        public void UpdateArrowHeadRotation()
        {
            var angle = 90 + Math.Atan2(To.Y - From.Y, To.X - From.X) * (180 / Math.PI);
            _head.RenderTransform = new RotateTransform(angle, To.X, To.Y);
        }
    }
}
