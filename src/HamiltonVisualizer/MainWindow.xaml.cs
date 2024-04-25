using HamiltonVisualizer.Constants;
using HamiltonVisualizer.Core.Collections;
using HamiltonVisualizer.Core.CustomControls.WPFBorder;
using HamiltonVisualizer.Core.CustomControls.WPFCanvas;
using HamiltonVisualizer.Core.CustomControls.WPFLinePolygon;
using HamiltonVisualizer.Core.Functionality;
using HamiltonVisualizer.Events.EventArgs.ForNode;
using HamiltonVisualizer.Extensions;
using HamiltonVisualizer.Utilities;
using HamiltonVisualizer.ViewModels;
using Microsoft.Win32;
using System.Windows;
using System.Windows.Input;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace HamiltonVisualizer;

public partial class MainWindow : Window
{
    internal readonly MainViewModel _viewModel = null!;
    internal readonly SelectedNodeCollection _selectedCollection = new();
    internal readonly DrawManager _drawManager;
    internal readonly AlgorithmPresenter _algorithm;
    internal readonly GraphElementsCollection _elementCollection;
    internal readonly SaveFileDialog _saveFileDialog;
    internal readonly OpenFileDialog _openFileDialog;

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
        _algorithm = new AlgorithmPresenter(_elementCollection.Nodes, _elementCollection.Edges, options =>
        {
            options.EdgeTransition = 1500;
        });

        _saveFileDialog = new()
        {
            FileName = "Graph",
            DefaultExt = ".csv",
            Filter = "Text documents (.csv)|*.csv",
            OverwritePrompt = true,
            AddExtension = true,
            AddToRecent = true,
        };

