using HamiltonVisualizer.Constants;
using HamiltonVisualizer.Core.CustomControls.WPFBorder;
using HamiltonVisualizer.Core.CustomControls.WPFLinePolygon;
using HamiltonVisualizer.Exceptions;
using System.Collections.ObjectModel;
using System.Windows.Media;

namespace HamiltonVisualizer.Core.Functions
{
    public class AlgorithmPresentation(
        ReadOnlyCollection<Node> nodes,
        ReadOnlyCollection<GraphLine> linePolygons)
    {
        public bool IsModified { get; set; } = false; // value indicate if reset actually need to be perform
        public bool SkipTransition { get; set; } = false; // how result will be displayed

        private void ColorizeNodes(SolidColorBrush color)
        {
            if (color != ConstantValues.ControlColors.NodeDefaultBackground)
                IsModified = true;

            foreach (Node node in nodes)
            {
                node.Background = color;
            }
        }
        private void ColorizeGraphLine(SolidColorBrush color)
        {
            if (color != ConstantValues.ControlColors.NodeDefaultBackground)
                IsModified = true;

            foreach (GraphLine line in linePolygons)
            {
                line.Head.Fill = color;
                line.Head.Stroke = color;
                line.Body.Fill = color;
                line.Body.Stroke = color;
            }
        }
        private void ColorizeGraphLine(IEnumerable<GraphLine> lines, SolidColorBrush color)
        {
            if (color != ConstantValues.ControlColors.NodeDefaultBackground)
                IsModified = true;

            foreach (GraphLine line in lines)
            {
                line.Head.Fill = color;
                line.Head.Stroke = color;
                line.Body.Fill = color;
                line.Body.Stroke = color;
            }
        }
        private void ColorizeNodes(IEnumerable<Node> nodes, SolidColorBrush color)
        {
            if (color != ConstantValues.ControlColors.NodeDefaultBackground)
                IsModified = true;

            foreach (Node node in nodes)
            {
                node.Background = color;
            }
        }
        private async Task ColorizeNodes(IEnumerable<Node> nodes, SolidColorBrush color, int millisecondsDelay, bool delayAtStart = false)
        {
            if (color != ConstantValues.ControlColors.NodeDefaultBackground)
                IsModified = true;

            Ensure.ThrowIf(
                condition: millisecondsDelay < 0,
                exception: typeof(ArgumentException),
                errorMessage: EM.Not_Support_Negative_Number);

            if (millisecondsDelay == 0)
            {
                ColorizeNodes(nodes, color);
            }
            else
            {
                if (delayAtStart)
                    foreach (Node node in nodes)
                    {
                        await Task.Delay(millisecondsDelay);
                        node.Background = color;
                    }
                else
                {
                    try
                    {
                        nodes.First().Background = color;

                        foreach (Node node in nodes.Skip(1))
                        {
                            await Task.Delay(millisecondsDelay);
                            node.Background = color;
                        }
                    }
                    catch (Exception) { }
                }
            }
        }

        public async void PresentTraversalAlgorithm(IEnumerable<Node> node)
        {
            ResetColor();

            if (SkipTransition)
                ColorizeNodes(node,
                    ConstantValues.ControlColors.NodeTraversalBackground);
            else
                await ColorizeNodes(node,
                    ConstantValues.ControlColors.NodeTraversalBackground, 500);

            IsModified = true;
        }
        public async void PresentComponentAlgorithm(IEnumerable<IEnumerable<Node>> components)
        {
            ResetColor();

            var leftOverNode = nodes.Except(components.SelectMany(e => e));

            IsModified = true;
        }

        public void ResetColor()
        {
            if (IsModified)
                foreach (Node node in nodes)
                {
                    node.Background = ConstantValues.ControlColors.NodeDefaultBackground;
                }
        }
    }
}
