using HamiltonVisualizer.Constants;
using HamiltonVisualizer.Events.EventArgs;
using HamiltonVisualizer.Events.EventHandlers;
using HamiltonVisualizer.GraphUIComponents.Interfaces;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace HamiltonVisualizer.GraphUIComponents
{
    /// <summary>
    /// Graph node.
    /// </summary>
    /// <remarks>
    /// This node single child is <see cref="NodeLabel"/>.
    /// </remarks>
    public class Node : Border, IUIComponent
    {
        public Point Origin { get; set; }
        public NodeLabel NodeLabel => (NodeLabel)Child;

        public const int NodeWidth = 34;

        public event NodeDeleteEventHandler? OnNodeDelete;
        public event NodeLabelSetCompleteEventHandler? OnNodeLabelChanged;
        public event OnNodeSelectedEventHandler? OnNodeSelected;

        public Node(Point position)
        {
            Origin = position;

            StyleUIComponent();

            Child = new NodeLabel(this);
            ContextMenu = new NodeContextMenu(this);

            SubscribeEvents();
        }

        private void SubscribeEvents()
        {
            OnNodeDelete += (sender, e) =>
            {
                Background = Brushes.Red;
            };

            OnNodeSelected += (sender, e) =>
            {
                Background = Brushes.LightGreen;
            };
        }

        public void StyleUIComponent()
        {
            Width = NodeWidth;
            Height = NodeWidth;
            Background = Brushes.White;
            BorderBrush = new SolidColorBrush(Colors.Black);
            BorderThickness = new(2);
            CornerRadius = new(30);

            Canvas.SetLeft(this, Origin.X - Width / 2);
            Canvas.SetTop(this, Origin.Y - Height / 2);
            Panel.SetZIndex(this, (int)ZIndexConstants.Node);
        }

        public void DeleteNode()
        {
            OnNodeDelete?.Invoke(this, new NodeDeleteEventArgs(this));
        }

        public void ChangeNodeLabel(string text)
        {
            OnNodeLabelChanged?.Invoke(this, new NodeSetLabelEventArgs(this, text));
        }
        public void ReleaseSelectMode()
        {
            Background = Brushes.White;
        }

        public void SelectNode()
        {
            OnNodeSelected?.Invoke(this, new NodeSelectedEventArgs());
        }
    }
}
