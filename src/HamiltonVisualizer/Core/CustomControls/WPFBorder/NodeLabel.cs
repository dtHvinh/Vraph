using HamiltonVisualizer.GraphUIComponents.Interfaces;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace HamiltonVisualizer.Core.CustomControls.WPFBorder
{
    /// <summary>
    /// The label of the <see cref="Node.Node"/>
    /// </summary>
    public class NodeLabel : TextBox, IUIComponent
    {
        /// <summary>
        /// Nodes to which this label attach.
        /// </summary>
        public Node Node { get; set; }

        public NodeLabel(Node node)
        {
            StyleUIComponent();

            KeyDown += NodeLabel_KeyDown;
            LostFocus += NodeLabel_LostFocus;
            Background = Brushes.Transparent;

            ContextMenu = null; // set this to null => right click will use ContextMenu from the Node class
            Node = node;
        }

        public void StyleUIComponent()
        {
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
                    IsEnabled = value;
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

        #endregion
    }
}
