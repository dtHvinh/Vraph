using HamiltonVisualizer.GraphUIComponents;
using System.Windows.Controls;
using System.Windows.Input;

namespace HamiltonVisualizer.View
{
    /// <summary>
    /// Interaction logic for DrawView.xaml
    /// </summary>
    public partial class DrawView : UserControl
    {
        public List<Node> Nodes { get; set; } = [];

        public DrawView()
        {
            InitializeComponent();
        }

        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var p = e.GetPosition(this);

                Node node = new(p);

                if (EnsureNoCollision(node))
                {
                    AddToCanvas(node);
                }
            }
        }

        private bool EnsureNoCollision(Node node)
        {
            if (Nodes.Count == 0)
                return true;

            foreach (Node n in Nodes)
            {
                if (Libraries.Geometry.CollisionHelper.IsCircleCollide(n.Position.X,
                                                                       n.Position.Y,
                                                                       node.Position.X,
                                                                       node.Position.Y,
                                                                       Node.NodeWidth / 2))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Add Graph node to the view ui.
        /// </summary>
        /// <param name="node"></param>
        private void AddToCanvas(Node node)
        {
            // TODO: add logic to prevent collision
            DrawingCanvas.Children.Add(node);
            Nodes.Add(node);
        }
    }
}
