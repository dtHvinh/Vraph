using HamiltonVisualizer.Core.CustomControls.WPFBorder;
using HamiltonVisualizer.Core.CustomControls.WPFLinePolygon;
using HamiltonVisualizer.DataStructure.Components;
using HamiltonVisualizer.Extensions;
using System.Windows.Media;

namespace HamiltonVisualizer.Utilities;
internal sealed class AlgorithmPresenterBuilder
{
    private IEnumerable<BFSComponent<Node>>? _bfsComps;
    private IEnumerable<GraphLine>? _lines;
    private IEnumerable<Node>? _nodes;
    private IEnumerable<IEnumerable<Node>>? _comps;

    private bool _isDirected = true;
    private CancellationToken _ct;
    private bool _delayAtStart = false;
    private int _transition = 0;
    private SolidColorBrush? _color;
    private Action? _whenThrownAction;
    private Action? _postAction;
    private AlgorithmPresentResult _result = AlgorithmPresentResult.Building;

    private AlgorithmPresenterBuilder()
    {
    }

    public static AlgorithmPresenterBuilder CreateBuilder()
    {
        return new AlgorithmPresenterBuilder();
    }

    public AlgorithmPresenterBuilder ForEachBFSComponent(IEnumerable<BFSComponent<Node>> components)
    {
        _bfsComps = components;
        return this;
    }

    public AlgorithmPresenterBuilder ForEachComponent(IEnumerable<IEnumerable<Node>> comps)
    {
        _comps = comps;
        return this;
    }

    public AlgorithmPresenterBuilder ForEachLine(IEnumerable<GraphLine> lines)
    {
        _lines = lines;
        return this;
    }

    public AlgorithmPresenterBuilder ForEachNode(IEnumerable<Node> nodes)
    {
        _nodes = nodes;
        return this;
    }

    public AlgorithmPresenterBuilder WithColor(SolidColorBrush color)
    {
        _color = color;
        return this;
    }

    public AlgorithmPresenterBuilder ConfigureTransition(int transition)
    {
        _transition = transition;
        return this;
    }

    public AlgorithmPresenterBuilder WithCancellationToken(CancellationToken ct)
    {
        _ct = ct;
        return this;
    }

    public AlgorithmPresenterBuilder WithGraph(bool isDirected)
    {
        _isDirected = isDirected;
        return this;
    }

    public AlgorithmPresenterBuilder ConfigureDelayAtStart(bool delayAtStart)
    {
        _delayAtStart = delayAtStart;
        return this;
    }

    public AlgorithmPresenterBuilder BeforeStart(Action action)
    {
        action();
        return this;
    }

    public AlgorithmPresenterBuilder IfThrown(Action action)
    {
        _whenThrownAction = action;
        return this;
    }

    public AlgorithmPresenterBuilder IfFinished(Action action)
    {
        _postAction = action;
        return this;
    }

    /// <summary>
    /// Should be called at the end of try block.
    /// </summary>
    /// <returns></returns>
    private AlgorithmPresentResult BuildFinished()
    {
        _postAction?.Invoke();
        return AlgorithmPresentResult.Finished;
    }

    /// <summary>
    /// Should be called at the end of catch block.
    /// </summary>
    /// <returns></returns>
    private AlgorithmPresentResult BuildFail()
    {
        _result = AlgorithmPresentResult.Interrupted;
        _whenThrownAction?.Invoke();
        return _result;
    }

    public async Task<AlgorithmPresentResult> BuildDFS()
    {
        try
        {
            if (_nodes is null)
                throw new InvalidOperationException("required data for present algorithm!");

            await ColorizeNodesAsync(
                _nodes,
                _color ?? throw new InvalidOperationException("No color was specified!"),
                _transition,
                _delayAtStart,
                _ct);

            return BuildFinished();
        }
        catch
        {
            return BuildFail();
        }
    }

