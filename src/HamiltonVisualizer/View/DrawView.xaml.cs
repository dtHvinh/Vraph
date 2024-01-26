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

                AddToCanvas(node);
            }
        }

        /// <summary>
        /// Add Graph node to the view ui.
        /// </summary>
        /// <param name="node"></param>
        private void AddToCanvas(Node node)
        {
            // TODO: add logic to prevent collision
            DrawingCanvas.Children.Add(node);
        }
    }
}
