using HamiltonVisualizer.DataStructure.Base;
using HamiltonVisualizer.DataStructure.Components;

namespace HamiltonVisualizer.DataStructure.Implements
{
    /// <summary>
    /// The <strong>Undirected</strong> graph class.
    /// </summary>
    /// <typeparam name="TVertex">The type of vertex.</typeparam>
    public class UndirectedGraph<TVertex> : GraphBase<TVertex> where TVertex : notnull
    {
        /// <inheritdoc/>
        protected override bool InternalAddEdge(Edge<TVertex> edge)
        {
            if (Adjacent.Edges.Contains(edge) || Adjacent.Edges.Contains(edge.Reverse()))
                return false;

            Adjacent.Edges.Add(edge);

            InternalSetAdjacent(edge.U, edge.V);
            InternalSetAdjacent(edge.V, edge.U);

            Adjacent.AddVertex(edge.V);
            Adjacent.AddVertex(edge.U);

            return true;
        }

        /// <inheritdoc/>
        protected override bool InternalRemove(Edge<TVertex> edge)
        {
            var r1 = Adjacent.Edges.Remove(edge);
            var r2 = Adjacent.Edges.Remove(edge.Reverse());

            if (r1 is true)
                Adjacent.Disconnect(edge.U, edge.V);
            if (r2 is true)
            {
                var er = edge.Reverse();
                Adjacent.Disconnect(er.U, er.V);
            }

            Adjacent.RemoveVertex(edge.V);
            Adjacent.RemoveVertex(edge.U);

            return r1 && r2;
        }

        /// <inheritdoc/>
        protected override int InternalGetWeight(TVertex u, TVertex v)
        {
            if (Adjacent.Edges.TryGetValue(new Edge<TVertex>(u, v), out var result))
            {
                return result.W;
            }

            if (Adjacent.Edges.TryGetValue(new Edge<TVertex>(v, u), out result))
            {
                return result.W;
            }

            throw new InvalidOperationException();
        }

        /// <inheritdoc/>
        public override bool RemoveEdge(TVertex u, TVertex v)
        {
            return base.RemoveEdge(v, u) && base.RemoveEdge(u, v);
        }

        /// <summary>
        /// Change to <see cref="DirectedGraph{TVertex}"/>.
        /// </summary>
        public override GraphBase<TVertex> Change()
        {
            var undirectedGraph = new DirectedGraph<TVertex>();

            foreach (var edge in Adjacent.Edges)
            {
                undirectedGraph.AddEdge(edge);
            }

            return undirectedGraph;
        }
    }
}
