using HamiltonVisualizer.Core.CustomControls.WPFBorder;
using HamiltonVisualizer.Core.CustomControls.WPFLinePolygon;
using HamiltonVisualizer.Exceptions;
using HamiltonVisualizer.Extensions;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace HamiltonVisualizer.Core
{
    /// <summary>
    /// Drawing manager.
    /// </summary>
    /// <param exception="Canvas">The canvas on which this class draws.</param>
    public class DrawManager(Canvas Canvas)
    {
        /// <summary>
        /// Draw a <see cref="Line"/> and add to the collection.
        /// </summary>
        public bool Draw(Node src, Node dst, [NotNullWhen(true)] out LinePolygonWrapper? obj)
        {
            var edge = LinePolygonWrapper.Create(src.Origin, dst.Origin);
            Canvas.Children.Add(edge);
            obj = edge;
            return true;
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
        public static async void ColorizeNodes(IEnumerable<Node> nodes, SolidColorBrush color, int millisecondsDelay, bool delayAtStart = false)
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
