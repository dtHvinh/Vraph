using System.Diagnostics.CodeAnalysis;

namespace HamiltonVisualizer.DataStructure.Components
{
    /// <summary>
    /// Represents an edge in a graph.
    /// </summary>
    /// <typeparam name="T">The type of vertices in the edge.</typeparam>
    /// <param name="u">The first vertex.</param>
    /// <param name="v">The second vertex.</param>
    /// <param name="w">The weight of the edge (default is 0).</param>
    public class Edge<T>(T u, T v, int w = 0) where T : notnull
    {
        /// <summary>
        /// Gets or sets the first vertex of the edge.
        /// </summary>
        public T U { get; set; } = u;

        /// <summary>
        /// Gets or sets the second vertex of the edge.
        /// </summary>
        public T V { get; set; } = v;

        /// <summary>
        /// Gets or sets the weight of the edge.
        /// </summary>
        public int W { get; set; } = w;

        /// <summary>
        /// Reverses the direction of the edge by swapping Source and Destination.
        /// </summary>
        public void SelfReverse()
        {
            (V, U) = (U, V);
        }

        /// <summary>
        /// Creates a new edge with the direction reversed.
        /// </summary>
        /// <returns>The reversed edge.</returns>
        public Edge<T> Reverse()
        {
            return new Edge<T>(V, U, W);
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current edge.
        /// </summary>
        /// <param name="obj">The object to compare with the current edge.</param>
        /// <returns>True if the objects are equal, otherwise false.</returns>
        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            return obj is Edge<T> edge && edge.U.Equals(U) && edge.V.Equals(V);
        }

        /// <summary>
        /// Determines whether two edges are equal.
        /// </summary>
        /// <param name="left">The first edge to compare.</param>
        /// <param name="right">The second edge to compare.</param>
        /// <returns>True if the edges are equal, otherwise false.</returns>
        public static bool operator ==(Edge<T> left, Edge<T> right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Determines whether two edges are not equal.
        /// </summary>
        /// <param name="left">The first edge to compare.</param>
        /// <param name="right">The second edge to compare.</param>
        /// <returns>True if the edges are not equal, otherwise false.</returns>
        public static bool operator !=(Edge<T> left, Edge<T> right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Returns a hash code for the edge.
        /// </summary>
        /// <returns>A hash code for the edge.</returns>
        public override int GetHashCode()
        {
            return HashCode.Combine(U, V);
        }

        /// <summary>
        /// Returns a string that represents the current edge.
        /// </summary>
        /// <returns>A string representation of the edge.</returns>
        public override string ToString()
        {
            return $"{U} - {V} = {W}";
        }
    }

}
