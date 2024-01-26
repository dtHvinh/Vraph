using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace HamiltonVisualizer.GraphUIComponents
{
    internal class NodeLabel : TextBox
    {
        public NodeLabel()
        {
            Background = Brushes.White;
            BorderThickness = new(0);
            Margin = new Thickness(5);

            KeyDown += NodeLabel_KeyDown;
        }

        private void NodeLabel_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                IsEnabled = false;
            }
        }
    }
}
