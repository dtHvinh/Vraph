using HamiltonVisualizer.Core.Collections;
using HamiltonVisualizer.Core.CustomControls.WPFBorder;
using HamiltonVisualizer.Core.CustomControls.WPFLinePolygon;
using System.Collections.ObjectModel;

namespace HamiltonVisualizer.Utilities
{
    public class RefBag(
        List<Node> nodes,
        List<GraphLine> edges,
        SelectedNodeCollection selected)
    {
        public ReadOnlyCollection<Node> Nodes { get; init; } = nodes.AsReadOnly();
        public ReadOnlyCollection<GraphLine> Edges { get; init; } = edges.AsReadOnly();
        public SelectedNodeCollection SelectedNodeCollection { get; init; } = selected;
    }
}
