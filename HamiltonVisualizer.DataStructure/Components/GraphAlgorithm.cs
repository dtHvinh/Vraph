using HamiltonVisualizer.DataStructure.Base;
using HamiltonVisualizer.DataStructure.Implements;

namespace HamiltonVisualizer.DataStructure.Components
{
    /// <summary>
    /// The class that implement all graph algorithm
    /// </summary>
    /// <typeparam name="TVertex"></typeparam>
    /// <param name="graph"></param>
    public class GraphAlgorithm<TVertex>(GraphBase<TVertex> graph)
        where TVertex : notnull
    {
        private readonly GraphBase<TVertex> _graph = graph;
        private bool IsUndirectedGraph { get; init; } = graph.GetType().GetGenericTypeDefinition() == typeof(UndirectedGraph<>);

        #region Traversal

        public IEnumerable<IEnumerable<TVertex>> BFSLayered(TVertex start)
        {
            var list = new List<List<TVertex>>();

            HashSet<TVertex> visited = [];
            HashSet<TVertex> visited2 = [];

            if (!_graph.ContainVertex(start))
                throw new ArgumentException($"Vertex \"{start}\" not found!");

            Queue<TVertex> queue = [];

            queue.Enqueue(start);

            list.Add([queue.First()]);

            while (queue.Count > 0)
            {
                var front = queue.Dequeue();

                if (!visited.Add(front))
                    continue;

                List<TVertex> layer = [];

                foreach (var v in _graph.Adjacent.GetAdjacentOrEmpty(front))
                {
                    queue.Enqueue(v);
                    if (visited2.Add(v))
                    {
                        layer.Add(v);
                    }
                }
                if (layer.Count > 0)
                    list.Add(layer);
            }
            return list;
        }
        public IEnumerable<TVertex> BFS(TVertex start)
        {
            var list = new List<TVertex>();

            HashSet<TVertex> visited = [];

            if (!_graph.ContainVertex(start))
                throw new ArgumentException($"Vertex \"{start}\" not found!");

            Queue<TVertex> queue = [];
            queue.Enqueue(start);

            while (queue.Count > 0)

            {
                var front = queue.Dequeue();
                if (!visited.Add(front))
                    continue;
                list.Add(front);
                foreach (var v in _graph.Adjacent.GetAdjacentOrEmpty(front))
                {
                    queue.Enqueue(v);
                }
            }
            return list;
        }

        public IEnumerable<TVertex> DFS(TVertex start)
        {
            var list = new List<TVertex>();

            HashSet<TVertex> visited = [];

            if (!_graph.ContainVertex(start))
                throw new ArgumentException($"Vertex \"{start}\" not found!");

            Stack<TVertex> stack = [];
            stack.Push(start);

            while (stack.Count > 0)
            {
                var top = stack.Pop();
                if (visited.Add(top))
                {
                    list.Add(top);

                    foreach (var v in _graph.Adjacent.GetAdjacentOrEmpty(top))
                    {
                        if (!visited.Contains(v))
                            stack.Push(v);
                    }
                }
            }

            return list;
        }

        #endregion Traversal

        #region Connected Component

        /// <inheritdoc/>
        public IEnumerable<SCC<TVertex>> GetComponents()
        {
            if (IsUndirectedGraph)
                return UndirectedGraphGetComponents();
            else
                return KosarajuAlgorithm();
        }

        private List<SCC<TVertex>> UndirectedGraphGetComponents()
        {
            var list = new List<SCC<TVertex>>();
            HashSet<TVertex> visited = [];

            foreach (var u in _graph.Adjacent.Vertices)
            {
                if (visited.Add(u))
                {
                    var scc = new SCC<TVertex>();

                    foreach (var v in DFS(u))
                    {
                        scc.Add(v);
                        visited.Add(v);
                    }

                    list.Add(scc);
                }
            }

            return list;
        }

        /// <inheritdoc/>
        private List<SCC<TVertex>> KosarajuAlgorithm()
        {
            var list = new List<SCC<TVertex>>();

            Stack<TVertex> st = [];
            HashSet<TVertex> visited = [];

            void DFS1(TVertex u)
            {
                if (!visited.Add(u))
                    return;
                foreach (var v in _graph.Adjacent.GetAdjacentOrEmpty(u))
                {
                    if (!visited.Contains(v))
                    {
                        DFS1(v);
                    }
                }
                st.Push(u);
            }

            foreach (var v in _graph.Adjacent.Vertices)
            {
                DFS1(v);
            }

            var rAdjacent = _graph.Adjacent.Reverse();

            visited.Clear();

            while (st.Count > 0)
            {
                var top = st.Pop();

                if (visited.Contains(top))
                    continue;

                var component = new SCC<TVertex>();

                void DFS2(TVertex u)
                {
                    if (visited.Add(u))
                    {
                        component.Add(u);
                        foreach (var v in rAdjacent.GetAdjacentOrEmpty(u))
                        {
                            DFS2(v);
                        }
                    }
                }

                DFS2(top);

                if (component.Count != 0)
                    list.Add(component);
            }

            return list;
        }

        #endregion Connected Component

        #region Cycle and Path

        /// <inheritdoc/>
        private bool HamiltonianCycleUtil(
            TVertex cur,
            HashSet<TVertex> visited,
            List<TVertex> path,
            int count)
        {
            if (count == _graph.VertexCount)
            {
                return _graph.ContainEdge(cur, path[0]);
            }

            foreach (var next in _graph.Adjacent.GetAdjacentOrEmpty(cur))
            {
                if (visited.Add(next))
                {
                    path.Add(next);
                    if (HamiltonianCycleUtil(next, visited, path, count + 1))
                        return true;

                    visited.Remove(next);
                    path.Remove(next);
                }
            }

            return false;
        }

        /// <inheritdoc/>
        private bool HamiltonianPathUtil(
            TVertex cur,
            HashSet<TVertex> visited,
            List<TVertex> path,
            int count)
        {
            if (count == _graph.VertexCount)
            {
                return true;
            }

            foreach (var next in _graph.Adjacent.GetAdjacentOrEmpty(cur))
            {
                if (visited.Add(next))
                {
                    path.Add(next);
                    if (HamiltonianPathUtil(next, visited, path, count + 1))
                        return true;

                    visited.Remove(next);
                    path.Remove(next);
                }
            }

            return false;
        }

        /// <inheritdoc/>
        public List<TVertex> HamiltonianPath()
        {
            foreach (var u in _graph.Adjacent.Vertices)
            {
                List<TVertex> path = [u];
                if (HamiltonianPathUtil(u, [u], path, 1))
                {
                    return path;
                }
            }

            return [];
        }

        /// <inheritdoc/>
        public List<TVertex> HamiltonianCycle()
        {
            foreach (var u in _graph.Adjacent.Vertices)
            {
                List<TVertex> path = [u];
                if (HamiltonianCycleUtil(u, [u], path, 1))
                {
                    return path;
                }
            }

            return [];
        }
        #endregion Cycle and path
    }
}
