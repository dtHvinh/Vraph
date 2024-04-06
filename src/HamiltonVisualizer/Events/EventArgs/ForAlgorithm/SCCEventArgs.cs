using HamiltonVisualizer.Core.CustomControls.WPFBorder;

namespace HamiltonVisualizer.Events.EventArgs.ForAlgorithm
{
    public class SCCEventArgs
    {
        /// <summary>
        /// The algorithm name.
        /// </summary>
        public string? Name { get; set; }

        public IEnumerable<IEnumerable<Node>> Data { get; set; } = null!;

        /// <summary>
        /// Tell client if it should skip the transition.
        /// </summary>
        public bool SkipTransition { get; set; } = false;
    }
}
