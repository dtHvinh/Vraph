namespace HamiltonVisualizer.Events.EventArgs
{
    public class FinishedExecuteEventArgs
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
        /// Tell client if it should skip the animation.
        /// </summary>
        public bool SkipAnimation { get; set; } = false;
    }
}
