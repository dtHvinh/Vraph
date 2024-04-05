using HamiltonVisualizer.Constants;
using HamiltonVisualizer.Core.Collections;
using HamiltonVisualizer.Core.CustomControls.WPFBorder;
using HamiltonVisualizer.Core.CustomControls.WPFLinePolygon;
using HamiltonVisualizer.Core.Functions;
using HamiltonVisualizer.Events.EventArgs.NodeEventArg;
using HamiltonVisualizer.Extensions;
using HamiltonVisualizer.Utilities;
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
    private readonly ObjectVisualizationManager _visualAppearanceManager;
    private readonly GraphElementsCollection _elementCollection;

    //
    public MainWindow()
    {
        InitializeComponent();

        _elementCollection = new GraphElementsCollection();

        var roNodes = _elementCollection.Nodes.AsReadOnly();
        var roEdges = _elementCollection.Edges.AsReadOnly();

        _viewModel = (MainViewModel)DataContext ?? throw new ArgumentNullException("Null");
        _viewModel.SetRefs(new RefBag(roNodes, roEdges, _selectedCollection));
        _drawManager = new DrawManager(DrawingCanvas);
        _visualAppearanceManager = new ObjectVisualizationManager(roNodes, roEdges);

        SubscribeCollectionEvents();
        SubscribeModelViewEvents();
        SubscribeCanvasEvents();
        SubscribeViewModelEvents();
    }

    //
    private void Border_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed)
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
    private void InstructionOnTaskbar(object sender, MouseButtonEventArgs e)
    {
        MessageBox.Show(
            """
            Nhấn chuột trái 2 lần vào vùng có màu đậm hơn để vẽ

            Sau khi nhập giá trị của nhãn nhấn Enter để hoàn thành

            Thực hiện các thuật toán bằng chuột phải lên các Node hoặc khoảng trống
            trong vùng vẽ.

            Để vẽ các cạnh trong đồ thì chuột phải vào nút để bắt đầu chọn
            Cứ mỗi 2 nút được chọn một cạnh của đồ thị sẽ được vẽ. 
            """, "Hướng dẫn", button: MessageBoxButton.OK);
    }

    //
    private void DeleteAll_Click(object sender, RoutedEventArgs e)
    {
        var result = MessageBox.Show("Xác nhận xóa tất cả", "Cảnh báo", MessageBoxButton.OKCancel);

        if (result == MessageBoxResult.Cancel)
            return;

        _elementCollection.ClearAll();
        _selectedCollection.Nodes.Clear();
        DrawingCanvas.Children.Clear();

        _viewModel.Refresh();
    }
    private void ResetButton_Click(object sender, RoutedEventArgs e)
    {
        _visualAppearanceManager.ResetColor();
    }
    private void SkipTransition_Click(object sender, RoutedEventArgs e)
    {
        _viewModel.SkipTransition = !_viewModel.SkipTransition;
    }

    //
    private void DrawingCanvas_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed && e.ClickCount == 2)
        {
            var mPos = e.GetPosition(DrawingCanvas);

            var node = new Node(DrawingCanvas,
                                ObjectPosition.GetValidPosition(mPos),
                                _elementCollection.Nodes);

            SubscribeNodeEvents(node);
            SubscribeNodeContextMenuEvents(node);

            if (node.Physic.IsNoCollide())
            {
                DrawingCanvas.Children.Add(node);
                _elementCollection.Nodes.Add(node);
                _viewModel.VM_NodeAdded();
            }
        }
    }

    //
    private void SubscribeCanvasEvents()
    {
        DrawingCanvas.MouseDown += DrawingCanvas_MouseDown;
    }
    private void SubscribeViewModelEvents()
    {
        _viewModel.OnPresentingAlgorithm += (sender, e) =>
        {
            _visualAppearanceManager.IsModified = true;
        };
    }
    private void SubscribeNodeEvents(Node node)
    {
        // when node moving, prevent canvas to listen to click event
        node.OnNodeStateChanged += (sender, e) =>
        {
            switch (e.State)
            {
                case NodeState.Idle:
                    DrawingCanvas.MouseDown += DrawingCanvas_MouseDown;
                    break;

                case NodeState.Moving:
                    DrawingCanvas.MouseDown -= DrawingCanvas_MouseDown;
                    break;
            }
        };

        // when node deleted
        node.OnNodeDelete += async (object sender, NodeDeleteEventArgs e) =>
        {
            if (!_viewModel.SkipTransition)
                await Task.Delay(500);
            _elementCollection.Remove(e.Node);
            DrawingCanvas.Children.Remove(e.Node);
            _selectedCollection.Remove(e.Node);
            _viewModel.VM_NodeRemoved(e.Node, out var pendingRemoveEdge);

            // remove associate _edge.
            pendingRemoveEdge.ForEach(e =>
            {
                _elementCollection.Remove(e.Edge);
                DrawingCanvas.Children.Remove(e.Edge);
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
    private void SubscribeNodeContextMenuEvents(Node node)
    {
        var nodeContextMenu = (NodeContextMenu)node.ContextMenu;

        nodeContextMenu.DFS.Click += (_, _) =>
        {
            _viewModel.DFS(node);
        };

        nodeContextMenu.BFS.Click += (_, _) =>
        {
            _viewModel.BFS(node);
        };
    }
    private void SubscribeModelViewEvents()
    {
        _viewModel.OnPresentingAlgorithm += async (sender, e) =>
        {
            if (e.SkipTransition)
                _visualAppearanceManager.ColorizeNodes(
                    (IEnumerable<Node>)e.Data!,
                    ConstantValues.ControlColors.NodeTraversalBackground);
            else
                await _visualAppearanceManager.ColorizeNodes(
                    (IEnumerable<Node>)e.Data!,
                    ConstantValues.ControlColors.NodeTraversalBackground, 500);
        };
    }
    private void SubscribeGraphLineEvents(GraphLine graphLine)
    {
        graphLine.OnGraphLineDeleted += (sender, e) =>
        {
            var deleteLine = e.GraphLine;

            // TODO: line may have some kind of animation ???

            _elementCollection.Remove(deleteLine);
            DrawingCanvas.Children.Remove(deleteLine);
            _viewModel.VM_EdgeRemoved();
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

                if (!_elementCollection.Edges.Any(e => AreTheSameLine(e.Body, node1, node2))
                    && _drawManager.Draw(node1, node2, out var edge))
                {
                    _elementCollection.Edges.Add(edge);
                    _viewModel.VM_EdgeAdded(edge);
                    SubscribeGraphLineEvents(edge);
                }
                node1.ReleaseSelectMode();
                node2.ReleaseSelectMode();
            }
        };
    }

    //
    private bool IsNodeAlreadyExist(string? nodeLabelContent)
    {
        return _elementCollection.Nodes.Count(e => e.NodeLabel.Text!.Equals(nodeLabelContent)) == 2;
    }
}
