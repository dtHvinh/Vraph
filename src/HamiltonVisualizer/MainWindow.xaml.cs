using HamiltonVisualizer.Core;
using HamiltonVisualizer.Events.EventArgs;
using HamiltonVisualizer.GraphUIComponents;
using HamiltonVisualizer.ViewModels;
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
    private readonly MainViewModel _viewModel = null!;
    private readonly AnimationManager _animationManager;
    private readonly SelectNodeCollection _selectedCollection = new();
    private readonly DrawManager _drawManager;

    public List<Node> Nodes { get; set; } = [];
    private List<Line> Edges { get; set; } = [];

    public bool IsSelectMode { get; set; } = false;

    public MainWindow()
    {
        InitializeComponent();

        _viewModel = (MainViewModel)DataContext ?? throw new ArgumentNullException("Null");
        _viewModel.ProvideRef(Nodes, Edges);

        _animationManager = AnimationManager.Instance;

        _drawManager = new(DrawingCanvas);

        SubscribeCollectionEvents();
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
            btn.BeginStoryboard(_animationManager.StoryboardWhenOn);
            btn.Background = Brushes.LightGreen;
            btn.Margin = _animationManager.ModeButtonOn;
        }
        else
        {
            btn.BeginStoryboard(_animationManager.StoryboardWhenOff);
            btn.Background = Brushes.Gray;
            btn.Margin = _animationManager.ModeButtonOff;
        }
    }

    private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed)
        {
            var mPos = e.GetPosition(DrawingCanvas);

            Node node = new(mPos);

            SubscribeNodeEvents(node);

            if (this.EnsureNoCollision(node))
            {
                AddToCanvas(node);
                _viewModel.VM_AddNewNode();
            }
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
        node.OnNodeDelete += async (object sender, NodeDeleteEventArgs e) =>
        {
            Nodes.Remove(e.Node);
            _viewModel.VM_RemoveNode();

            // invoke delete animation 
            await Task.Delay(500);
            DrawingCanvas.Children.Remove(e.Node);

            // remove associate edge.
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
            _selectedCollection.Add((Node)sender);
        };

        node.NodeLabel.OnLabelMouseDown += (object sender, MouseEventArgs e) =>
        {
            if (IsSelectMode)
            {
                var nodeLabel = (NodeLabel)sender;
                nodeLabel.Node.SelectNode();
            }
        };
    }

    private void SubscribeCollectionEvents()
    {
        _selectedCollection.PropertyChanged += (sender, e) =>
        {
            // whenever 2 node selected, connect them.
            if (_selectedCollection.Count == 2)
            {
                var nodes = _selectedCollection.GetFirst2();
                var node1 = nodes.Item1;
                var node2 = nodes.Item2;

                if (_drawManager.Draw(node1, node2, out var edge))
                {
                    Edges.Add(edge);
                }
                _viewModel.VM_AddNewEdge(node1, node2);
            }
        };
    }

    #endregion Subcribe Event

    #region Helper class

    /// <summary>
    /// Add Graph node to the view ui.
    /// </summary>
    /// <param name="node"></param>
    private void AddToCanvas(Node node)
    {
        DrawingCanvas.Children.Add(node);
        Nodes.Add(node);
    }

    private bool EnsureNoCollision(Node node)
    {
        if (Nodes.Count == 0)
            return true;

        foreach (Node n in Nodes)
        {
            if (CSLibraries.Mathematic.Geometry.CollisionHelper.IsCircleCollide(
                                                CSLibraries.Mathematic.Geometry.MathPoint.ConvertFrom(node.Origin),
                                                CSLibraries.Mathematic.Geometry.MathPoint.ConvertFrom(n.Origin),
                                                Node.NodeWidth / 2))
            {
                return false;
            }
        }

        return true;
    }

    #endregion Helper class
}
