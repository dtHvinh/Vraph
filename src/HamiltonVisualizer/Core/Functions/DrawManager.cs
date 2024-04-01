using HamiltonVisualizer.Core.CustomControls.WPFBorder;
using HamiltonVisualizer.Core.CustomControls.WPFLinePolygon;
using HamiltonVisualizer.Extensions;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace HamiltonVisualizer.Core.Functions
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
        public bool Draw(Node src, Node dst, [NotNullWhen(true)] out Edge? obj)
        {
            var edge = new Edge(src, dst);

            src.Attach(new EdgeConnectInfo(edge, src, ConnectPosition.Head));
            dst.Attach(new EdgeConnectInfo(edge, dst, ConnectPosition.Tail));

            Canvas.Children.Add(edge);
            obj = edge;
            return true;
        }
    }
}
