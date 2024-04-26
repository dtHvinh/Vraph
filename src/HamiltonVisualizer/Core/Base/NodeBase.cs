using HamiltonVisualizer.Constants;
using HamiltonVisualizer.Core.Collections;
using HamiltonVisualizer.Core.Contracts;
using HamiltonVisualizer.Core.CustomControls.WPFBorder;
using HamiltonVisualizer.Core.CustomControls.WPFCanvas;
using HamiltonVisualizer.Core.CustomControls.WPFLinePolygon;
using HamiltonVisualizer.Core.Functionality;
using HamiltonVisualizer.Events.EventArgs.ForNode;
using HamiltonVisualizer.Events.EventHandlers.ForNode;
using HamiltonVisualizer.Extensions;
using HamiltonVisualizer.Helpers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace HamiltonVisualizer.Core.Base
{
    /// <summary>
    /// A node that can move on a canvas
    /// </summary>
    internal abstract class NodeBase :
        Border,
        IUIComponent,
        INavigableElement,
        IMultiLanguageSupport,
        IEquatable<NodeBase>
    {
        private Point _origin;
        private readonly List<GraphLineConnectInfo> _adjacent;

        public ObjectPhysic PhysicManager { get; }
        public ObjectPosition PositionManager { get; }

        public List<GraphLineConnectInfo> Adjacent => _adjacent;

        public event NodeStateChangedEventHandler? OnNodeStateChanged;
        public event NodePositionChangedEventHandler? OnNodePositionChanged;

        /// <summary>
        /// Set this property will move the Node on canvas.
        /// </summary>
        public Point Origin
        {
            get => _origin;
            set
            {
                var validPos = ObjectPosition.TryStayInBound(value);

                var collideNode = PhysicManager.DetectCollision();

                _origin = validPos;

                OnStateChanged(validPos, NodeState.Moving);
                OnPositionChanged(validPos, collideNode).ConfigureAwait(true);

                Canvas.SetLeft(this, validPos.X - ConstantValues.ControlSpecifications.NodeWidth / 2);
                Canvas.SetTop(this, validPos.Y - ConstantValues.ControlSpecifications.NodeWidth / 2);
            }
        }

        protected NodeBase(CustomCanvas parent, Point position, GraphNodeCollection others)
        {
            StyleUIComponent();

            PositionManager = new ObjectPosition(this, parent);
            PhysicManager = new ObjectPhysic(this, others);
            _adjacent = [];

            Origin = position;
        }

        public void OnStateChanged(Point? newPosition, NodeState? state)
        {
            OnNodeStateChanged?.Invoke(this, new NodeStateChangeEventArgs(newPosition, state));
        }

        public async Task OnPositionChanged(Point newPosition, IEnumerable<Node> nodes)
        {
            await Task.Run(() =>
            {
                Dispatcher.InvokeAsync(() =>
                {
                    _adjacent.ForEach(line =>
                    {
                        GraphLineRepositionHelper.Move(newPosition, line);
                    });
                });
            });
            OnNodePositionChanged?.Invoke(this, new NodePositionChangedEventArgs(newPosition, nodes));
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

        public virtual string ToString(string lang)
        {
            return lang switch
            {
                "vi" => $"""
                        Tọa Độ:{new string(' ', 14)}{(int)_origin.X}:{(int)_origin.Y}
                        Bậc:{new string(' ', 20)}{_adjacent.Count}
                        """,
                _ => throw new Exception($"Invalid lang {lang}!"),// TODO: add to EM class
            };
        }
        public bool Equals(NodeBase? other)
        {
            return other is not null && Origin.TolerantEquals(other.Origin);
        }
    }
}
