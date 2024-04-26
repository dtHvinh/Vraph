using HamiltonVisualizer.Constants;
using HamiltonVisualizer.Core.CustomControls.WPFBorder;
using HamiltonVisualizer.Events.EventArgs.ForGraphLine;
using HamiltonVisualizer.Events.EventHandlers.ForGraphLine;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace HamiltonVisualizer.Core.CustomControls.WPFLinePolygon
{
    internal sealed class GraphLine
    {
        private double HeadLength { get; set; } = 25;
        private double HeadWidth { get; set; } = 7.5;

        private readonly Line _body;
        private readonly Polygon _head;
        private Vector _vec;

        public Node From { get; set; }
        public Node To { get; set; }

        public Line Body => _body;
        public Polygon Head => _head;
        public Vector Vector => _vec;

        public event GraphLineDeleteEventHandler? OnGraphLineDeleted;

        public GraphLine(Node src, Node dst)
        {
            From = src;
            To = dst;

            _body = InitLine();
            _head = InitArrowHead();

            src.Attach(this, ConnectPosition.Head);
            dst.Attach(this, ConnectPosition.Tail);
        }

        public void ChangeColor(Brush color)
        {
            _head.Fill = color;
            _body.Stroke = color;
        }
        public void ResetColor()
        {
            _head.Fill = Brushes.Black;
            _body.Stroke = Brushes.Black;
        }

        private Line InitLine()
        {
            Line line = new()
            {
                Stroke = Brushes.Black,
                X1 = From.Origin.X,
                X2 = To.Origin.X,
                Y1 = From.Origin.Y,
                Y2 = To.Origin.Y,
                StrokeThickness = 2,
            };
            line.SetValue(RenderOptions.EdgeModeProperty, EdgeMode.Aliased);
            Panel.SetZIndex(line, ConstantValues.ZIndex.Line);
            UpdateVector();
            return line;
        }
        private Polygon InitArrowHead()
        {
            var points = CreateArrowHead();
            var ah = new Polygon()
            {
                Fill = Brushes.Black,
                Points = [points.Item1, points.Item2, points.Item3]
            };
            Panel.SetZIndex(ah, ConstantValues.ZIndex.Line);
            return ah;
        }
        private Tuple<Point, Point, Point> CreateArrowHead()
        {
            Point arrHead = To.Origin - Vector * 17;

            Point mediatorPoint = arrHead - Vector * HeadLength;
            Vector b = new(Vector.Y, Vector.X * -1);
            b.Normalize();

            Point arrLeft = mediatorPoint - b * HeadWidth;
            Point arrRight = mediatorPoint + b * HeadWidth;

            return Tuple.Create(
                arrHead,
                arrLeft,
                arrRight);
        }

        public void DeleteFrom(Node node) // detach from one side and execute other side
        {
            if (node.Equals(From))
                To.Detach(this);
            else if (node.Equals(To))
                From.Detach(this);

            OnGraphLineDeleted?.Invoke(this, new GraphLineDeleteEventArgs(this));
        }
        private void UpdateArrowHead() // Update arrow head base on the current position of From and To point
        {
            var points = CreateArrowHead();
            _head.Points[0] = points.Item1;
            _head.Points[1] = points.Item2;
            _head.Points[2] = points.Item3;
        }
        private void UpdateVector()
        {
            _vec = To.Origin - From.Origin;
            _vec.Normalize();
        }

        public void OnHeadOrTailPositionChanged()
        {
            UpdateVector();
            UpdateArrowHead();
        }

        public override bool Equals(object? obj)
        {
            return obj is GraphLine other &&
                other.From.Equals(From) && other.To.Equals(To);
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(_body.GetHashCode(), _head.GetHashCode, HeadLength, HeadWidth);
        }
    }
}
