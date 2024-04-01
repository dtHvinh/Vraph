using HamiltonVisualizer.Core.CustomControls.WPFBorder;
using HamiltonVisualizer.Core.CustomControls.WPFLinePolygon;

namespace HamiltonVisualizer.Core
{
    /// <summary>
    /// Contain information about the attachmet between instance of <see cref="LinePolygonWrapper"/> and 
    /// <see cref="Node"/>
    /// </summary>
    public class LinePolygonWrapperAttachInfo(LinePolygonWrapper polygonWrapper, Node node, AttachPosition pos)
    {
        private readonly LinePolygonWrapper linePolygonWrapper = polygonWrapper;
        private readonly Node _node = node;
        private readonly AttachPosition _pos = pos; // where line and node attach.

        public LinePolygonWrapper LinePolygonWrapper => linePolygonWrapper;
        public AttachPosition AttachPosition => _pos;
        public Node Node => _node;
    }

    public enum AttachPosition
    {
        Head,
        Tail
    }
}
