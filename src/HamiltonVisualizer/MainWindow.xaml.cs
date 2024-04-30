using HamiltonVisualizer.Constants;
using HamiltonVisualizer.Core.Collections;
using HamiltonVisualizer.Core.CustomControls.WPFBorder;
using HamiltonVisualizer.Core.CustomControls.WPFCanvas;
using HamiltonVisualizer.Core.CustomControls.WPFLinePolygon;
using HamiltonVisualizer.Events.EventArgs;
using HamiltonVisualizer.Events.EventArgs.ForFeature.FeatureEventArgs;
using HamiltonVisualizer.Events.EventArgs.ForNode;
using HamiltonVisualizer.Extensions;
using HamiltonVisualizer.Utilities;
using HamiltonVisualizer.ViewModels;
using System.Windows;
using System.Windows.Input;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace HamiltonVisualizer;

public partial class MainWindow : Window
{
    internal readonly MainViewModel ViewModel = null!;
    internal readonly DrawManager DrawManager;
    internal readonly AlgorithmPresenter Algorithm;
    internal readonly GraphElementsCollection ElementCollection;
    internal readonly SelectedNodeCollection SelectedNodeCollection = new();
    internal readonly IOManager IOManager;
    internal readonly Feature Feature;

    //
    public MainWindow()
    {
        InitializeComponent();

        ElementCollection = new GraphElementsCollection();

        ViewModel = (MainViewModel)DataContext ?? throw new ArgumentNullException("Null");
        ViewModel.SetRefs(new RefBag(ElementCollection.Nodes.AsReadOnly(),
                                     ElementCollection.Edges.AsReadOnly(),
                                     SelectedNodeCollection));
        DrawManager = new DrawManager(_canvas);
        Algorithm = new AlgorithmPresenter(ElementCollection.Nodes, ElementCollection.Edges, options =>
        {
            options.EdgeTransition = 1500;
        });
        IOManager = new(this);
        Feature = new(this);

        SubscribeFeatureNotificationEvents();
        SubscribeFeatureEvents();
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
            
            Nhập dữ liệu từ tệp tin bằng cách kéo thả tệp vào vùng vẽ
            """, "Hướng dẫn", button: MessageBoxButton.OK);
    }

    //
    private void DeleteAll_Click(object sender, RoutedEventArgs e)
    {
        if (ActionGuard.ShouldContinue(ConstantValues.Messages.DeleteAllConfirmMessage))
        {
            DeleteAllCore();
        }
    }
    private void ResetButton_Click(object sender, RoutedEventArgs e)
    {
        Algorithm.Reset();
    }
    private void SkipTransition_Click(object sender, RoutedEventArgs e)
    {
        ViewModel.SkipTransitionSwitch();
    }
    private void GraphMode_Click(object sender, RoutedEventArgs e)
    {
        ViewModel.GraphModeSwitch();
    }

    //
    private void SubscribeFeatureNotificationEvents()
    {
        Feature.NotificationEventHandler += (_, e) =>
        {
            MessageBox.Show(e.Message, "Kết quả");
        };
    }
    private void SubscribeFeatureEvents()
    {
        Feature.FindEventHandler += (_, e) =>
        {
            var fne = (FindEventArgs)e;
            var labels = fne.Labels.Split(',');

            int c = 0;
            var nodes = ElementCollection.Nodes.Where(e =>
            {
                if (labels.Contains(e.NodeLabel.Text))
                {
                    c++;
                    return true;
                }
                return false;
            });

            foreach (var node in nodes)
            {
                node.OnSelectNode();
            }
            Feature.OnNotify(new NotificationEventArgs(c == 0 ? "Không tìm thấy đỉnh nào" : $"Đã tìm thấy {c} đỉnh!"));
        };
        Feature.DeleteEventHandler += (_, e) =>
        {
            var fne = (DeleteEventArgs)e;
            var labels = fne.Labels.Split(',');

            int c = 0;
            var nodes = ElementCollection.Nodes.Where(e =>
            {
                if (labels.Contains(e.NodeLabel.Text))
                {
                    c++;
                    return true;
                }
                return false;
            });

            foreach (var node in nodes)
            {
                node.DeleteNode();
            }
            Feature.OnNotify(new NotificationEventArgs(c == 0 ? "Không tìm thấy đỉnh nào" : $"Đã xóa {c} đỉnh!"));
        };

    }
    private void SubscribeCanvasEvents()
    {
        _canvas.MouseDown += (sender, e) =>
        {
            if (e.LeftButton == MouseButtonState.Pressed && e.ClickCount == 2)
            {
                Algorithm.Reset();
                CreateNodeAtPosition(e.GetPosition(_canvas));
            }
        };


    }
    private void SubscribeCanvasContextMenuEvents()
    {
        DCContextMenu canvasContextMenu = (DCContextMenu)_canvas.ContextMenu;

        canvasContextMenu.SCC.Click += (sender, e) =>
        {
            ViewModel.DisplaySCC();
        };
        canvasContextMenu.HamiltonCycle.Click += (sender, e) =>
        {
            ViewModel.DisplayHamiltonCycle();
        };
        canvasContextMenu.CSVExport.Click += (sender, e) =>
        {
            IOManager.SaveFileCore();
        };
        canvasContextMenu.CSVImport.Click += (sender, e) =>
        {
            IOManager.OpenFileCore();
        };
    }
    private void SubscribeAlgorithmPresentingEvents()
    {
        ViewModel.PresentingTraversalAlgorithm += async (sender, e) =>
        {
            Algorithm.SkipTransition = e.SkipTransition;

            switch (e.Name)
            {
                case ConstantValues.AlgorithmNames.DFS:
                    await Algorithm.PresentDFSAlgorithmAsync(e.Data);
                    break;
                case ConstantValues.AlgorithmNames.Hamilton:
                    await Algorithm.PresentHamiltonianCycleAlgorithmAsync(e.Data);
                    break;
            }
        };

        ViewModel.PresentingSCCAlgorithm += async (sender, e) =>
        {
            Algorithm.SkipTransition = e.SkipTransition;
            await Algorithm.PresentComponentAlgorithm(e.Data);
        };

        ViewModel.PresentingLayeredBFSAlgorithm += async (sender, e) =>
        {
            await Algorithm.PresentLayeredBFSAlgorithm(e.Data);
        };
    }
    private void SubscribeNodeEvents(Node node)
    {
        // when node deleted
        node.NodeDelete += async (object sender, NodeDeleteEventArgs e) =>
        {
            await DeleteNodeCore(node, !ViewModel.SkipTransition);
        };

        // delete duplicate node when label existed
        node.NodeLabelChanged += (object sender, NodeSetLabelEventArgs e) =>
        {
            var text = e.Text;

            if (IsNodeLabelAlreadyExist(text))
            {
                e.Node.DeleteNode();
            }
        };

        // when at select mode
        node.NodeSelected += (object sender, NodeSelectedEventArgs e) =>
        {
            SelectedNodeCollection.Add((Node)sender);
            ViewModel.Refresh();
            Algorithm.Reset();
        };

        // when release select on a mode
        node.NodeReleaseSelect += (object sender, NodeReleaseSelectEventArgs e) =>
        {
            SelectedNodeCollection.Remove((Node)sender);
            ViewModel.Refresh();
        };
    }
    private void SubscribeNodeContextMenuEvents(Node node)
    {
        var nodeContextMenu = (NodeContextMenu)node.ContextMenu;

        nodeContextMenu.DFS.Click += (_, _) =>
        {
            ViewModel.DisplayDFS(node);
        };

        nodeContextMenu.BFS.Click += (_, _) =>
        {
            ViewModel.DisplayBFS(node);
        };
    }
    private void SubscribeGraphLineEvents(GraphLine graphLine)
    {
        graphLine.OnGraphLineDeleted += (sender, e) =>
        {
            var deleteLine = e.GraphLine;

            ElementCollection.Remove(deleteLine);
            _canvas.Children.Remove(deleteLine);
            ViewModel.Refresh();
        };
    }
    private void SubscribeCollectionEvents()
    {
        SelectedNodeCollection.PropertyChanged += (sender, e) =>
        {
            // whenever 2 node selected, connect them.
            if (SelectedNodeCollection.Count == 2)
            {
                var nodes = SelectedNodeCollection.GetFirst2();
                var node1 = nodes.Item1;
                var node2 = nodes.Item2;

                CreateLine(node1, node2);

                node1.OnReleaseSelectMode();
                node2.OnReleaseSelectMode();
            }
        };
    }
    private void SubscribeGraphModeChangeEvents()
    {
        ViewModel.GraphModeChanged += (sender, e) =>
        {
            Algorithm.GraphTypeSwitch();
            Algorithm.Reset();
        };
    }

    //
    private bool IsNodeLabelAlreadyExist(string? nodeLabelContent)
    {
        return ElementCollection.Nodes.Count(e => e.NodeLabel.Text!.Equals(nodeLabelContent)) == 2;
    }
    private static bool IsLineAlreadyExist(Line line, Node from, Node to, bool directed = false)
    {
        bool sameDirCompare = line.X1 == from.Origin.X
         && line.Y1 == from.Origin.Y
         && line.X2 == to.Origin.X
         && line.Y2 == to.Origin.Y;

        if (directed)
            return sameDirCompare;

        bool opositeDirCompare = line.X1 == to.Origin.X
                && line.Y1 == to.Origin.Y
                && line.X2 == from.Origin.X
                && line.Y2 == from.Origin.Y;

        return sameDirCompare || opositeDirCompare;
    }

    //
    internal void DeleteAllCore()
    {
        ElementCollection.ClearAll();
        SelectedNodeCollection.Nodes.Clear();
        _canvas.Children.Clear();

        ViewModel.Clear();
    }
    internal async Task DeleteNodeCore(Node node, bool delay)
    {
        if (delay) await Task.Delay(500);

        ElementCollection.Remove(node);
        _canvas.Children.Remove(node);
        SelectedNodeCollection.Remove(node);
        ViewModel.RefreshWhenRemove(node);

        // remove associate _edge.
        node.Adjacent.ForEach(e =>
        {
            ElementCollection.Remove(e.Edge);
            _canvas.Children.Remove(e.Edge);
            e.Edge.DeleteFrom(node);
            ViewModel.Refresh();
        });

    }

    internal void CreateNode(Node node)
    {
        SubscribeNodeEvents(node);
        SubscribeNodeContextMenuEvents(node);

        _canvas.Children.Add(node);
        ElementCollection.Add(node);
        ViewModel.RefreshWhenAdd(node);
    }
    internal void CreateNodeAtPosition(Point position)
    {
        var node = new Node(_canvas, position, ElementCollection.Nodes);

        if (node.PhysicManager.HasNoCollide())
        {
            SubscribeNodeEvents(node);
            SubscribeNodeContextMenuEvents(node);

            _canvas.Children.Add(node);
            ElementCollection.Add(node);
            ViewModel.RefreshWhenAdd(node);
        }
    }
    internal void CreateLine(Node from, Node to)
    {
        if (!ElementCollection.Edges.Any(e => IsLineAlreadyExist(e.Body, from, to, ViewModel.IsDirectionalGraph))
            && DrawManager.DrawLine(from, to, ViewModel.IsDirectionalGraph, out var edge))
        {
            ElementCollection.Edges.Add(edge);
            ViewModel.RefreshWhenAdd(edge);
            SubscribeGraphLineEvents(edge);
        }
    }
}
