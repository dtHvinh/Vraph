using HamiltonVisualizer.Constants;
using HamiltonVisualizer.Core.CustomControls.WPFBorder;
using HamiltonVisualizer.Core.CustomControls.WPFLinePolygon;
using HamiltonVisualizer.DataStructure.Components;
using HamiltonVisualizer.Extensions;
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
    public int NodeTransition = ConstantValues.Time.TransitionDefault;
    public int EdgeTransition = ConstantValues.Time.TransitionDefault;
    public SolidColorBrush ColorizedNode = ConstantValues.ControlColors.NodeTraversalBackground;
    public SolidColorBrush ColorizedLine = ConstantValues.ControlColors.NodeTraversalBackground;

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

    private static void ResetLinesColor(IEnumerable<GraphLine> lines)
    {
        foreach (var line in lines)
        {
            line.ResetColor();
        }
    }

    /// <exception cref="OperationCanceledException"></exception>
    private static void ColorizeLines(IEnumerable<GraphLine> lines, SolidColorBrush color, CancellationToken e)
    {
        foreach (var line in lines)
        {
            e.ThrowIfCancellationRequested();
            line.ChangeColor(color);
        }
    }

    /// <exception cref="OperationCanceledException"></exception>
    private void ColorizeNodeCore(Node node, SolidColorBrush color, CancellationToken e)
    {
        e.ThrowIfCancellationRequested();
        if (color != ConstantValues.ControlColors.NodeDefaultBackground)
            _isModified = true;

        node.Background = color;
    }

    /// <exception cref="OperationCanceledException"></exception>
    public void ColorizeNode(Node node, SolidColorBrush color, CancellationToken e)
    {
        ColorizeNodeCore(node, color, e);
    }

    /// <exception cref="OperationCanceledException"></exception>
    public async Task ColorizeNodeAsync(Node node, SolidColorBrush color, int millisecondsDelay, CancellationToken e)
    {
        e.ThrowIfCancellationRequested();

        if (millisecondsDelay > 0)
            await Task.Delay(millisecondsDelay, e);

        ColorizeNodeCore(node, color, e);
    }

    /// <exception cref="OperationCanceledException"></exception>
    public async Task ColorizeNodesAsync(IEnumerable<Node> nodes, SolidColorBrush color, int millisecondsDelay,
                                         bool delayAtStart, CancellationToken e)
    {
        e.ThrowIfCancellationRequested();

        if (delayAtStart)
            foreach (Node node in nodes)
            {
                await ColorizeNodeAsync(node, color, millisecondsDelay, e);
            }
        else
        {
            e.ThrowIfCancellationRequested();
            ColorizeNode(nodes.First(), color, e);

            foreach (Node node in nodes.Skip(1))
            {
                await ColorizeNodeAsync(node, color, millisecondsDelay, e);
            }
        }
    }

    public async Task PresentDFSAlgorithmAsync(IEnumerable<Node> nodes)
    {
        ResetOrCancel();
        try
        {
            if (SkipTransition)
                await ColorizeNodesAsync(nodes, ColorizedNode, 0, false, CancellationTokenSource.Token);
            else
                await ColorizeNodesAsync(nodes, ColorizedNode, NodeTransition, false, CancellationTokenSource.Token);

            MessageBox.Show("Hoàn thành!");
        }
        catch (OperationCanceledException)
        {
            MessageBox.Show("Kết Thúc!");
        }
    }
    public async Task PresentHamiltonianCycleAlgorithmAsync(IEnumerable<Node> nodes)
    {
        ResetOrCancel();
        try
        {
            if (SkipTransition)
                await ColorizeNodesAsync(nodes, ColorizedNode, 0, false, CancellationTokenSource.Token);
            else
                await ColorizeNodesAsync(nodes, ColorizedNode, NodeTransition, false, CancellationTokenSource.Token);

            var previous = nodes.First();

            foreach (var node in nodes.Skip(1))
            {
                CancellationTokenSource.Token.ThrowIfCancellationRequested();
                GraphLine line = null!;
                if (IsDirectedGraph)
                {
                    line = _linePolygons.First(e => e.From.Origin.TolerantEquals(previous.Origin)
                        && e.To.Origin.TolerantEquals(node.Origin));
                }
                else
                {
                    line = _linePolygons.First(e => e.From.Origin.TolerantEquals(previous.Origin)
                                            && e.To.Origin.TolerantEquals(node.Origin)
                                            || e.To.Origin.TolerantEquals(previous.Origin)
                                            && e.From.Origin.TolerantEquals(node.Origin));
                }
                line.ChangeColor(ColorizedNode);
                previous = node;
            }
            var firstNode = nodes.First();
            GraphLine lastLine = null!;
            if (IsDirectedGraph)
            {
                lastLine = _linePolygons.First(e => e.From.Origin.TolerantEquals(previous.Origin)
                                            && e.To.Origin.TolerantEquals(firstNode.Origin));
            }
            else
            {
                lastLine = _linePolygons.First(e => e.From.Origin.TolerantEquals(previous.Origin)
                                            && e.To.Origin.TolerantEquals(firstNode.Origin) ||
                                            e.To.Origin.TolerantEquals(previous.Origin)
                                            && e.From.Origin.TolerantEquals(firstNode.Origin));
            }
            lastLine.ChangeColor(ColorizedNode);

            MessageBox.Show("Hoàn thành");
        }
        catch (OperationCanceledException)
        {
            MessageBox.Show("Kết Thúc");
        }
    }
    public async Task PresentComponentAlgorithm(IEnumerable<IEnumerable<Node>> components)
    {
        ResetOrCancel();
        ColorPalate.Reset();
        try
        {

            foreach (var component in components)
            {
                var color = ColorPalate.GetUnusedColor();
                await ColorizeNodesAsync(component, ColorizedNode, 0, false, CancellationTokenSource.Token);
            }

            foreach (var node in _nodes.Where(e => e.Adjacent.Count == 0))
            {
                var color = ColorPalate.GetUnusedColor();
                CancellationTokenSource.Token.ThrowIfCancellationRequested();
                ColorizeNode(node, ColorizedNode, CancellationTokenSource.Token);
            }
            MessageBox.Show("Hoàn thành");
        }
        catch (OperationCanceledException)
        {
            MessageBox.Show("Kết Thúc!");
        }
    }
    public async Task PresentLayeredBFSAlgorithm(IEnumerable<BFSComponent<Node>> layeredNode)
    {
        ResetOrCancel();
        ColorPalate.Reset();
        try
        {
            foreach (BFSComponent<Node> layer in layeredNode)
            {
                var lines = BFSComponentProcesser.GetLines(_linePolygons, layer, IsDirectedGraph);
                ColorizeLines(lines, ColorizedLine, CancellationTokenSource.Token);
                await ColorizeNodesAsync(layer.Children, ColorizedNode, 0, false, CancellationTokenSource.Token);
                await Task.Delay(EdgeTransition, CancellationTokenSource.Token);
                ResetLinesColor(lines);
            }
            MessageBox.Show("Hoàn thành");
        }
        catch (OperationCanceledException)
        {
            MessageBox.Show("Kết Thúc");
        }
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
                node.Background = ConstantValues.ControlColors.NodeDefaultBackground;
            }
            foreach (GraphLine line in _linePolygons)
            {
                line.ResetColor();
            }
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