    public async Task<AlgorithmPresentResult> BuildHC()
    {
        try
        {
            if (_nodes is null)
                throw new InvalidOperationException("required data for present algorithm!");

            if (_lines is null)
                throw new InvalidOperationException("required data for present algorithm!");

            await ColorizeNodesAsync(
                _nodes,
                _color ?? throw new InvalidOperationException("No color was specified!"),
                _transition,
                _delayAtStart,
            _ct);

            var previous = _nodes.First();

            foreach (var node in _nodes.Skip(1))
            {
                _ct.ThrowIfCancellationRequested();
                GraphLine line = null!;
                if (_isDirected)
                {
                    line = _lines.First(e => e.From.Origin.TolerantEquals(previous.Origin)
                        && e.To.Origin.TolerantEquals(node.Origin));
                }
                else
                {
                    line = _lines.First(e => e.From.Origin.TolerantEquals(previous.Origin)
                                            && e.To.Origin.TolerantEquals(node.Origin)
                                            || e.To.Origin.TolerantEquals(previous.Origin)
                                            && e.From.Origin.TolerantEquals(node.Origin));
                }
                line.ChangeColor(_color);
                previous = node;
            }

            var firstNode = _nodes.First();

            GraphLine lastLine = null!;

            if (_isDirected)
            {
                lastLine = _lines.First(e => e.From.Origin.TolerantEquals(previous.Origin)
                                            && e.To.Origin.TolerantEquals(firstNode.Origin));
            }
            else
            {
                lastLine = _lines.First(e => e.From.Origin.TolerantEquals(previous.Origin)
                                            && e.To.Origin.TolerantEquals(firstNode.Origin) ||
                                            e.To.Origin.TolerantEquals(previous.Origin)
                                            && e.From.Origin.TolerantEquals(firstNode.Origin));
            }
            lastLine.ChangeColor(_color);

            return BuildFinished();
        }
        catch
        {
            return BuildFail();
        }
    }

    public async Task<AlgorithmPresentResult> BuildBFS()
    {
        try
        {
            ArgumentNullException.ThrowIfNull(_bfsComps);
            ArgumentNullException.ThrowIfNull(_lines);
            ArgumentNullException.ThrowIfNull(_color);

            foreach (BFSComponent<Node> layer in _bfsComps)
            {
                var lines = BFSComponentProcesser.GetLines(_lines, layer, _isDirected);
                ColorizeLines(lines, _color, _ct);
                await ColorizeNodesAsync(layer.Children, _color, 0, false, _ct);
                await Task.Delay(_transition, _ct);
                ResetLinesColor(lines);
            }

            return BuildFinished();
        }
        catch
        {
            return BuildFail();
        }
    }

    public async Task<AlgorithmPresentResult> BuildComponents()
    {
        try
        {
            ArgumentNullException.ThrowIfNull(_comps);
            ArgumentNullException.ThrowIfNull(_nodes);

            foreach (var component in _comps)
            {
                await ColorizeNodesAsync(component, ColorPalate.GetUnusedColor(), 0, false, _ct);
            }

            foreach (var node in _nodes.Where(e => e.Adjacent.Count == 0))
            {
                _ct.ThrowIfCancellationRequested();
                ColorizeNode(node, ColorPalate.GetUnusedColor(), _ct);
            }

            return BuildFinished();
        }
        catch
        {
            return BuildFail();
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
    private static void ColorizeNodeCore(Node node, SolidColorBrush color, CancellationToken e)
    {
        e.ThrowIfCancellationRequested();
        node.Background = color;
    }

    /// <exception cref="OperationCanceledException"></exception>
    private static void ColorizeNode(Node node, SolidColorBrush color, CancellationToken e)
    {
        ColorizeNodeCore(node, color, e);
    }

    /// <exception cref="OperationCanceledException"></exception>
    private static async Task ColorizeNodeAsync(Node node, SolidColorBrush color, int millisecondsDelay, CancellationToken e)
    {
        e.ThrowIfCancellationRequested();

        if (millisecondsDelay > 0)
            await Task.Delay(millisecondsDelay, e);

        ColorizeNodeCore(node, color, e);
    }

    /// <exception cref="OperationCanceledException"></exception>
    private static async Task ColorizeNodesAsync(IEnumerable<Node> nodes, SolidColorBrush color, int millisecondsDelay,
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

    private static void ResetLinesColor(IEnumerable<GraphLine> lines)
    {
        foreach (var line in lines)
        {
            line.ResetColor();
        }
    }
}

internal enum AlgorithmPresentResult
{
    /// <summary>
    /// Completed.
    /// </summary>
    Finished,
    /// <summary>
    /// May be interrupted by CancellationToken.
    /// </summary>
    Interrupted,
    /// <summary>
    /// Build method not invoke yet.
    /// </summary>
    Building,
}
