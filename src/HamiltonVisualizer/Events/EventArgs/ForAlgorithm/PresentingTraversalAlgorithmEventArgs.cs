using HamiltonVisualizer.Core.CustomControls.WPFBorder;
using HamiltonVisualizer.DataStructure.Components;

namespace HamiltonVisualizer.Events.EventArgs.ForAlgorithm
{
    internal class PresentingTraversalAlgorithmEventArgs
    {
        /// <summary>
        /// The algorithm name.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// The data.
        /// </summary>
        public IEnumerable<Node> Data { get; set; } = null!;

        /// <summary>
        /// Tell client if it should skip the transition.
        /// </summary>
        public bool SkipTransition { get; set; } = false;
    }

    internal class PresentingLayeredBFSEventArgs
    {
        /// <summary>
        /// The data.
        /// </summary>
        public IEnumerable<BFSComponent<Node>> Data { get; set; } = null!;
    }
}
