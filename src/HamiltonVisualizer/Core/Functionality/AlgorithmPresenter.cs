using HamiltonVisualizer.Constants;
using HamiltonVisualizer.Core.CustomControls.WPFBorder;
using HamiltonVisualizer.Core.CustomControls.WPFLinePolygon;
using HamiltonVisualizer.DataStructure.Components;
using HamiltonVisualizer.Exceptions;
using HamiltonVisualizer.Extensions;
using HamiltonVisualizer.Options;
using System.Windows;
using System.Windows.Media;

namespace HamiltonVisualizer.Core.Functionality
{
    public class AlgorithmPresenter(List<Node> nodes, List<GraphLine> graphLine)
    {
        private bool _isModified = false; // value indicate if reset actually need to be perform
        private readonly List<Node> _nodes = nodes;
        private readonly List<GraphLine> _linePolygons = graphLine;

        public bool IsDirectedGraph { get; set; } = true;
        public bool SkipTransition { get; set; } = false; // how result will be displayed
        public int NodeTransition = ConstantValues.Time.Transition;
        public int EdgeTransition = ConstantValues.Time.Transition;
        public SolidColorBrush ColorizedNode = ConstantValues.ControlColors.NodeTraversalBackground;
        public SolidColorBrush ColorizedLine = ConstantValues.ControlColors.NodeTraversalBackground;

        public AlgorithmPresenter(List<Node> nodes, List<GraphLine> graphLines, Action<AlgorithmPresenterOptions> configureOptions) : this(nodes, graphLines)
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

        private static void ColorizeLines(IEnumerable<GraphLine> lines, SolidColorBrush color)
        {
            foreach (var line in lines)
            {
                line.ChangeColor(color);
            }
        }
        private static void ResetLinesColor(IEnumerable<GraphLine> lines)
        {
            foreach (var line in lines)
            {
                line.ResetColor();
            }
        }
        private async Task ColorizeNode(Node node, SolidColorBrush color, int millisecondsDelay = 0)
        {
            if (color != ConstantValues.ControlColors.NodeDefaultBackground)
                _isModified = true;

            if (millisecondsDelay > 0)
                await Task.Delay(millisecondsDelay);

            node.Background = color;
        }
        private async Task ColorizeNodes(IEnumerable<Node> nodes, SolidColorBrush color, int millisecondsDelay = 0, bool delayAtStart = false)
        {
            if (color != ConstantValues.ControlColors.NodeDefaultBackground)
                _isModified = true;

            Ensure.ThrowIf(
                condition: millisecondsDelay < 0,
                exception: typeof(ArgumentException),
                errorMessage: EM.Not_Support_Negative_Number);

            if (delayAtStart)
                foreach (Node node in nodes.ToList())
                {
                    await ColorizeNode(node, color, millisecondsDelay);
                }
            else
            {
                try
                {
                    nodes = nodes.ToList();

                    await ColorizeNode(nodes.First(), color);

                    foreach (Node node in nodes.Skip(1))
                    {
                        await ColorizeNode(node, color, millisecondsDelay);
                    }
                }
                catch (Exception) { }
            }
        }

        public async void PresentDFSAlgorithm(IEnumerable<Node> node)
        {
            ResetColor();

            if (SkipTransition)
                await ColorizeNodes(node, ColorizedNode);
            else
                await ColorizeNodes(node, ColorizedNode, NodeTransition);

            _isModified = true;
        }
        public async void PresentHamiltonianCycleAlgorithm(IEnumerable<Node> nodes)
        {
            ResetColor();
            if (SkipTransition)
                await ColorizeNodes(nodes, ColorizedNode);
            else
                await ColorizeNodes(nodes, ColorizedNode, NodeTransition);

            try
            {
                var previous = nodes.First();

                foreach (var node in nodes.Skip(1))
                {
                    var line = _linePolygons
                        .FirstOrDefault(e => e.From.Origin.TolerantEquals(previous.Origin)
                                    && e.To.Origin.TolerantEquals(node.Origin))
                        ?? throw new ArgumentException($"Not found any edge from \'{previous.NodeLabel.Text}\' to \'{node.NodeLabel.Text}\'");

                    line.ChangeColor(ColorizedNode);
                    previous = node;
                }
                var firstNode = nodes.First();
                var lastLine = _linePolygons
                    .FirstOrDefault(e => e.From.Origin.TolerantEquals(previous.Origin)
                                && e.To.Origin.TolerantEquals(firstNode.Origin))
                ?? throw new ArgumentException($"Not found any edge from \'{previous.NodeLabel.Text}\' to \'{firstNode.NodeLabel.Text}\'");
                lastLine.ChangeColor(ColorizedNode);

                MessageBox.Show("Hoàn thành");
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
        public async void PresentComponentAlgorithm(IEnumerable<IEnumerable<Node>> components)
        {
            ResetColor();
            ColorPalate.Reset();

            foreach (var component in components)
            {
                var color = ColorPalate.GetUnusedColor();
                await ColorizeNodes(component, color);
            }

            foreach (var node in _nodes.Where(e => e.Adjacent.Count == 0))
            {
                var color = ColorPalate.GetUnusedColor();
                await ColorizeNode(node, color);
            }
            MessageBox.Show("Hoàn thành");

            _isModified = true;
        }
        public async void PresentLayeredBFSAlgorithm(IEnumerable<BFSComponent<Node>> layeredNode)
        {
            ResetColor();
            ColorPalate.Reset();

            foreach (BFSComponent<Node> layer in layeredNode)
            {
                var lines = BFSComponentProcesser.GetLines(_linePolygons, layer, IsDirectedGraph);
                ColorizeLines(lines, ColorizedLine);
                await ColorizeNodes(layer.Children, ColorizedNode, 0, false);
                await Task.Delay(EdgeTransition);
                ResetLinesColor(lines);
            }
            MessageBox.Show("Hoàn thành");
        }
        public void GraphModeChange()
        {
            IsDirectedGraph = !IsDirectedGraph;
            GraphLineArrowVisibilityChange();
        }
        public void ResetColor()
        {
            if (_isModified)
            {
                foreach (Node node in _nodes)
                {
                    node.Background = ConstantValues.ControlColors.NodeDefaultBackground;
                }
                foreach (GraphLine line in _linePolygons)
                {
                    line.ResetColor();
                }
            }
        }
    }

    static class BFSComponentProcesser
    {
        /// <summary>
        /// Get the lines from <paramref name="source"/> which connect from Root to Children.
        /// </summary>
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
}
