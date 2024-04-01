using HamiltonVisualizer.Core.CustomControls.WPFBorder;
using HamiltonVisualizer.Core.CustomControls.WPFLinePolygon;

namespace HamiltonVisualizer.Core
{
    /// <summary>
    /// Contain information about the attachmet between instance of <see cref="CustomControls.WPFLinePolygon.Edge"/> and 
    /// <see cref="Node"/>
    /// </summary>
    public class EdgeAttachInfo(Edge edge, Node node, AttachPosition pos)
    {
        private readonly Edge _edge = edge;
        private readonly Node _node = node;
        private readonly AttachPosition _pos = pos; // where line and node attach.

        public Edge Edge => _edge;
        public AttachPosition AttachPosition => _pos;
        public Node Node => _node;
    }

    public enum AttachPosition
    {
        Head,
        Tail
    }
}
