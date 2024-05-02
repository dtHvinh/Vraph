using HamiltonVisualizer.Core.Collections;
using HamiltonVisualizer.Core.CustomControls.WPFBorder;
using HamiltonVisualizer.Core.CustomControls.WPFLinePolygon;
using System.Collections.ObjectModel;

namespace HamiltonVisualizer.Utilities;

internal sealed class RefBag
{
    public ReadOnlyCollection<Node> Nodes { get; private set; }
    public ReadOnlyCollection<GraphLine> Edges { get; private set; }
    public SelectedNodePair SelectedNodeCollection { get; private set; }
    public RefBag(
        List<Node> nodes,
        List<GraphLine> edges,
        SelectedNodePair selected)
    {
        Nodes = nodes.AsReadOnly();
        Edges = edges.AsReadOnly();
        SelectedNodeCollection = selected;

    }

    public RefBag(
        ReadOnlyCollection<Node> nodes,
        ReadOnlyCollection<GraphLine> lines,
        SelectedNodePair selectedNodeCollection)
    {
        Nodes = nodes;
        Edges = lines;
        SelectedNodeCollection = selectedNodeCollection;
    }
}
