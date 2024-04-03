using HamiltonVisualizer.Core.CustomControls.WPFBorder;
using HamiltonVisualizer.Core.CustomControls.WPFLinePolygon;

namespace HamiltonVisualizer.Core
{
    /// <summary>
    /// Contain information about the connect between instance of <see cref="CustomControls.WPFLinePolygon.GraphLine"/> and 
    /// <see cref="Node"/>
    /// </summary>
    public class GraphLineConnectInfo(GraphLine edge, Node node, ConnectPosition pos)
    {
        private readonly GraphLine _edge = edge;
        private readonly Node _node = node;
        private readonly ConnectPosition _pos = pos; // where line and node attach.

        public GraphLine Edge => _edge;
        public ConnectPosition AttachPosition => _pos;
        public Node Node => _node;
    }

    public enum ConnectPosition
    {
        Head,
        Tail
    }
}
