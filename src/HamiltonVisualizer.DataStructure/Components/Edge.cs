using System.Diagnostics.CodeAnalysis;

namespace HamiltonVisualizer.DataStructure.Components
{
    public class Edge<T>(T u, T v, int w = 0) where T : notnull
    {
        public T U { get; set; } = u;
        public T V { get; set; } = v;
        public int W { get; set; } = w;

        public void SelfReverse()
        {
            (V, U) = (U, V);
        }
        public Edge<T> Reverse()
        {
            return new Edge<T>(V, U, W);
        }

        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            return obj is Edge<T> edge && edge.U.Equals(U) && edge.V.Equals(V);
        }
        public static bool operator ==(Edge<T> left, Edge<T> right)
        {
            return left.Equals(right);
        }
        public static bool operator !=(Edge<T> left, Edge<T> right)
        {
            return !(left == right);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(U, V);
        }
        public override string ToString()
        {
            return $"{U} - {V} = {W}";
        }
    }

}
