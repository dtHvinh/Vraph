using HamiltonVisualizer.GraphUIComponents;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace HamiltonVisualizer.Core
{
    public class DrawManager(Canvas Canvas)
    {
        public List<Line> Lines { get; } = [];

        /// <summary>
        /// Draw a <see cref="Line"/> and add to the collection.
        /// </summary>
        public void Draw(Node node1, Node node2)
        {
            var node1_y = Canvas.GetLeft(node1);
            var node1_x = Canvas.GetTop(node1);
            var node2_y = Canvas.GetLeft(node2);
            var node2_x = Canvas.GetTop(node2);

            Line myLine = new()
            {
                Stroke = Brushes.Black,
                X1 = node1_x,
                X2 = node1_y,
                Y1 = node2_x,
                Y2 = node2_y,
                StrokeThickness = 1,
                SnapsToDevicePixels = true
            };

            myLine.SetValue(RenderOptions.EdgeModeProperty, EdgeMode.Aliased);
            Canvas.Children.Add(myLine);
            Lines.Add(myLine);
        }
    }
}
