using HamiltonVisualizer.DataStructure.Base;
using HamiltonVisualizer.DataStructure.Components;

namespace HamiltonVisualizer.DataStructure.Implements
{
    /// <summary>
    /// The <strong>Directed</strong> graph class.
    /// </summary>
    /// <typeparam name="TVertex">The type of vertex.</typeparam>
    public class DirectedGraph<TVertex> : GraphBase<TVertex> where TVertex : notnull
    {
        /// <inheritdoc/>
        protected override bool InternalAddEdge(Edge<TVertex> edge)
        {
            if (!Adjacent.Edges.Add(edge))
            {
                return false;
            }
            Adjacent.AddVertex(edge.U);
            Adjacent.AddVertex(edge.V);
            InternalSetAdjacent(edge.U, edge.V);
            return true;
        }

        /// <inheritdoc/>
        protected override bool InternalRemove(Edge<TVertex> edge)
        {
            if (Adjacent.Edges.Remove(edge))
            {
                Adjacent.Disconnect(edge.U, edge.V);

                Adjacent.RemoveVertex(edge.U);
                Adjacent.RemoveVertex(edge.V);
                return true;
            }
            return false;
        }

        /// <inheritdoc/>
        protected override int InternalGetWeight(TVertex u, TVertex v)
        {
            if (Adjacent.Edges.TryGetValue(new Edge<TVertex>(u, v), out var result))
            {
                return result.W;
            }

            throw new InvalidOperationException();
        }

        /// <summary>
        /// Change to <see cref="UndirectedGraph{TVertex}"/>.
        /// </summary>
        public override GraphBase<TVertex> Change()
        {
            var undirectedGraph = new UndirectedGraph<TVertex>();

            foreach (var edge in Adjacent.Edges)
            {
                undirectedGraph.AddEdge(edge);
            }

            return undirectedGraph;
        }
    }
}
