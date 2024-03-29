using HamiltonVisualizer.GraphUIComponents.Interfaces;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace HamiltonVisualizer.GraphUIComponents
{
    /// <summary>
    /// The label of the <see cref="GraphUIComponents.Node"/>
    /// </summary>
    public class NodeLabel : TextBox, IUIComponent
    {
        /// <summary>
        /// Nodes to which this label attach.
        /// </summary>
        public Node Node { get; set; }

        public event MouseEventHandler? OnLabelMouseDown;

        public NodeLabel(Node node)
        {
            StyleUIComponent();

            KeyDown += NodeLabel_KeyDown;
            LostFocus += NodeLabel_LostFocus;
            MouseDown += NodeLabel_MouseDown;
            Background = Brushes.Transparent;

            Node = node;
        }

        public void StyleUIComponent()
        {
            ContextMenu = null;

            Background = Brushes.White;
            BorderThickness = new(0);
            Margin = new Thickness(5);
        }

        /// <summary>
        /// If <strong>true</strong>, user is NOT allow to set label text.
        /// </summary>
        public bool IsReadonly
        {
            set
            {
                if (value && IsEnabled)
                {
                    IsEnabled = false;
                    Node.ChangeNodeLabel(Text);
                }
                else
                {
                    IsEnabled = true;
                }
            }
        }

        #region Event listener handle

        private void NodeLabel_KeyDown(object sender, KeyEventArgs e)
        {
            // if user press enter and fill the text before hand, REMOVE modifiability.
            if (e.Key == Key.Enter && !string.IsNullOrEmpty(Text))
            {
                IsReadonly = true;
            }
        }

        private void NodeLabel_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(Text))
                IsReadonly = true;
        }

        // The node is derived from Border class which do not have MouseDown event, so this label 
        // will do on behalf of the node.
        private void NodeLabel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            OnLabelMouseDown?.Invoke(this, e);
        }

        #endregion
    }
}
