using HamiltonVisualizer.Core.CustomControls.WPFBorder;
using HamiltonVisualizer.Core.CustomControls.WPFLinePolygon;

namespace HamiltonVisualizer.Core.Collections
{
    public class GraphElementsCollection
    {
        public List<Node> Nodes { get; set; } = [];
        public List<GraphLine> Edges { get; set; } = [];
    }
}
