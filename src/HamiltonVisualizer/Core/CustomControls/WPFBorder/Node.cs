using HamiltonVisualizer.Constants;
using HamiltonVisualizer.Core.Base;
using HamiltonVisualizer.Core.Collections;
using HamiltonVisualizer.Core.CustomControls.WPFCanvas;
using HamiltonVisualizer.Events.EventArgs.ForNode;
using HamiltonVisualizer.Events.EventHandlers.ForNode;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace HamiltonVisualizer.Core.CustomControls.WPFBorder;

/// <summary>
/// Graph node.
/// </summary>
/// <remarks>
/// This node single child is <see cref="NodeLabel"/>.
/// </remarks>
/// 
[DebuggerDisplay("[Labels:{NodeLabel.Text};X:{Origin.X};Y:{Origin.Y}]")]
internal sealed class Node : MovableObject
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

    public event NodeDeleteEventHandler? NodeDelete;
    public event NodeLabelSetCompleteEventHandler? NodeLabelChanged;
    public event OnNodeSelectedEventHandler? NodeSelected;
    public event OnNodeReleaseSelectEventHandler? NodeReleaseSelect;

    public Node(CustomCanvas parent, Point position, NodeCollection others)
        : base(parent, position, others)
    {
        StyleUIComponent();

        Child = new NodeLabel(this);
        ContextMenu = new NodeContextMenu(this);

        SubscribeEvents();
    }

    private void SubscribeEvents()
    {
        MouseDown += (sender, e) =>
        {
            if (e.MiddleButton == MouseButtonState.Pressed)
            {
                if (IsSelected)
                    OnReleaseSelectMode();
                else
                    OnSelectNode();
            }
        };
        NodeDelete += (sender, e) =>
        {
            _canChangeBackground = true;
            Background = ConstantValues.ControlColors.NodeDeleteBackground;
        };

        NodeSelected += (sender, e) =>
        {
            Background = ConstantValues.ControlColors.NodeSelectBackground;
            IsSelected = true;
            _canChangeBackground = false;
        };

        NodeReleaseSelect += (sender, e) =>
        {
            _canChangeBackground = true;
            Background = Brushes.White;
            IsSelected = false;
        };
    }

    public void DeleteNode()
    {
        NodeDelete?.Invoke(this, new NodeDeleteEventArgs(this));
    }

    public void OnChangeNodeLabel(string text)
    {
        NodeLabelChanged?.Invoke(this, new NodeSetLabelEventArgs(this, text));
    }

    public void OnReleaseSelectMode()
    {
        NodeReleaseSelect?.Invoke(this, new NodeReleaseSelectEventArgs());
    }

    public void OnSelectNode()
    {
        NodeSelected?.Invoke(this, new NodeSelectedEventArgs());
    }

    public override string ToString(string lang)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"Nhãn:{new string(' ', 17)}{NodeLabel.Text}");
        sb.AppendLine(base.ToString(lang));
        return sb.ToString();
    }
}
