using HamiltonVisualizer.Extensions;
using System.Diagnostics.CodeAnalysis;
using System.Windows;

namespace HamiltonVisualizer.Core
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

    public class PointMap
    {
        private readonly Dictionary<Point, int> _map = new(PointComparer.Instance);
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
                _map.Add(key, _i++);

            return _i;
        }

        public bool Remove(Point key)
        {
            return _map.Remove(key);
        }
    }
}
