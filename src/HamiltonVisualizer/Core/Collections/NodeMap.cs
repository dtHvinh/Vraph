using HamiltonVisualizer.Core.CustomControls.WPFBorder;
using HamiltonVisualizer.Exceptions;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace HamiltonVisualizer.Core.Collections
{
    public class NodeComparer : IEqualityComparer<Node>
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
            return x is not null && y is not null && x.NodeLabel.Text.Equals(y.NodeLabel.Text);
        }

        public int GetHashCode([DisallowNull] Node obj)
        {
            return obj.GetHashCode();
        }
    }

    [DebuggerDisplay("Count={_map.Count}")]
    public class NodeMap
    {
        private const int MAX_CAP = 1000;

        private readonly Dictionary<Node, int> _map = new(comparer: NodeComparer.Instance);
        private readonly List<Node> _reverseMap = [null]; // add a place holder point so that the first element will be add at 1 index
        private static int _i = 1;

        /// <summary>
        /// If key exist => Return associate integer number <br/>
        /// Otherwise => Add new
        /// </summary>
        public int LookUp(Node key)
        {
            if (_map.TryGetValue(key, out int res))
            {
                return res;
            }
            else
            {
                _map.Add(key, _i);
                _reverseMap.Add(key);
            }
            return _i++;
        }

        public Node LookUp(int value)
        {
            Ensure.ThrowIf(
                condition: value >= _reverseMap.Count,
                exception: typeof(IndexOutOfRangeException),
                errorMessage: EM.No_Map_At_Index,
                args: [nameof(Node), value]);

            return _reverseMap[value];
        }

        public bool Remove(Node key)
        {
            return _map.Remove(key);
        }
    }
}
