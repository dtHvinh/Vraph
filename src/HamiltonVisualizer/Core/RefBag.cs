using HamiltonVisualizer.Core.CustomControls.WPFBorder;
using HamiltonVisualizer.Core.CustomControls.WPFLinePolygon;

namespace HamiltonVisualizer.Core
{
    public class RefBag(List<Node> nodes, List<LinePolygonWrapper> edges, SelectedNodeCollection selected)
    {
        public List<Node> Nodes { get; init; } = nodes;
        public List<LinePolygonWrapper> Edges { get; init; } = edges;
        public SelectedNodeCollection SelectedNodeCollection { get; init; } = selected;
    }
}
