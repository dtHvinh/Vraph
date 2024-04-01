using HamiltonVisualizer.Constants;
using HamiltonVisualizer.Core.Contracts;
using HamiltonVisualizer.Core.CustomControls.WPFCanvas;
using HamiltonVisualizer.Events.EventArgs;
using HamiltonVisualizer.Events.EventHandlers;
using HamiltonVisualizer.Helpers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace HamiltonVisualizer.Core.Base
{
    /// <summary>
    /// A node that can move on a canvas
    /// </summary>
    public abstract class NodeBase : Border, IUIComponent, INavigableElement
    {
        private Point _origin;

        private readonly DrawingCanvas _attachCanvas; // the canvas to which this element attach.
        private readonly List<EdgeAttachInfo> _adjacent; // when this element move its position, move other related movable obj
        private readonly ObjectMovementManager _objectMovement; // manage the matter of movement of this element

        public List<EdgeAttachInfo> Adjacent => _adjacent;
        public event NodeStateChangedEventHandler? OnNodeStateChanged;

        /// <summary>
        /// When this property is set, the canvas will be updated.
        /// </summary>
        public Point Origin
        {
            get => _origin;
            set
            {
                _objectMovement.OnOriginChanged();
                _origin = value;
            }
        }

        public void StyleUIComponent()
        {
            Width = ConstantValues.ControlSpecifications.NodeWidth;
            Height = ConstantValues.ControlSpecifications.NodeWidth;
            Background = Brushes.White;
            BorderBrush = new SolidColorBrush(Colors.Black);
            BorderThickness = new(2);
            CornerRadius = new(30);

            Canvas.SetLeft(this, Origin.X - Width / 2);
            Canvas.SetTop(this, Origin.Y - Height / 2);
            Panel.SetZIndex(this, ConstantValues.ZIndex.Node);
        }

        protected NodeBase(DrawingCanvas parent, Point position)
        {
            StyleUIComponent();
            SubscribeEvents();

            _attachCanvas = parent;
            _objectMovement = new ObjectMovementManager(this, _attachCanvas, OnNodeStateChanged);
            Origin = position;
            _adjacent = [];
        }

        private void SubscribeEvents()
        {
            OnNodeStateChanged += (sender, e) =>
            {
                if (e.NewPosition is not null && e.State == NodeState.Moving)
                    _adjacent.ForEach(line =>
                    {
                        EdgeRepositionHelper.Move(e.NewPosition.Value, line);
                    });
            };
        }

        public void Attach(EdgeAttachInfo attachInfo)
        {
            _adjacent.Add(attachInfo);
        }
    }
}
