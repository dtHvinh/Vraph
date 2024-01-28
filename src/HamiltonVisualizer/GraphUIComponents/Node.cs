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
    public class Node : Border, IUIComponent
    {
        #region Public Properties

        /// <summary>
        /// The position of this node.
        /// </summary>
        public Point Position { get; set; }

        /// <summary>
        /// The canvas that contain this node.
        /// </summary>
        public Canvas ContainCanvas { get; set; }

        #endregion Public Properties

        #region Event Handler

        /// <summary>
        /// Event that execute when node delete.
        /// </summary>
        public event NodeDeleteEventHandler? OnNodeDelete;

        public event NodeLabelSetCompleteEventHandler? OnNodeLabelSetComplete;

        #endregion Event Handler

        #region Constants

        public const int NodeWidth = 34;

        #endregion Constants

        #region Constructors

        public Node(Point position, Canvas parent)
        {
            Position = position;

            StyleUIComponent();

            Child = new NodeLabel();
            ContextMenu = new NodeContextMenu(this);
            ContainCanvas = parent;
        }

        #endregion Constructors

        public void StyleUIComponent()
        {
            Width = NodeWidth;
            Height = NodeWidth;
            Background = Brushes.White;
            BorderBrush = new SolidColorBrush(Colors.Black);
            BorderThickness = new(2);
            CornerRadius = new(30);

            Canvas.SetLeft(this, Position.X - Width / 2);
            Canvas.SetTop(this, Position.Y - Height / 2);
        }

        /// <summary>
        /// Delete the node.
        /// </summary>
        public void DeleteRequest()
        {
            ContainCanvas.Children.Remove(this);
            OnNodeDelete?.Invoke(this, new NodeDeleteEventArgs());
        }
    }
}
