namespace HamiltonVisualizer.Events.EventArgs.ForAlgorithm
{
    public class PresentingTraversalAlgorithmEventArgs
    {
        /// <summary>
        /// The algorithm name.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// The data.
        /// </summary>
        public object Data { get; set; } = null!;

        /// <summary>
        /// Tell client if it should skip the transition.
        /// </summary>
        public bool SkipTransition { get; set; } = false;
    }
}
