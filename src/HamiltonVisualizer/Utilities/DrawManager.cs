using HamiltonVisualizer.Core.CustomControls.WPFBorder;
using HamiltonVisualizer.Core.CustomControls.WPFLinePolygon;
using HamiltonVisualizer.Extensions;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace HamiltonVisualizer.Utilities;

/// <summary>
/// Drawing manager.
/// </summary>
/// <param exception="Canvas">The canvas on which this class draws.</param>
internal sealed class DrawManager(Canvas Canvas)
{
    /// <summary>
    /// Draw a <see cref="Line"/> and add to the collection.
    /// </summary>
    public bool DrawLine(Node src, Node dst, bool headVisible, [NotNullWhen(true)] out GraphLine? obj)
    {
        var edge = new GraphLine(src, dst);
        obj = edge;
        if (!headVisible)
        {
            edge.Head.Visibility = System.Windows.Visibility.Collapsed;
        }
        Canvas.Children.Add(edge);
        return true;
    }
}
