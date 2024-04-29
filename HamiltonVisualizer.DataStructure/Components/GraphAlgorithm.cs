using HamiltonVisualizer.DataStructure.Base;
using HamiltonVisualizer.DataStructure.Implements;

namespace HamiltonVisualizer.DataStructure.Components
{
    public class GraphAlgorithm<TVertex>(GraphBase<TVertex> graph)
        where TVertex : notnull
    {
        private readonly GraphBase<TVertex> _graph = graph;
        private bool IsUndirectedGraph { get; init; } = graph.GetType().GetGenericTypeDefinition() == typeof(UndirectedGraph<>);

        public IEnumerable<BFSComponent<TVertex>> BFSLayered(TVertex start)
        {
            var list = new List<BFSComponent<TVertex>>();

            HashSet<TVertex> visited = [];
            HashSet<TVertex> cev = []; // component elements visited

            if (!_graph.ContainVertex(start))
                throw new ArgumentException($"Vertex \"{start}\" not found!");

            Queue<TVertex> queue = [];

            queue.Enqueue(start);

            var firstComp = new BFSComponent<TVertex>(start);
            firstComp.AddChild(start);

            list.Add(firstComp);

            while (queue.Count > 0)
            {
                var front = queue.Dequeue();

                if (!visited.Add(front))
                    continue;

                BFSComponent<TVertex> layer = new(front);

                foreach (var v in _graph.GAdj.GetAdjacentOrEmpty(front))
                {
                    queue.Enqueue(v);
                    if (cev.Add(v))
                    {
                        layer.AddChild(v);
                    }
                }
                if (layer.AnyChild())
                    list.Add(layer);
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

                    foreach (var v in _graph.GAdj.GetAdjacentOrEmpty(top))
                    {
                        if (!visited.Contains(v))
                            stack.Push(v);
                    }
                }
            }
            return list;
        }
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

            foreach (var u in _graph.GAdj.Vertices)
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
        private List<SCC<TVertex>> KosarajuAlgorithm()
        {
            var list = new List<SCC<TVertex>>();

            Stack<TVertex> st = [];
            HashSet<TVertex> visited = [];

            void DFS1(TVertex u)
            {
                if (!visited.Add(u))
                    return;
                foreach (var v in _graph.GAdj.GetAdjacentOrEmpty(u))
                {
                    if (!visited.Contains(v))
                    {
                        DFS1(v);
                    }
                }
                st.Push(u);
            }

            foreach (var v in _graph.GAdj.Vertices)
            {
                DFS1(v);
            }

            var rAdjacent = _graph.GAdj.Transpose();

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

        private bool HamiltonianCycleUtil(TVertex cur, HashSet<TVertex> visited, List<TVertex> path, int count)
        {
            if (count == _graph.VertexCount)
            {
                return _graph.ContainEdge(cur, path[0]);
            }

            foreach (var next in _graph.GAdj.GetAdjacentOrEmpty(cur))
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
        public List<TVertex> HamiltonianCycle()
        {
            foreach (var u in _graph.GAdj.Vertices)
            {
                List<TVertex> path = [u];
                if (HamiltonianCycleUtil(u, [u], path, 1))
                {
                    return path;
                }
            }

            return [];
        }
    }
}
