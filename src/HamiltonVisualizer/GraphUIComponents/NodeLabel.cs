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
            LostFocus += NodeLabel_LostFocus;
        }

        public void StyleUIComponent()
        {
            Background = Brushes.White;
            BorderThickness = new(0);
            Margin = new Thickness(5);
        }

        /// <summary>
        /// If <strong>true</strong>, user is allow to set label text.
        /// </summary>
        public void SetValueDone()
        {
            IsEnabled = false;
            ContextMenu = null;
        }

        #region Event listener handle

        private void NodeLabel_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SetValueDone();
            }
        }

        private void NodeLabel_LostFocus(object sender, RoutedEventArgs e)
        {
            SetValueDone();
        }

        #endregion
    }
}
