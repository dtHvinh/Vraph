using HamiltonVisualizer.Core.CustomControls.WPFBorder;
using HamiltonVisualizer.Extensions;
using System.Diagnostics.CodeAnalysis;

namespace HamiltonVisualizer.Core.Collections
{
    internal sealed class NodeComparer : IEqualityComparer<Node>
    {
        private static NodeComparer? _instance;

        private NodeComparer() { }

        public static NodeComparer Instance
        {
            get
            {
                return _instance ??= new NodeComparer();
            }
        }

        public bool Equals(Node? x, Node? y)
        {
            return x is not null && y is not null && x.Origin.TolerantEquals(y.Origin);
        }

        public int GetHashCode([DisallowNull] Node obj)
        {
            return obj.GetHashCode();
        }
    }
}
