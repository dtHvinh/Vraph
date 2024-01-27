using System.Windows;
using System.Windows.Controls;

namespace HamiltonVisualizer.CustomControls
{
    public class GraphDrawingCanvas : Canvas
    {
        static GraphDrawingCanvas()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GraphDrawingCanvas), new FrameworkPropertyMetadata(typeof(GraphDrawingCanvas)));
        }
    }
}
