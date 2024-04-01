using HamiltonVisualizer.Core;
using HamiltonVisualizer.Core.CustomControls.WPFBorder;
using HamiltonVisualizer.Core.CustomControls.WPFLinePolygon;
using System.Collections.ObjectModel;

namespace HamiltonVisualizer.Utilities
{
    public class RefBag(
        ReadOnlyCollection<Node> nodes,
        ReadOnlyCollection<LinePolygonWrapper> edges,
        SelectedNodeCollection selected)
    {
        public ReadOnlyCollection<Node> Nodes { get; init; } = nodes;
        public ReadOnlyCollection<LinePolygonWrapper> Edges { get; init; } = edges;
        public SelectedNodeCollection SelectedNodeCollection { get; init; } = selected;
    }
}
