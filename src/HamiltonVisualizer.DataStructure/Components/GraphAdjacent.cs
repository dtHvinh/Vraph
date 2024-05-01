namespace HamiltonVisualizer.DataStructure.Components
{
    /// <summary>
    /// Class contain the adjacent of vertices in the graph.
    /// </summary>
    /// <typeparam name="TVertex">The type of vertex.</typeparam>
    public class GraphAdjacent<TVertex> where TVertex : notnull
    {
        private readonly Dictionary<TVertex, HashSet<TVertex>> _items = [];
        public HashSet<Edge<TVertex>> Edges = [];
        public HashSet<TVertex> Vertices = [];

        public HashSet<TVertex> GetAdjacentOrEmpty(TVertex vertex)
        {
            if (_items.TryGetValue(vertex, out var lst))
            {
                return lst;
            }

            return [];
        }
        public void AddVertex(TVertex u)
        {
            _items.TryAdd(u, []);
            Vertices.Add(u);
        }
        public void RemoveVertex(TVertex vertex)
        {
            _items.Remove(vertex);
            Vertices.Remove(vertex);
            Edges.RemoveWhere(e => e.U.Equals(vertex) || e.V.Equals(vertex));
            foreach (var adj in _items.Values)
            {
                adj.Remove(vertex);
            }
        }
        public bool Connect(TVertex source, TVertex target)
        {
            if (_items.TryGetValue(source, out var lst))
            {
                lst.Add(target);
                return false;
            }

            _items.Add(source, [target]);
            return true;
        }
        public bool Disconnect(TVertex source, TVertex target)
        {
            if (_items.TryGetValue(source, out var lst))
            {
                lst.Remove(target);
                return true;
            }
            return false;
        }
        public GraphAdjacent<TVertex> Transpose()
        {
            var rAdjacent = new GraphAdjacent<TVertex>();

            foreach (var va in _items)
            {
                var ver = va.Key;
                var adj = va.Value;
                foreach (var v in va.Value)
                {
                    if (rAdjacent._items.TryGetValue(v, out var adjList))
                    {
                        adjList.Add(ver);
                    }
                    else
                    {
                        rAdjacent._items.Add(v, [ver]);
                    }
                }
            }

            return rAdjacent;
        }
    }
}
