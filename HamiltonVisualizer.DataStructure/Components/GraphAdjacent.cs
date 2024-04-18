namespace HamiltonVisualizer.DataStructure.Components
{
    /// <summary>
    /// Class contain the adjacent of vertices in the graph.
    /// </summary>
    /// <typeparam name="TVertex">The type of vertex.</typeparam>
    public class GraphAdjacent<TVertex> where TVertex : notnull
    {
        private readonly Dictionary<TVertex, HashSet<TVertex>> _items = [];

        /// <summary>
        /// Collection of <see cref="Edge{T}"/> instances in the graph.
        /// </summary>
        public HashSet<Edge<TVertex>> Edges = [];

        /// <summary>
        /// Contains unique vertices in the graph. Do not supposed to change manually.
        /// </summary>
        public HashSet<TVertex> Vertices = [];

        /// <summary>
        /// Get the adjacent of the provided <paramref name="vertex"/>.
        /// </summary>
        /// <param name="vertex">The vertex to find it adjacent.</param>
        /// <returns>
        /// If found, return the <see cref="HashSet{T}"/> that contain vertices that is adjacent of the <paramref name="vertex"/>
        /// otherwise return empty collection.
        /// </returns>
        public HashSet<TVertex> GetAdjacentOrEmpty(TVertex vertex)
        {
            if (_items.TryGetValue(vertex, out var lst))
            {
                return lst;
            }

            return [];
        }

        /// <summary>
        /// Remove a vertex.
        /// </summary>
        /// <param name="vertex"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Make <paramref name="target"/> vertex as the adjacent of the <paramref name="source"/> vertex.
        /// </summary>
        /// <remarks>
        /// If the collection does not contain the <paramref name="source"/> vertex. It will create new one and 
        /// add <paramref name="target"/> as its adjacent.
        /// </remarks>
        /// <param name="source">The source vertex.</param>
        /// <param name="target">The vertex to connect.</param>
        /// <returns>True if new item has been created, otherwise false</returns>
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

        /// <summary>
        /// Release the connection between vertices <paramref name="source"/> and <paramref name="target"/> from this graph.
        /// </summary>
        /// <param name="source">The source vertex.</param>
        /// <param name="target">The vertex to connect.</param>
        /// <returns>True if successfully disconnect, otherwise false.</returns>
        public bool Disconnect(TVertex source, TVertex target)
        {
            if (_items.TryGetValue(source, out var lst))
            {
                lst.Remove(target);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Reverse all vertex and its adjacent.
        /// </summary>
        /// <returns>Return new instance of <see cref="GraphAdjacent{TVertex}"/> after reverse vertex and its adjacent.</returns>
        public GraphAdjacent<TVertex> Reverse()
        {
            var rAdjacent = new GraphAdjacent<TVertex>();

            IEnumerable<object> lst = [];

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
