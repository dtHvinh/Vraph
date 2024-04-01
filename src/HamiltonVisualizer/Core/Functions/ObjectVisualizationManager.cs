using HamiltonVisualizer.Constants;
using HamiltonVisualizer.Core.CustomControls.WPFBorder;
using HamiltonVisualizer.Core.CustomControls.WPFLinePolygon;
using HamiltonVisualizer.Exceptions;
using System.Collections.ObjectModel;
using System.Windows.Media;

namespace HamiltonVisualizer.Core.Functions
{
    public class ObjectVisualizationManager(
        ReadOnlyCollection<Node> nodes,
        ReadOnlyCollection<Edge> linePolygons)
    {
        public bool IsModified { get; set; } = false; // value indicate if reset actually need to be perform

        public void ColorizeNodes(SolidColorBrush color)
        {
            if (color != ConstantValues.ControlColors.NodeDefaultBackground)
                IsModified = true;

            foreach (Node node in nodes)
            {
                node.Background = color;
            }
        }

        //TODO: unused
        public void ColorizeEdge(SolidColorBrush color)
        {
            if (color != ConstantValues.ControlColors.NodeDefaultBackground)
                IsModified = true;

            foreach (Edge line in linePolygons)
            {
                line.Head.Fill = color;
                line.Head.Stroke = color;
                line.Body.Fill = color;
                line.Body.Stroke = color;
            }
        }

        public void ColorizeNodes(IEnumerable<Node> nodes, SolidColorBrush color)
        {
            if (color != ConstantValues.ControlColors.NodeDefaultBackground)
                IsModified = true;

            foreach (Node node in nodes)
            {
                node.Background = color;
            }
        }

        /// <summary>
        /// Sequency colorize node after <paramref name="millisecondsDelay"/>.
        /// </summary>
        public async void ColorizeNodes(
            IEnumerable<Node> nodes,
            SolidColorBrush color,
            int millisecondsDelay,
            bool delayAtStart = false)
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
