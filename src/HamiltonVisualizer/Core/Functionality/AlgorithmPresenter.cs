using HamiltonVisualizer.Constants;
using HamiltonVisualizer.Core.CustomControls.WPFBorder;
using HamiltonVisualizer.Core.CustomControls.WPFLinePolygon;
using HamiltonVisualizer.Exceptions;
using HamiltonVisualizer.Extensions;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;

namespace HamiltonVisualizer.Core.Functionality
{
    public class AlgorithmPresenter(
        ReadOnlyCollection<Node> nodes,
        ReadOnlyCollection<GraphLine> linePolygons)
    {
        public bool IsModified { get; set; } = false; // value indicate if reset actually need to be perform
        public bool SkipTransition { get; set; } = false; // how result will be displayed

        private void GraphLineArrowVisibilityChange()
        {
            foreach (var line in linePolygons)
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
        private void ColorizeLines(IEnumerable<GraphLine> lines, SolidColorBrush color)
        {
            foreach (var line in lines)
            {
                line.ChangeColor(color);
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
                    await ColorizeNode(nodes.First(), color);

                    foreach (Node node in nodes.Skip(1))
                    {
                        await ColorizeNode(node, color, millisecondsDelay);
                    }
                }
                catch (Exception) { }
            }
        }

        //private async Task ColorizeNodesAndLines(IEnumerable<Node> nodes, SolidColorBrush color, int millisecondsDelay = 0)
        //{
        //    if (color != ConstantValues.ControlColors.NodeDefaultBackground)
        //        IsModified = true;

        //    Ensure.ThrowIf(
        //        condition: millisecondsDelay < 0,
        //        exception: typeof(ArgumentException),
        //        errorMessage: EM.Not_Support_Negative_Number);

        //    var fromNode = nodes.First();

        //    await ColorizeNode(fromNode, color);

        //    foreach (Node nodes in nodes.Skip(1))
        //    {
        //        GraphLine? lineBetween = fromNode.Adjacent.FirstOrDefault(e => e.Edge.To.Origin.TolerantEquals(nodes.Origin))?.Edge;
        //        if (lineBetween != null)
        //            await ColorizeLine(lineBetween, color);
        //        await ColorizeNode(nodes, color);
        //        fromNode = nodes;
        //    }
        //}

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
                    var line = linePolygons
                        .FirstOrDefault(e => e.From.Origin.TolerantEquals(previous.Origin)
                                    && e.To.Origin.TolerantEquals(node.Origin))
                        ?? throw new ArgumentException($"Not found any edge from {previous.NodeLabel.Text} to {node.NodeLabel.Text}");

                    line.ChangeColor(ConstantValues.ControlColors.NodeTraversalBackground);
                    previous = node;
                }
                var firstNode = nodes.First();
                var lastLine = linePolygons
                    .FirstOrDefault(e => e.From.Origin.TolerantEquals(previous.Origin)
                                && e.To.Origin.TolerantEquals(firstNode.Origin))
                ?? throw new ArgumentException($"Not found any edge from {previous.NodeLabel.Text} to {firstNode.NodeLabel.Text}");
                lastLine.ChangeColor(ConstantValues.ControlColors.NodeTraversalBackground);
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

            foreach (var node in nodes.Where(e => e.Adjacent.Count == 0))
            {
                var color = ColorPalate.GetUnusedColor();
                await ColorizeNode(node, color);
            }

            IsModified = true;
        }
        public async void PresentLayeredBFSAlgorithm(IEnumerable<IEnumerable<Node>> layeredNode)
        {
            ResetColor();
            ColorPalate.Reset();

            foreach (IEnumerable<Node> layer in layeredNode)
            {
                await ColorizeNodes(layer, ConstantValues.ControlColors.NodeTraversalBackground, 0, false);
                await Task.Delay(1000);
            }
        }
        public void GraphModeChange()
        {
            GraphLineArrowVisibilityChange();
        }
        public void ResetColor()
        {
            if (IsModified)
            {
                foreach (Node node in nodes)
                {
                    node.Background = ConstantValues.ControlColors.NodeDefaultBackground;
                }
                foreach (GraphLine line in linePolygons)
                {
                    line.ResetColor();
                }
            }
        }
    }
}
