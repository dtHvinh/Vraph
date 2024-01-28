namespace HamiltonVisualizer.Events.EventArgs
{
    public class NodeSetLabelEventArgs(string text)
    {
        public string? Text { get; set; } = text;
    }
}
