using HamiltonVisualizer.Core.Base;
using HamiltonVisualizer.Core.CustomControls.WPFLinePolygon;

namespace HamiltonVisualizer.Core
{
    /// <summary>
    /// Contain information about the connect between instance of <see cref="GraphLine"/> and 
    /// <see cref="Node"/>
    /// </summary>
    public class GraphLineConnectInfo(GraphLine edge, NodeBase node, ConnectPosition pos)
    {
        private readonly GraphLine _edge = edge;
        private readonly NodeBase _node = node;
        private readonly ConnectPosition _pos = pos; // where line and node attach.

        public GraphLine Edge => _edge;
        public ConnectPosition AttachPosition => _pos;
        public NodeBase Node => _node;
    }

    public enum ConnectPosition
    {
        Head,
        Tail
    }
}
