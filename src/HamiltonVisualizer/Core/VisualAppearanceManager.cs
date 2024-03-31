using HamiltonVisualizer.Core.CustomControls.WPFBorder;
using HamiltonVisualizer.Core.CustomControls.WPFLinePolygon;
using HamiltonVisualizer.Exceptions;
using System.Collections.ObjectModel;
using System.Windows.Media;

namespace HamiltonVisualizer.Core
{
    public class VisualAppearanceManager(
        ReadOnlyCollection<Node> nodes,
        ReadOnlyCollection<LinePolygonWrapper> linePolygons)
    {
        public void ColorizeNodes(SolidColorBrush color)
        {
            foreach (Node node in nodes)
            {
                node.Background = color;
            }
        }

        //TODO: unused
        public void ColorizeLinePolygonWrapper(SolidColorBrush color)
        {
            foreach (LinePolygonWrapper line in linePolygons)
            {
                line.Head.Fill = color;
                line.Head.Stroke = color;
                line.Body.Fill = color;
                line.Body.Stroke = color;
            }
        }

        public static void ColorizeNodes(IEnumerable<Node> nodes, SolidColorBrush color)
        {
            foreach (Node node in nodes)
            {
                node.Background = color;
            }
        }

        /// <summary>
        /// Sequency colorize node after <paramref name="millisecondsDelay"/>.
        /// </summary>
        public static async void ColorizeNodes(
            IEnumerable<Node> nodes,
            SolidColorBrush color,
            int millisecondsDelay,
            bool delayAtStart = false)
        {
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
    }
}
