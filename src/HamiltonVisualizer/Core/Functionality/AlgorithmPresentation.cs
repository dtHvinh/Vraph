using HamiltonVisualizer.Constants;
using HamiltonVisualizer.Core.CustomControls.WPFBorder;
using HamiltonVisualizer.Core.CustomControls.WPFLinePolygon;
using HamiltonVisualizer.Core.Functionality;
using HamiltonVisualizer.Exceptions;
using HamiltonVisualizer.Helpers;
using Serilog;
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

        private void GraphLineArrowVisibilityChange()
        {
            foreach (var line in linePolygons)
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

        public async void PresentTraversalAlgorithm(IEnumerable<Node> node)
        {
            Log.Information("Presenting Traversal algorithm!");
            ResetColor();

            if (SkipTransition)
                await ColorizeNodes(node,
                    ConstantValues.ControlColors.NodeTraversalBackground);
            else
                await ColorizeNodes(node,
                    ConstantValues.ControlColors.NodeTraversalBackground, ConstantValues.Time.TransitionDelay);

            IsModified = true;
        }
        public async void PresentComponentAlgorithm(IEnumerable<IEnumerable<Node>> components)
        {
            Log.Information("Presenting Component algorithm!");

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
        public void GraphModeChange()
        {
            GraphLineArrowVisibilityChange();
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
