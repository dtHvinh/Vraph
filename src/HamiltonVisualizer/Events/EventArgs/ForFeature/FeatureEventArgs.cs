namespace HamiltonVisualizer.Events.EventArgs.ForFeature.FeatureEventArgs;

internal sealed class DeleteEventArgs(string labels) : System.EventArgs
{
    public string Labels { get; set; } = labels;
}

internal sealed class FindEventArgs(string labels) : System.EventArgs
{
    public string Labels { get; set; } = labels;
}
