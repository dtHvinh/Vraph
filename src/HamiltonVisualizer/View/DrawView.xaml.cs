using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace HamiltonVisualizer.View
{
    /// <summary>
    /// Interaction logic for DrawView.xaml
    /// </summary>
    public partial class DrawView : UserControl
    {
        public DrawView()
        {
            InitializeComponent();
        }

        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {

                var clickPoint = e.GetPosition(this);

                GraphNode node = new();
                Canvas.SetLeft(node, clickPoint.X - node.Width / 2);
                Canvas.SetTop(node, clickPoint.Y - node.Height / 2);
                Canvas.Children.Add(node);
            }
        }
    }


    internal readonly struct Position(int X, int Y)
    {
        public int X { get; } = X;
        public int Y { get; } = Y;
    }

    internal class GraphNode : Border
    {
        public Position Position { get; set; }

        public GraphNode()
        {
            Width = 30;
            Height = 30;
            Background = Brushes.White;
            BorderBrush = new SolidColorBrush(Colors.Black);
            BorderThickness = new(2);
            CornerRadius = new(30);
        }
    }
}
