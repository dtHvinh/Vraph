using HamiltonVisualizer.Constants;
using HamiltonVisualizer.Core.Collections;
using HamiltonVisualizer.Core.Contracts;
using HamiltonVisualizer.Core.CustomControls.WPFCanvas;
using HamiltonVisualizer.Core.CustomControls.WPFLinePolygon;
using HamiltonVisualizer.Core.Functions;
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
    public abstract class NodeBase : Border, IUIComponent, INavigableElement, IMultiLanguageSupport
    {
        private Point _origin;
        private readonly List<GraphLineConnectInfo> _adjacent; // when this element move its position, move other related movable obj

        public ObjectPosition Movement { get; } // manage the matter of movement of this element
        public ObjectPhysic Physic { get; } // manage the matter of physic of this element

        public List<GraphLineConnectInfo> Adjacent => _adjacent;
        public event NodeStateChangedEventHandler? OnNodeStateChanged;

        /// <summary>
        /// When this property is set, the canvas will be updated.
        /// </summary>
        public Point Origin
        {
            get => _origin;
            set
            {
                Movement.OnOriginChanged();
                _origin = value;
            }
        }

        protected NodeBase(
            DrawingCanvas parent,
            Point position,
            GraphNodeCollection others)
        {
            StyleUIComponent();
            SubscribeEvents();

            Movement = new ObjectPosition(this, parent, OnNodeStateChanged);
            Physic = new ObjectPhysic(this, others);
            _adjacent = [];

            Origin = position;
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

        private void SubscribeEvents()
        {
            OnNodeStateChanged += (sender, e) =>
            {
                if (e.NewPosition is not null && e.State == NodeState.Moving)
                    _adjacent.ForEach(line =>
                    {
                        GraphLineRepositionHelper.Move(e.NewPosition.Value, line);
                    });
            };
        }

        public void Attach(GraphLine line, ConnectPosition pos)
        {
            var attachInfo = new GraphLineConnectInfo(line, this, pos);
            _adjacent.Add(attachInfo);
        }

        public void Detach(GraphLine line)
        {
            var attachInfo = _adjacent.First(x => x.Edge.Equals(line));
            _adjacent.Remove(attachInfo);
        }

        public string ToString(string lang)
        {
            return lang switch
            {
                "vi" => $"""
                        Tọa Độ:{new string(' ', 14)}{(int)_origin.X}:{(int)_origin.Y}
                        Số cạnh liền kề:{new string(' ', 2)}{_adjacent.Count}
                        """,
                _ => throw new Exception($"Invalid lang {lang}!"),// TODO: add to EM class
            };
        }
    }
}
