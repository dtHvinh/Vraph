using HamiltonVisualizer.Core.Base;
using HamiltonVisualizer.Core.CustomControls.WPFLinePolygon;

namespace HamiltonVisualizer.Core
{
    /// <summary>
    /// Contain information about the connect between instance of <see cref="GraphLine"/> and 
    /// <see cref="Node"/>
    /// </summary>
    internal sealed class GraphLineConnectInfo(GraphLine edge, MovableObject node, ConnectPosition pos)
    {
        private readonly GraphLine _edge = edge;
        private readonly MovableObject _node = node;
        private readonly ConnectPosition _pos = pos; // where line and node attach.

        public GraphLine Edge => _edge;
        public ConnectPosition AttachPosition => _pos;
        public MovableObject Node => _node;
    }

    public enum ConnectPosition
    {
        Head,
        Tail
    }
}