        _openFileDialog = new()
        {
            AddExtension = true,
        };

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
        _algorithm.ResetColor();
    }
    private void SkipTransition_Click(object sender, RoutedEventArgs e)
    {
        _viewModel.SkipTransitionSwitch();
    }
    private void GraphMode_Click(object sender, RoutedEventArgs e)
    {
        _viewModel.GraphModeSwitch();
    }

    //
    private void SubscribeCanvasEvents()
    {
        _canvas.MouseDown += (sender, e) =>
        {
            if (e.LeftButton == MouseButtonState.Pressed && e.ClickCount == 2)
            {
                _algorithm.ResetColor();
                CreateNodeAtPosition(e.GetPosition(_canvas));
            }
        };

        _canvas.Drop += (sender, e) =>
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] dropFiles = (string[])e.Data.GetData(DataFormats.FileDrop, true);
                if (_elementCollection.Nodes.Any())
                {

                    if (ActionGuard.BeforeImport(out bool needSaveFile))
                    {
                        if (needSaveFile)
                        {
                            SaveFileCore();
                        }
                        DeleteAllCore();
                        ReadFileCore(dropFiles[0]);
                    }
                }
                else
                    ReadFileCore(dropFiles[0]);
            }
        };
    }
    private void SubscribeCanvasContextMenuEvents()
    {
        DCContextMenu canvasContextMenu = (DCContextMenu)_canvas.ContextMenu;

        canvasContextMenu.SCC.Click += (sender, e) =>
        {
            _viewModel.DisplaySCC();
        };
        canvasContextMenu.HamiltonCycle.Click += (sender, e) =>
        {
            _viewModel.DisplayHamiltonCycle();
        };
        canvasContextMenu.CSVExport.Click += (sender, e) =>
        {
            SaveFileCore();
        };
        canvasContextMenu.CSVImport.Click += (sender, e) =>
        {
            var result = _openFileDialog.ShowDialog();
            if (_elementCollection.Nodes.Any())
            {
                if (ActionGuard.BeforeImport(out bool needSaveFile))
                {
                    if (needSaveFile)
                    {
                        SaveFileCore();
                    }
                    DeleteAllCore();
                    ReadFileCore(_openFileDialog.FileName);
                }
            }
            else
                ReadFileCore(_openFileDialog.FileName);
        };
    }
    private void SubscribeAlgorithmPresentingEvents()
    {
        _viewModel.PresentingTraversalAlgorithm += async (sender, e) =>
        {
            _algorithm.SkipTransition = e.SkipTransition;

            switch (e.Name)
            {
                case ConstantValues.AlgorithmNames.DFS:
                    await _algorithm.PresentDFSAlgorithm(e.Data);
                    break;
                case ConstantValues.AlgorithmNames.Hamilton:
                    await _algorithm.PresentHamiltonianCycleAlgorithm(e.Data);
                    break;
            }
        };

        _viewModel.PresentingSCCAlgorithm += async (sender, e) =>
        {
            _algorithm.SkipTransition = e.SkipTransition;

            await _algorithm.PresentComponentAlgorithm(e.Data);
        };

        _viewModel.PresentingLayeredBFSAlgorithm += async (sender, e) =>
        {
            await _algorithm.PresentLayeredBFSAlgorithm(e.Data);
        };
    }
    private void SubscribeNodeEvents(Node node)
    {
        // when node deleted
        node.NodeDelete += async (object sender, NodeDeleteEventArgs e) =>
        {
            if (!_viewModel.SkipTransition)
                await Task.Delay(500);
            DeleteNodeCore(node);
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
            _selectedCollection.Add((Node)sender);
            _viewModel.Refresh();
            _algorithm.ResetColor();
        };

        // when release select on a mode
        node.NodeReleaseSelect += (object sender, NodeReleaseSelectEventArgs e) =>
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

            _elementCollection.Remove(deleteLine);
            _canvas.Children.Remove(deleteLine);
            _viewModel.Refresh();
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

                CreateLine(node1, node2);

                node1.OnReleaseSelectMode();
                node2.OnReleaseSelectMode();
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
    private bool IsNodeLabelAlreadyExist(string? nodeLabelContent)
    {
        return _elementCollection.Nodes.Count(e => e.NodeLabel.Text!.Equals(nodeLabelContent)) == 2;
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
    private void ReadFileCore(string path)
    {
        FileImporter.ReadFrom(this, path);
    }
    private void SaveFileCore()
    {
        _saveFileDialog.ShowDialog();
        FileExporter.WriteTo(_saveFileDialog.FileName, _elementCollection);
    }
    private void DeleteAllCore()
    {
        _elementCollection.ClearAll();
        _selectedCollection.Nodes.Clear();
        _canvas.Children.Clear();

        _viewModel.Clear();
    }
    private void DeleteNodeCore(Node node)
    {
        _elementCollection.Remove(node);
        _canvas.Children.Remove(node);
        _selectedCollection.Remove(node);
        _viewModel.RefreshWhenRemove(node);

        // remove associate _edge.
        node.Adjacent.ForEach(e =>
        {
            _elementCollection.Remove(e.Edge);
            _canvas.Children.Remove(e.Edge);
            e.Edge.DeleteFrom(node);
            _viewModel.Refresh();
        });

    }

    public void CreateNode(Node node)
    {
        SubscribeNodeEvents(node);
        SubscribeNodeContextMenuEvents(node);

        _canvas.Children.Add(node);
        _elementCollection.Add(node);
        _viewModel.RefreshWhenAdd(node);
    }
    public void CreateNodeAtPosition(Point position)
    {
        var node = new Node(_canvas, position, _elementCollection.Nodes);

        if (node.PhysicManager.HasNoCollide())
        {
            SubscribeNodeEvents(node);
            SubscribeNodeContextMenuEvents(node);

            _canvas.Children.Add(node);
            _elementCollection.Add(node);
            _viewModel.RefreshWhenAdd(node);
        }
    }
    public void CreateLine(Node from, Node to)
    {
        if (!_elementCollection.Edges.Any(e => IsLineAlreadyExist(e.Body, from, to, _viewModel.IsDirectionalGraph))
            && _drawManager.DrawLine(from, to, _viewModel.IsDirectionalGraph, out var edge))
        {
            _elementCollection.Edges.Add(edge);
            _viewModel.RefreshWhenAdd(edge);
            SubscribeGraphLineEvents(edge);
        }
    }
}
