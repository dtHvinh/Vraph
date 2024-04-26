using HamiltonVisualizer.Core.CustomControls.WPFLinePolygon;

namespace HamiltonVisualizer.Events.EventArgs.ForGraphLine;

internal sealed class GraphLineDeleteEventArgs(GraphLine graphLine)
{
    public GraphLine GraphLine { get; set; } = graphLine;
}
