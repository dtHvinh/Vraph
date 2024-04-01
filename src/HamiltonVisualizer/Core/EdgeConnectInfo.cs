using HamiltonVisualizer.Core.CustomControls.WPFBorder;
using HamiltonVisualizer.Core.CustomControls.WPFLinePolygon;

namespace HamiltonVisualizer.Core
{
    /// <summary>
    /// Contain information about the connect between instance of <see cref="CustomControls.WPFLinePolygon.Edge"/> and 
    /// <see cref="Node"/>
    /// </summary>
    public class EdgeConnectInfo(Edge edge, Node node, ConnectPosition pos)
    {
        private readonly Edge _edge = edge;
        private readonly Node _node = node;
        private readonly ConnectPosition _pos = pos; // where line and node attach.

        public Edge Edge => _edge;
        public ConnectPosition AttachPosition => _pos;
        public Node Node => _node;
    }

    public enum ConnectPosition
    {
        Head,
        Tail
    }
}
