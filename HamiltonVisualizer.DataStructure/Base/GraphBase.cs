#nullable disable

using HamiltonVisualizer.DataStructure.Components;

namespace HamiltonVisualizer.DataStructure.Base
{
    public abstract class GraphBase<TVertex>
    {
        public GraphAdjacent<TVertex> Adjacent { get; }
        public GraphAlgorithm<TVertex> Algorithm { get; }
        public int VertexCount => Adjacent.Vertices.Count;
        public int EdgeCount => Adjacent.Edges.Count;

        protected GraphBase()
        {
            Adjacent = new();
            Algorithm = new(this);
        }

        protected void InternalSetAdjacent(TVertex u, TVertex v)
        {
            if (Adjacent.Connect(u, v))
            {
                Adjacent.Vertices.Add(v);
            }
            else
            {
                Adjacent.Vertices.Add(v);
                Adjacent.Vertices.Add(u);
            }
        }
        protected abstract bool InternalAddEdge(Edge<TVertex> edge);
        protected abstract bool InternalRemove(Edge<TVertex> edge);
        protected abstract int InternalGetWeight(TVertex u, TVertex v);
        public virtual bool ContainEdge(TVertex u, TVertex v)
        {
            return Adjacent.Edges.Contains(new Edge<TVertex>(u, v));
        }
        public virtual bool ContainVertex(TVertex u)
        {
            return Adjacent.Vertices.Contains(u);
        }
        public bool AddEdge(TVertex u, TVertex v, int w)
        {
            return InternalAddEdge(new Edge<TVertex>(u, v, w));
        }
        public bool AddEdge(TVertex u, TVertex v)
        {
            return InternalAddEdge(new Edge<TVertex>(u, v));
        }
        public bool AddEdge(Edge<TVertex> edge)
        {
            return InternalAddEdge(edge);
        }
        public virtual bool RemoveEdge(TVertex u, TVertex v)
        {
            var c = Adjacent.Edges.RemoveWhere(e => e.U.Equals(u) && e.V.Equals(v));
            return c > 0;
        }
        public int GetWeight(TVertex u, TVertex v)
        {
            return InternalGetWeight(u, v);
        }
        public bool TryGetWeight(TVertex u, TVertex v, out int w)
        {
            try
            {
                w = InternalGetWeight(u, v);
            }
            catch
            {
                w = default;
                return false;
            }
            return true;
        }
        public IEnumerable<TVertex> GetAdjacent(TVertex vetex)
        {
            return Adjacent.GetAdjacentOrEmpty(vetex);
        }
        public abstract GraphBase<TVertex> Change();
        public override string ToString()
        {
            return
               $"""
                [NoE: {Adjacent.Edges.Count}; NoV: {Adjacent.Vertices.Count}]
                """;
        }
        public void Clear()
        {
            Adjacent.Edges.Clear();
            Adjacent.Vertices.Clear();
        }
    }
}
