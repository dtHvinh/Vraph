using HamiltonVisualizer.GraphUIComponents.Interfaces;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace HamiltonVisualizer.GraphUIComponents
{
    public class NodeLabel : TextBox, IUIComponent
    {
        public NodeLabel()
        {
            StyleUIComponent();
            KeyDown += NodeLabel_KeyDown;
        }

        public void StyleUIComponent()
        {
            Background = Brushes.White;
            BorderThickness = new(0);
            Margin = new Thickness(5);
        }

        private void NodeLabel_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                IsEnabled = false;
                ContextMenu = null;
            }
        }
    }
}
