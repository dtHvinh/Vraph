using HamiltonVisualizer.Core.CustomControls.WPFBorder;
using HamiltonVisualizer.Exceptions;
using HamiltonVisualizer.Extensions;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Windows;

namespace HamiltonVisualizer.Utilities
{
    internal class PointComparer : IEqualityComparer<Point>
    {
        private static PointComparer? _instance;

        private PointComparer() { }

        public static PointComparer Instance
        {
            get
            {
                return _instance ??= new PointComparer();
            }
        }

        public bool Equals(Point x, Point y)
        {
            return x.TolerantEquals(y);
        }

        public int GetHashCode([DisallowNull] Point obj)
        {
            return obj.GetHashCode();
        }
    }

    [DebuggerDisplay("Count={_map.Count}")]
    public class PointMap
    {
        private const int MAX_CAP = 1000;

        private readonly Dictionary<Point, int> _map = [];
        private readonly List<Point> _reverseMap = [new()]; // add a place holder point so that the first element will be add at 1 index
        private static int _i = 1;

        /// <summary>
        /// If key exist => Return associate integer number <br/>
        /// Otherwise => Add new
        /// </summary>
        public int LookUp(Point key)
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

        public Point LookUp(int value)
        {
            Ensure.ThrowIf(
                condition: value >= _reverseMap.Count,
                exception: typeof(IndexOutOfRangeException),
                errorMessage: EM.No_Map_At_Index,
                args: [nameof(Node), value]);

            return _reverseMap.ElementAt(value);
        }

        public bool Remove(Point key)
        {
            return _map.Remove(key);
        }
    }
}
