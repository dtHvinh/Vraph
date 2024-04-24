using HamiltonVisualizer.Constants;
using HamiltonVisualizer.Core.CustomControls.WPFBorder;
using HamiltonVisualizer.Core.CustomControls.WPFLinePolygon;
using HamiltonVisualizer.DataStructure.Components;
using HamiltonVisualizer.Exceptions;
using HamiltonVisualizer.Extensions;
using System.Windows;
using System.Windows.Media;

namespace HamiltonVisualizer.Core.Functionality
{
    public class AlgorithmPresenter(
        List<Node> nodes,
        List<GraphLine> linePolygons,
        bool isDirected)
    {
        private readonly List<Node> _nodes = nodes;
        private readonly List<GraphLine> _linePolygons = linePolygons;

        public bool IsDirectedGraph { get; set; } = isDirected;
        public bool IsModified { get; set; } = false; // value indicate if reset actually need to be perform
        public bool SkipTransition { get; set; } = false; // how result will be displayed

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
                IsModified = true;

            if (millisecondsDelay > 0)
                await Task.Delay(millisecondsDelay);

            node.Background = color;
        }
        private async Task ColorizeNodes(IEnumerable<Node> nodes, SolidColorBrush color, int millisecondsDelay = 0, bool delayAtStart = false)
        {
            if (color != ConstantValues.ControlColors.NodeDefaultBackground)
                IsModified = true;

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

        public async void PresentTraversalAlgorithm(IEnumerable<Node> node)
        {
            ResetColor();

            if (SkipTransition)
                await ColorizeNodes(node,
                    ConstantValues.ControlColors.NodeTraversalBackground);
            else
                await ColorizeNodes(node,
                    ConstantValues.ControlColors.NodeTraversalBackground, ConstantValues.Time.TransitionDelay);

            IsModified = true;
        }
        public async void PresentHamiltonianCycleAlgorithm(IEnumerable<Node> nodes)
        {
            ResetColor();
            if (SkipTransition)
                await ColorizeNodes(nodes,
                    ConstantValues.ControlColors.NodeTraversalBackground);
            else
                await ColorizeNodes(nodes,
                    ConstantValues.ControlColors.NodeTraversalBackground, ConstantValues.Time.TransitionDelay);

            try
            {
                var previous = nodes.First();

                foreach (var node in nodes.Skip(1))
                {
                    var line = _linePolygons
                        .FirstOrDefault(e => e.From.Origin.TolerantEquals(previous.Origin)
                                    && e.To.Origin.TolerantEquals(node.Origin))
                        ?? throw new ArgumentException($"Not found any edge from {previous.NodeLabel.Text} to {node.NodeLabel.Text}");

                    line.ChangeColor(ConstantValues.ControlColors.NodeTraversalBackground);
                    previous = node;
                }
                var firstNode = nodes.First();
                var lastLine = _linePolygons
                    .FirstOrDefault(e => e.From.Origin.TolerantEquals(previous.Origin)
                                && e.To.Origin.TolerantEquals(firstNode.Origin))
                ?? throw new ArgumentException($"Not found any edge from {previous.NodeLabel.Text} to {firstNode.NodeLabel.Text}");
                lastLine.ChangeColor(ConstantValues.ControlColors.NodeTraversalBackground);

                MessageBox.Show("Hoàn thành");
            }
            catch (Exception)
            {
                MessageBox.Show("Lỗi thuật toán");
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

            IsModified = true;
        }
        public async void PresentLayeredBFSAlgorithm(IEnumerable<BFSComponent<Node>> layeredNode)
        {
            ResetColor();
            ColorPalate.Reset();

            foreach (BFSComponent<Node> layer in layeredNode)
            {
                var lines = BFSComponentProcesser.GetLines(_linePolygons, layer, IsDirectedGraph);
                ColorizeLines(lines, ConstantValues.ControlColors.NodeTraversalBackground);
                await ColorizeNodes(layer.Children, ConstantValues.ControlColors.NodeTraversalBackground, 0, false);
                await Task.Delay(1000);
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
            if (IsModified)
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
}
