using HamiltonVisualizer.Core;
using HamiltonVisualizer.Core.CustomControls.WPFBorder;
using HamiltonVisualizer.Core.CustomControls.WPFCanvas;
using HamiltonVisualizer.Core.CustomControls.WPFLinePolygon;
using HamiltonVisualizer.Events.EventArgs;
using HamiltonVisualizer.Extensions;
using HamiltonVisualizer.ViewModels;
using System.Windows;
using System.Windows.Input;
using System.Windows.Shapes;

namespace HamiltonVisualizer;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly MainViewModel _viewModel = null!;
    private readonly SelectedNodeCollection _selectedCollection = new();
    private readonly DrawManager _drawManager;

    public List<Node> Nodes { get; set; } = [];
    private List<LinePolygonWrapper> Edges { get; set; } = [];

    public bool IsSelectMode { get; set; } = false;

    public MainWindow()
    {
        InitializeComponent();

        _viewModel = (MainViewModel)DataContext ?? throw new ArgumentNullException("Null");
        _viewModel.ProvideRef(new RefBag(Nodes, Edges, _selectedCollection));
        _drawManager = new DrawManager(DrawingCanvas);

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

    #region Events

    private void InstructionButton_Click(object sender, RoutedEventArgs e)
    {
        MessageBox.Show(
            """
            Nhấn chuột trái vào vùng có màu đậm hơn để vẽ

            Sau khi nhập giá trị của nhãn nhấn Enter để hoàn thành

            Thực hiện các thuật toán bằng chuột phải lên các Node hoặc khoảng trống
            trong vùng vẽ.
            """, "Hướng dẫn", button: MessageBoxButton.OK);
    }

    #endregion Events

    #region Drawing

    private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed)
        {
            var mPos = e.GetPosition(DrawingCanvas);

            Node node = new(mPos);

            SubscribeNodeEvents(node);

            if (EnsureNoCollision(node))
            {
                DrawingCanvas.Children.Add(node);
                Nodes.Add(node);
                _viewModel.VM_NodeAdded();
            }
        }
    }

    #endregion Drawing

    #region Subcribe Event

    private void SubscribeNodeEvents(Node node)
    {
        // when node deleted
        node.OnNodeDelete += async (object sender, NodeDeleteEventArgs e) =>
        {
            // invoke delete animation 
            await Task.Delay(500);
            Nodes.Remove(e.Node);
            DrawingCanvas.Children.Remove(e.Node);
            _selectedCollection.Remove(e.Node);
            _viewModel.VM_NodeRemoved(e.Node, out var pendingRemoveEdge);

            // remove associate edge.
            pendingRemoveEdge.ForEach(e =>
            {
                Edges.Remove(e);
                DrawingCanvas.Children.Remove(e);
                _viewModel.VM_EdgeRemoved();
            });
        };

        // delete duplicate node with identical label
        node.OnNodeLabelChanged += (object sender, NodeSetLabelEventArgs e) =>
        {
            var text = e.Text;

            if (IsNodeAlreadyExist(text))
            {
                e.Node.DeleteNode();
            }
        };

        // when at select mode
        node.OnNodeSelected += (object sender, NodeSelectedEventArgs e) =>
        {
            _selectedCollection.Add((Node)sender);
            _viewModel.VM_NodeSelectedOrRelease();
        };

        // when release select on a mode
        node.OnNodeReleaseSelect += (object sender, NodeReleaseSelectEventArgs e) =>
        {
            _selectedCollection.Remove((Node)sender);
            _viewModel.VM_NodeSelectedOrRelease();
        };
    }

    private void SubscribeCollectionEvents()
    {
        static bool AreTheSameLine(Line line, Node node1, Node node2, bool directed = false)
        {
            bool sameDirCompare = line.X1 == node1.Origin.X
                    && line.Y1 == node1.Origin.Y
                    && line.X2 == node2.Origin.X
                    && line.Y2 == node2.Origin.Y;

            if (directed)
                return sameDirCompare;

            bool opositeDirCompare = line.X1 == node2.Origin.X
                    && line.Y1 == node2.Origin.Y
                    && line.X2 == node1.Origin.X
                    && line.Y2 == node1.Origin.Y;

            return sameDirCompare || opositeDirCompare;
        }

        _selectedCollection.PropertyChanged += (sender, e) =>
        {
            // whenever 2 node selected, connect them.
            if (_selectedCollection.Count == 2)
            {
                var nodes = _selectedCollection.GetFirst2();
                var node1 = nodes.Item1;
                var node2 = nodes.Item2;

                if (!Edges.Any(e => AreTheSameLine(e.Body, node1, node2))
                    && _drawManager.Draw(node1, node2, out var edge))
                {
                    Edges.Add(edge);
                    _viewModel.VM_EdgeAdded(edge);
                }
                node1.ReleaseSelectMode();
                node2.ReleaseSelectMode();
            }
        };
    }

    #endregion Subcribe Event

    #region Helper class

    private bool IsNodeAlreadyExist(string? nodeLabelContent)
    {
        return Nodes.Count(e => e.NodeLabel.Text!.Equals(nodeLabelContent)) == 2;
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
