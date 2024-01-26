using HamiltonVisualizer.GraphUIComponents.Interfaces;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace HamiltonVisualizer.GraphUIComponents
{
    /// <summary>
    /// Graph node.
    /// </summary>
    public class Node : Border, IUIComponent
    {
        public Point Position { get; set; }

        public Node(Point position)
        {
            Position = position;

            StyleUIComponent();

            Child = new NodeLabel();
            ContextMenu = new NodeContextMenu(this);
        }

        public void StyleUIComponent()
        {
            Width = 34;
            Height = 34;
            Background = Brushes.White;
            BorderBrush = new SolidColorBrush(Colors.Black);
            BorderThickness = new(2);
            CornerRadius = new(30);

            Canvas.SetLeft(this, Position.X - Width / 2);
            Canvas.SetTop(this, Position.Y - Height / 2);
        }
    }
}
