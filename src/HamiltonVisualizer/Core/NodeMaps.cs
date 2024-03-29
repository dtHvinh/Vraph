using HamiltonVisualizer.Extensions;
using HamiltonVisualizer.GraphUIComponents;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace HamiltonVisualizer.Core
{
    internal class NodeComparer : IEqualityComparer<Node>
    {
        private static NodeComparer? _instance;
        private NodeComparer()
        {

        }

        public static NodeComparer Instance
        {
            get
            {
                return _instance ??= new NodeComparer();
            }
        }

        public bool Equals(Node? x, Node? y)
        {
            return x is not null
                && y is not null
                && x.Origin.TolerantEquals(y.Origin);
        }

        public int GetHashCode([DisallowNull] Node obj)
        {
            return HashCode.Combine(obj.Origin.X, obj.Origin.Y);
        }
    }

    public class NodeMaps
    {
        private readonly Dictionary<Node, int> _map = new(NodeComparer.Instance);
        private static int _i = 1;

        public int LookUp(Node key)
        {
            ref var valOrNew = ref CollectionsMarshal.GetValueRefOrAddDefault(_map, key, out bool existed);

            if (!existed)
                valOrNew = _i++;

            return valOrNew;
        }
    }
}
