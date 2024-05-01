using System.Text;

namespace HamiltonVisualizer.DataStructure.Components
{
    /// <summary>
    /// The strongly connected component class.
    /// </summary>
    public class SCC<TVertex>
    {
        /// <inheritdoc/>
        public List<TVertex> Vertices { get; } = [];

        /// <inheritdoc/>
        public int Count { get => Vertices.Count; }

        /// <inheritdoc/>
        public void Add(TVertex v)
        {
            Vertices.Add(v);
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            StringBuilder sb = new();
            sb.Append('[');
            sb.Append(string.Join(",", Vertices.Select(e => e!.ToString())));
            sb.Append(']');
            return sb.ToString();
        }
    }
}
