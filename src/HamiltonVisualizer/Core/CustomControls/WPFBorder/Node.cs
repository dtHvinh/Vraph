using HamiltonVisualizer.Constants;
using HamiltonVisualizer.Core.Base;
using HamiltonVisualizer.Core.Collections;
using HamiltonVisualizer.Core.CustomControls.WPFCanvas;
using HamiltonVisualizer.Events.EventArgs;
using HamiltonVisualizer.Events.EventHandlers;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;

namespace HamiltonVisualizer.Core.CustomControls.WPFBorder
{
    /// <summary>
    /// Graph node.
    /// </summary>
    /// <remarks>
    /// This node single child is <see cref="NodeLabel"/>.
    /// </remarks>
    /// 
    [DebuggerDisplay("[X:{Origin.X};Y:{Origin.Y}]")]
    public sealed class Node : NodeBase
    {
        public bool _canChangeBackground = true; // prevent accidentally re-colorize selected node

        public new Brush Background
        {
            get
            {
                return base.Background;
            }
            set
            {
                if (_canChangeBackground)
                    base.Background = value;
            }
        }

        public NodeLabel NodeLabel => (NodeLabel)Child;
        public bool IsSelected { get; private set; } = false;

        public event NodeDeleteEventHandler? OnNodeDelete;
        public event NodeLabelSetCompleteEventHandler? OnNodeLabelChanged;
        public event OnNodeSelectedEventHandler? OnNodeSelected;
        public event OnNodeReleaseSelectEventHandler? OnNodeReleaseSelect;

        public Node(DrawingCanvas parrent, Point position, GraphNodeCollection others)
            : base(parrent, position, others)
        {
            StyleUIComponent();

            Child = new NodeLabel(this);
            ContextMenu = new NodeContextMenu(this);

            SubscribeEvents();
        }

        private void SubscribeEvents()
        {
            OnNodeDelete += (sender, e) =>
            {
                _canChangeBackground = true;
                Background = ConstantValues.ControlColors.NodeDeleteBackground;
            };

            OnNodeSelected += (sender, e) =>
            {
                Background = ConstantValues.ControlColors.NodeSelectBackground;
                IsSelected = true;
                _canChangeBackground = false;
            };

            OnNodeReleaseSelect += (sender, e) =>
            {
                _canChangeBackground = true;
                Background = Brushes.White;
                IsSelected = false;
            };
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
            OnNodeReleaseSelect?.Invoke(this, new NodeReleaseSelectEventArgs());
        }

        public void SelectNode()
        {
            OnNodeSelected?.Invoke(this, new NodeSelectedEventArgs());
        }
    }
}
