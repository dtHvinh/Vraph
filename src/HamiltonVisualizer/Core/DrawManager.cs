using HamiltonVisualizer.Core.CustomControls.WPFBorder;
using HamiltonVisualizer.Core.CustomControls.WPFLinePolygon;
using HamiltonVisualizer.Extensions;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Controls;
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
        public bool Draw(Node src, Node dst, [NotNullWhen(true)] out Edge? obj)
        {
            var edge = new Edge(src, dst);

            src.Attach(new EdgeAttachInfo(edge, src, AttachPosition.Head));
            dst.Attach(new EdgeAttachInfo(edge, dst, AttachPosition.Tail));

            Canvas.Children.Add(edge);
            obj = edge;
            return true;
        }
    }
}
