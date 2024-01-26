using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace HamiltonVisualizer.GraphUIComponents
{
    /// <summary>
    /// Graph node.
    /// </summary>
    internal class Node : Border
    {
        public Point Position { get; set; }

        public Node(Point position)
        {
            Position = position;

            Width = 34;
            Height = 34;
            Background = Brushes.White;
            BorderBrush = new SolidColorBrush(Colors.Black);
            BorderThickness = new(2);
            CornerRadius = new(30);

            Canvas.SetLeft(this, Position.X - Width / 2);
            Canvas.SetTop(this, Position.Y - Height / 2);

            Child = new NodeLabel();
        }
    }
}
