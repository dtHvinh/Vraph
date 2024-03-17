using HamiltonVisualizer.Core;
using HamiltonVisualizer.Events.EventArgs;
using HamiltonVisualizer.GraphUIComponents;
using HamiltonVisualizer.ViewModels;
using Libraries.Geometry;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace HamiltonVisualizer;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainViewModel VM { get; set; }
    public List<Node> Nodes { get; set; } = [];
    public AnimationManager AnimationManager { get; set; }
    public SelectNodeCollection SelectNodeCollection { get; set; } = new();
    public DrawManager DrawManager { get; set; }

    public bool IsSelectMode { get; set; } = false;

    public MainWindow()
    {
        InitializeComponent();

        VM = (MainViewModel)DataContext;

        ArgumentNullException.ThrowIfNull(VM);

        AnimationManager = AnimationManager.Instance;

        DrawManager = new(DrawingCanvas);
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
                VM.AddNewNode(node);
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
        node.RequestNodeDelete += (object sender, NodeDeleteEventArgs e) =>
        {
            VM.RemoveNode(e.Node);
            Nodes.Remove(e.Node);
        };

        node.OnNodeLabelChanged += (object sender, NodeSetLabelEventArgs e) =>
        {
            // Check if the node label is distinct.
            var text = e.Text;

            // Nodes is added before the label is set so the duplicate only happen after it.
            if (Nodes.Count(e => e.NodeLabel.Text!.Equals(text)) == 2)
            {
                e.Node.DeleteNode();
            }
        };

        node.OnNodeSelected += (object sender, NodeSelectedEventArgs e) =>
        {
            SelectNodeCollection.Add((Node)sender);
        };

        SelectNodeCollection.PropertyChanged += (sender, e) =>
        {
            if (SelectNodeCollection.Count == 2)
            {
                var nodes = SelectNodeCollection.GetFirst2();
                var node1 = nodes.Item1;
                var node2 = nodes.Item2;

                // TODO: Fix this mf.
                DrawManager.Draw(node1, node2);
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
            if (CollisionHelper.IsCircleCollide(Libraries.Geometry.Point.ConvertFrom(node.TopLeftPoint),
                                                Libraries.Geometry.Point.ConvertFrom(n.TopLeftPoint),
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