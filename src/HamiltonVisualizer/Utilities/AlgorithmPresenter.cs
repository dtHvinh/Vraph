using HamiltonVisualizer.Constants;
using HamiltonVisualizer.Core.CustomControls.WPFBorder;
using HamiltonVisualizer.Core.CustomControls.WPFLinePolygon;
using HamiltonVisualizer.DataStructure.Components;
using HamiltonVisualizer.Options;
using System.Windows;
using System.Windows.Media;

namespace HamiltonVisualizer.Utilities;

internal sealed class AlgorithmPresenter(List<Node> nodes, List<GraphLine> lines)
{
    private bool _isModified = false; // value indicate if reset actually need to be perform
    private readonly List<Node> _nodes = nodes;
    private readonly List<GraphLine> _linePolygons = lines;

    public CancellationTokenSource CancellationTokenSource { get; private set; } = new CancellationTokenSource();
    public bool IsDirectedGraph { get; set; } = true;
    public bool SkipTransition { get; set; } = false;
    public int NodeTransition = ConstantValues.Time.NodeTransitionDefault;
    public int EdgeTransition = ConstantValues.Time.NodeTransitionDefault;
    public SolidColorBrush ColorizedNode = ConstantValues.ControlColors.NodeTraversalColor;
    public SolidColorBrush ColorizedLine = ConstantValues.ControlColors.LineTraversalColor;

    public AlgorithmPresenter(List<Node> nodes,
                              List<GraphLine> graphLines,
                              Action<AlgorithmPresenterOptions> configureOptions)
        : this(nodes, graphLines)
    {
        var options = new AlgorithmPresenterOptions();
        configureOptions?.Invoke(options);
        this.ProcessOptions(options);
    }

    private void GraphLineArrowVisibilityChange()
    {
        foreach (var line in _linePolygons)
        {
            switch (line.Head.Visibility)
            {
                case Visibility.Visible:
                    line.Head.Visibility = Visibility.Collapsed;
                    break;
                case Visibility.Collapsed:
                    line.Head.Visibility = Visibility.Visible;
                    break;
            }
        }
    }

    private void ResetLinesColor()
    {
        foreach (var line in lines)
        {
            line.ResetColor();
        }
    }

    private void BeforePresenting()
    {
        ResetOrCancel();
        ColorPalate.Reset();
        _isModified = true;
    }

    private void FinishedPresenting()
    {
        MessageBox.Show("Hoàn Thành");
    }

    private void HandlingException()
    {
        MessageBox.Show("Kết thúc");
    }

    public async Task PresentDFSAlgorithmAsync(IEnumerable<Node> nodes)
    {
        await AlgorithmPresenterBuilder.CreateBuilder()
            .BeforeStart(BeforePresenting)
            .ForEachNode(nodes)
            .WithColor(ColorizedNode)
            .ConfigureDelayAtStart(false)
            .ConfigureTransition(SkipTransition ? 0 : NodeTransition)
            .WithCancellationToken(CancellationTokenSource.Token)
            .IfFinished(FinishedPresenting)
            .IfThrown(HandlingException)
            .BuildDFS();
    }
    public async Task PresentHamiltonianCycleAlgorithmAsync(IEnumerable<Node> nodes)
    {
        await AlgorithmPresenterBuilder.CreateBuilder()
            .BeforeStart(BeforePresenting)
            .ForEachNode(nodes)
            .ForEachLine(_linePolygons)
            .WithColor(ColorizedNode)
            .WithGraph(IsDirectedGraph)
            .ConfigureDelayAtStart(false)
            .ConfigureTransition(SkipTransition ? 0 : NodeTransition)
            .WithCancellationToken(CancellationTokenSource.Token)
            .IfFinished(FinishedPresenting)
            .IfThrown(HandlingException)
            .BuildHC();
    }
    public async Task PresentComponentAlgorithm(IEnumerable<IEnumerable<Node>> components)
    {

        //    foreach (var component in components)
        //    {
        //        var color = ColorPalate.GetUnusedColor();
        //        await ColorizeNodesAsync(component, ColorizedNode, 0, false, CancellationTokenSource.Token);
        //    }

        //    foreach (var node in _nodes.Where(e => e.Adjacent.Count == 0))
        //    {
        //        var color = ColorPalate.GetUnusedColor();
        //        CancellationTokenSource.Token.ThrowIfCancellationRequested();
        //        ColorizeNode(node, ColorizedNode, CancellationTokenSource.Token);
        //    }

        await AlgorithmPresenterBuilder.CreateBuilder()
            .BeforeStart(BeforePresenting)
            .ForEachNode(_nodes)
            .ForEachComponent(components)
            .ConfigureDelayAtStart(false)
            .ConfigureTransition(SkipTransition ? 0 : NodeTransition)
            .WithCancellationToken(CancellationTokenSource.Token)
            .IfFinished(FinishedPresenting)
            .IfThrown(HandlingException)
            .BuildComponents();
    }
    public async Task PresentLayeredBFSAlgorithm(IEnumerable<BFSComponent<Node>> layeredNode)
    {
        await AlgorithmPresenterBuilder.CreateBuilder()
            .BeforeStart(BeforePresenting)
            .ForEachBFSComponent(layeredNode)
            .ForEachNode(nodes)
            .ForEachLine(_linePolygons)
            .WithColor(ColorizedNode)
            .ConfigureTransition(SkipTransition ? 0 : NodeTransition)
            .WithCancellationToken(CancellationTokenSource.Token)
            .IfFinished(FinishedPresenting)
            .IfThrown(HandlingException)
            .BuildBFS();
    }

    public void GraphTypeSwitch()
    {
        IsDirectedGraph = !IsDirectedGraph;
        GraphLineArrowVisibilityChange();
    }
    public void ResetOrCancel()
    {
        if (_isModified)
        {
            CancellationTokenSource.Cancel();
            CancellationTokenSource.Dispose();
            foreach (Node node in _nodes)
            {
                node.Background = ConstantValues.ControlColors.NodeDefaultColor;
            }
            ResetLinesColor();
            _isModified = false;
            CancellationTokenSource = new();
        }
    }
}

static class BFSComponentProcesser
{
    public static IEnumerable<GraphLine> GetLines(IEnumerable<GraphLine> source, BFSComponent<Node> component, bool isDirectedGraph)
    {
        List<GraphLine> lines = [];

        if (isDirectedGraph)
        {
            foreach (var child in component.Children)
            {
                var line = source.FirstOrDefault(l => l.From.Equals(component.Root()) && l.To.Equals(child));
                if (line != null)
                    lines.Add(line);
            }
        }
        else // !isDirectedGraph
        {
            foreach (var child in component.Children)
            {
                var line = source.FirstOrDefault(
                        l => l.From.Equals(component.Root()) && l.To.Equals(child)
                        || l.From.Equals(child) && l.To.Equals(component.Root()));
                if (line != null)
                    lines.Add(line);
            }
        }

        return lines;
    }
}

internal static class AlgorithmPresenterExtensions
{
    internal static void ProcessOptions(this AlgorithmPresenter presenter, AlgorithmPresenterOptions options)
    {
        presenter.IsDirectedGraph = options.IsDirectedGraph;
        presenter.SkipTransition = options.SkipTransition;
        presenter.NodeTransition = options.NodeTransition;
        presenter.EdgeTransition = options.EdgeTransition;
        presenter.ColorizedNode = options.ColorizedNode;
        presenter.ColorizedLine = options.ColorizedLine;
    }
}
