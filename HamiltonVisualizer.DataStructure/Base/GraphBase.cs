#nullable disable

using HamiltonVisualizer.DataStructure.Components;

namespace HamiltonVisualizer.DataStructure.Base
{
    /// <summary>
    /// The base abstract class that represent graph data structure.
    /// </summary>
    /// <typeparam name="TVertex"></typeparam>
    public abstract class GraphBase<TVertex>
    {
        /// <summary>
        /// Contain the apis for management the graph edge, vertex, and vertex adjacent.
        /// </summary>
        public GraphAdjacent<TVertex> Adjacent { get; }

        /// <summary>
        /// Represent the algorithm of this graph.
        /// </summary>
        public GraphAlgorithm<TVertex> Algorithm { get; }

        /// <summary>
        /// The number of vertices.
        /// </summary>
        public int VertexCount => Adjacent.Vertices.Count;

        /// <summary>
        /// The number of edges.
        /// </summary>
        public int EdgeCount => Adjacent.Edges.Count;

        /// <summary>
        /// Construct a new graph.
        /// </summary>
        protected GraphBase()
        {
            Adjacent = new();
            Algorithm = new(this);
        }

        /// <summary>
        /// Update the collection of Adjacent.Vertices and its adjacent.
        /// </summary>
        /// <param name="u">The first vertex.</param>
        /// <param name="v">The second vertex.</param>
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

        /// <summary>
        /// An internal method contain logic for adding new edge to the graph.
        /// </summary>
        /// <param name="edge">The edge to add.</param>
        /// <returns>True if successfully added, otherwise false.</returns>
        protected abstract bool InternalAddEdge(Edge<TVertex> edge);

        /// <summary>
        /// An internal method for removing an edge to the graph.
        /// </summary>
        /// <param name="edge">The edge to remove.</param>
        /// <returns>True if successfully removed, otherwise false.</returns>
        protected abstract bool InternalRemove(Edge<TVertex> edge);


        /// <summary>
        /// An internal method for finding the connection weight between Adjacent.Vertices in the graph.
        /// </summary>
        /// <param name="u">The first vertex.</param>
        /// <param name="v">The second vertex.</param>
        /// <returns>The edge weight if found, otherwise throw exception.</returns>
        /// <exception cref="InvalidOperationException"></exception>
        protected abstract int InternalGetWeight(TVertex u, TVertex v);

        /// <summary>
        /// Check if the graph contain the edge with 2 Adjacent.Vertices <paramref name="u"/> and <paramref name="v"/>.
        /// </summary>
        /// <param name="u">The first vertex.</param>
        /// <param name="v">The second vertex.</param>
        /// <returns>True if the graph contain the edge, otherwise false.</returns>
        public virtual bool ContainEdge(TVertex u, TVertex v)
        {
            return Adjacent.Edges.Contains(new Edge<TVertex>(u, v));
        }

        /// <summary>
        /// Check if the graph contain the specified <paramref name="u"/>.
        /// </summary>
        /// <param name="u">The vertex to check.</param>
        /// <returns>True if the graph contain the vertex, otherwise false.</returns>
        public virtual bool ContainVertex(TVertex u)
        {
            return Adjacent.Vertices.Contains(u);
        }

        /// <summary>
        /// Add new edge to the graph if not exist.
        /// </summary>
        /// <returns>True if successfully added, otherwise false.</returns>
        public bool AddEdge(TVertex u, TVertex v, int w)
        {
            return InternalAddEdge(new Edge<TVertex>(u, v, w));
        }

        /// <summary>
        /// Add new edge to the graph if not exist.
        /// </summary>
        /// <returns>True if successfully added, otherwise false.</returns>
        public bool AddEdge(TVertex u, TVertex v)
        {
            return InternalAddEdge(new Edge<TVertex>(u, v));
        }

        /// <summary>
        /// Add new edge to the graph if not exist.
        /// </summary>
        /// <returns>True if successfully added, otherwise false.</returns>
        public bool AddEdge(Edge<TVertex> edge)
        {
            return InternalAddEdge(edge);
        }

        /// <summary>
        /// Remove an edge.
        /// </summary>
        /// <param name="u">The first vertex.</param>
        /// <param name="v">The second vertex.</param>
        /// <returns></returns>
        public virtual bool RemoveEdge(TVertex u, TVertex v)
        {
            var c = Adjacent.Edges.RemoveWhere(e => e.U.Equals(u) && e.V.Equals(v));
            return c > 0;
        }

        /// <summary>
        /// Get the connection weight between 2 Adjacent.Vertices.
        /// </summary>
        /// <param name="u">The first vertex.</param>
        /// <param name="v">The second vertex.</param>
        /// <returns>The edge weight if found, otherwise throw exception.</returns>
        /// <exception cref="InvalidOperationException"></exception>
        public int GetWeight(TVertex u, TVertex v)
        {
            return InternalGetWeight(u, v);
        }

        /// <summary>
        /// Try to retrieve the connection weight between 2 Adjacent.Vertices.
        /// </summary>
        /// <param name="u">The first vertex.</param>
        /// <param name="v">The second vertex.</param>
        /// <param name="w">The weight of the connection, 0 will return if not found.</param>
        /// <returns>True if the successfully retrieve the connection weight, otherwise false.</returns>
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

        /// <summary>
        /// Returns a string representation of the object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return
               $"""
                [NoE: {Adjacent.Edges.Count}; NoV: {Adjacent.Vertices.Count}]
                """;
        }

        /// <inheritdoc cref="GraphAdjacent{TVertex}.GetAdjacentOrEmpty(TVertex)"/>
        public IEnumerable<TVertex> GetAdjacent(TVertex vetex)
        {
            return Adjacent.GetAdjacentOrEmpty(vetex);
        }

        /// <summary>
        /// Change type.
        /// </summary>
        public abstract GraphBase<TVertex> Change();
    }
}
