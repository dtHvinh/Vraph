using HamiltonVisualizer.Core;
using HamiltonVisualizer.Events.EventArgs;
using HamiltonVisualizer.GraphUIComponents;
using HamiltonVisualizer.ViewModels;
using Libraries.Geometry;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace HamiltonVisualizer;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainViewModel VM { get; set; }
    public AnimationManager AnimationManager { get; set; }
    public SelectNodeCollection SelectedNodeCollection { get; set; } = new();
    public DrawManager DrawManager { get; set; }

    public List<Node> Nodes { get; set; } = [];
    public List<Line> Edges { get; set; } = [];

    public bool IsSelectMode { get; set; } = false;

    public MainWindow()
    {
        InitializeComponent();

        VM = (MainViewModel)DataContext;

        ArgumentNullException.ThrowIfNull(VM);

        AnimationManager = AnimationManager.Instance;

        DrawManager = new(DrawingCanvas, Edges);
    }

    #region Navbar behaviors

    private void Border_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton == MouseButton.Left)
            DragMove();
    }

    private void Exit(object sender, MouseButtonEventArgs e)
    {
        Application.Current.Shutdown();
    }

    private void MaximizeAndRestore(object sender, MouseButtonEventArgs e)
    {
        if (WindowState == WindowState.Maximized)
        {
            WindowState = WindowState.Normal;
        }
        else
            WindowState = WindowState.Maximized;
    }

    private void MinimizeToTaskbar(object sender, MouseButtonEventArgs e)
    {
        WindowState = WindowState.Minimized;
    }

    #endregion Navbar behaviors

    #region Drawing

    private void ModeButton_Click(object sender, RoutedEventArgs e)
    {
        IsSelectMode = !IsSelectMode;

        var btn = (Button)sender;

        if (IsSelectMode)
        {
            btn.BeginStoryboard(AnimationManager.StoryboardWhenOn);
            btn.Background = Brushes.LightGreen;
            btn.Margin = AnimationManager.ModeButtonOn;
        }
        else
        {
            btn.BeginStoryboard(AnimationManager.StoryboardWhenOff);
            btn.Background = Brushes.Gray;
            btn.Margin = AnimationManager.ModeButtonOff;
        }
    }

    private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed)
        {
            var mPos = e.GetPosition(DrawingCanvas);

            Node node = new(mPos, DrawingCanvas);

            SubscribeNodeEvents(node);

            if (EnsureNoCollision(node))
            {
                AddToCanvas(node);
                VM.VM_AddNewNode(node);
            }
        }
    }

    // TODO: Test this method
    private void SelectNodes(params string[] nodeLabel)
    {
        var selectNode = Nodes.IntersectBy(nodeLabel, n => n.NodeLabel.Text);

        foreach (var node in selectNode)
        {
            node.SelectNode();
        }
    }

    // TODO: Test this method
    private void SelectAll()
    {
        foreach (var node in Nodes)
        {
            node.SelectNode();
        }
    }

    #endregion Drawing

    #region Subcribe Event

    /// <summary>
    /// Subscribe all events for <see cref="Node"/> instance.
    /// </summary>
    private void SubscribeNodeEvents(Node node)
    {
        // when node deleted
        node.RequestNodeDelete += (object sender, NodeDeleteEventArgs e) =>
        {
            VM.VM_RemoveNode(e.Node);
            Nodes.Remove(e.Node);
        };

        // delete duplicate node with identical label
        node.OnNodeLabelChanged += (object sender, NodeSetLabelEventArgs e) =>
        {
            var text = e.Text;

            if (Nodes.Count(e => e.NodeLabel.Text!.Equals(text)) == 2)
            {
                e.Node.DeleteNode();
            }
        };

        // when at select mode
        node.OnNodeSelected += (object sender, NodeSelectedEventArgs e) =>
        {
            SelectedNodeCollection.Add((Node)sender);
        };

        SelectedNodeCollection.PropertyChanged += (sender, e) =>
        {
            if (SelectedNodeCollection.Count == 2)
            {
                var nodes = SelectedNodeCollection.GetFirst2();
                var node1 = nodes.Item1;
                var node2 = nodes.Item2;

                DrawManager.Draw(node1, node2, out var edge);
                VM.VM_AddNewEdge(edge);
            }
        };
    }

    #endregion Subcribe Event

    #region Helper class

    /// <summary>
    /// Make sure there no collision happen between nodes.
    /// </summary>
    /// <param name="node">The node to check.</param>
    private bool EnsureNoCollision(Node node)
    {
        if (Nodes.Count == 0)
            return true;

        foreach (Node n in Nodes)
        {
            if (CollisionHelper.IsCircleCollide(Libraries.Geometry.Point.ConvertFrom(node.Origin),
                                                Libraries.Geometry.Point.ConvertFrom(n.Origin),
                                                Node.NodeWidth / 2))
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Add Graph node to the view ui.
    /// </summary>
    /// <param name="node"></param>
    private void AddToCanvas(Node node)
    {
        DrawingCanvas.Children.Add(node);
        Nodes.Add(node);
    }

    #endregion Helper class
}