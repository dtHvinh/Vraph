using HamiltonVisualizer.Core.Collections;
using HamiltonVisualizer.Core.CustomControls.WPFBorder;
using HamiltonVisualizer.Core.CustomControls.WPFCanvas;
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
    private readonly AlgorithmPresentation _algorithm;
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
        _drawManager = new DrawManager(_canvas);
        _algorithm = new AlgorithmPresentation(roNodes, roEdges);

        SubscribeCollectionEvents();
        SubscribeCanvasEvents();
        SubscribeAlgorithmPresentingEvents();
        SubscribeCanvasContextMenuEvents();
        SubscribeGraphModeChangeEvents();
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

            Thực hiện các thuật toán bằng chuột phải lên các đỉnh hoặc khoảng trống
            trong vùng vẽ.

            Để vẽ các cạnh trong đồ thị:
             Cách 1: Chuột phải vào nút để bắt đầu chọn
             Cách 2: Nhấn chuột giữa

            Nếu đỉnh trong trạng thái chọn (nền màu xanh) nhấn chuột giữa để hủy chọn
            Lưu ý: Cứ mỗi 2 nút được chọn một cạnh của đồ thị sẽ được vẽ. 
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
        _canvas.Children.Clear();

        _viewModel.Refresh();
    }
    private void ResetButton_Click(object sender, RoutedEventArgs e)
    {
        _algorithm.ResetColor();
    }
    private void SkipTransition_Click(object sender, RoutedEventArgs e)
    {
        _viewModel.SkipTransition = !_viewModel.SkipTransition;
    }
    private void GraphMode_Click(object sender, RoutedEventArgs e)
    {
        _viewModel.IsDirectionalGraph = !_viewModel.IsDirectionalGraph;
    }

    //
    private void DrawingCanvas_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed && e.ClickCount == 2)
        {
            var mPos = e.GetPosition(_canvas);

            var node = new Node(_canvas,
                ObjectPosition.TryStayInBound(mPos),
                _elementCollection.Nodes);

            SubscribeNodeEvents(node);
            SubscribeNodeContextMenuEvents(node);

            _canvas.Children.Add(node);
            _elementCollection.Nodes.Add(node);
            _viewModel.Refresh();
        }
    }

    //
    private void SubscribeCanvasEvents()
    {
        _canvas.MouseDown += DrawingCanvas_MouseDown;

    }
    private void SubscribeCanvasContextMenuEvents()
    {
        ((DCContextMenu)_canvas.ContextMenu).SCC.Click += (sender, e) =>
        {
            _viewModel.DisplaySCC();
        };
    }
    private void SubscribeAlgorithmPresentingEvents()
    {
        _viewModel.PresentingTraversalAlgorithm += (sender, e) =>
        {
            _algorithm.SkipTransition = e.SkipTransition;

            _algorithm.PresentTraversalAlgorithm(e.Data);
        };

        _viewModel.PresentingSCCAlgorithm += (sender, e) =>
        {
            _algorithm.SkipTransition = e.SkipTransition;

            _algorithm.PresentComponentAlgorithm(e.Data);
        };
    }
    private void SubscribeNodeEvents(Node node)
    {
        // when node deleted
        node.OnNodeDelete += async (object sender, NodeDeleteEventArgs e) =>
        {
            if (!_viewModel.SkipTransition)
                await Task.Delay(500);
            _elementCollection.Remove(e.Node);
            _canvas.Children.Remove(e.Node);
            _selectedCollection.Remove(e.Node);
            _viewModel.Refresh();

            // remove associate _edge.
            e.Node.Adjacent.ForEach(e =>
            {
                _elementCollection.Remove(e.Edge);
                _canvas.Children.Remove(e.Edge);
                _viewModel.Refresh();
            });
        };

        // delete duplicate node when label existed
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
            _viewModel.Refresh();
            _algorithm.ResetColor();
        };

        // when release select on a mode
        node.OnNodeReleaseSelect += (object sender, NodeReleaseSelectEventArgs e) =>
        {
            _selectedCollection.Remove((Node)sender);
            _viewModel.Refresh();
        };
    }
    private void SubscribeNodeContextMenuEvents(Node node)
    {
        var nodeContextMenu = (NodeContextMenu)node.ContextMenu;

        nodeContextMenu.DFS.Click += (_, _) =>
        {
            _viewModel.DisplayDFS(node);
        };

        nodeContextMenu.BFS.Click += (_, _) =>
        {
            _viewModel.DisplayBFS(node);
        };
    }
    private void SubscribeGraphLineEvents(GraphLine graphLine)
    {
        graphLine.OnGraphLineDeleted += (sender, e) =>
        {
            var deleteLine = e.GraphLine;

            // TODO: line may have some kind of animation ???

            _elementCollection.Remove(deleteLine);
            _canvas.Children.Remove(deleteLine);
            _viewModel.Refresh();
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
                    && _drawManager.DrawLine(node1, node2, _viewModel.IsDirectionalGraph, out var edge))
                {
                    _elementCollection.Edges.Add(edge);
                    _viewModel.Refresh(edge);
                    SubscribeGraphLineEvents(edge);
                }
                node1.ReleaseSelectMode();
                node2.ReleaseSelectMode();
            }
        };
    }
    private void SubscribeGraphModeChangeEvents()
    {
        _viewModel.GraphModeChanged += (sender, e) =>
        {
            _algorithm.GraphModeChange();
            _algorithm.ResetColor();
        };
    }

    //
    private bool IsNodeAlreadyExist(string? nodeLabelContent)
    {
        return _elementCollection.Nodes.Count(e => e.NodeLabel.Text!.Equals(nodeLabelContent)) == 2;
    }

}
