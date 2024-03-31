namespace HamiltonVisualizer.Events.EventArgs
{
    public class PresentingAlgorithmEventArgs
    {
        /// <summary>
        /// The algorithm name.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// The data.
        /// </summary>
        public object? Data { get; set; }

        /// <summary>
        /// Tell client if it should skip the transition.
        /// </summary>
        public bool SkipTransition { get; set; } = false;
    }
}
