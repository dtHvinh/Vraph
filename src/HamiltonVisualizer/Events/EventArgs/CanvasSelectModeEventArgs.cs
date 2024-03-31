namespace HamiltonVisualizer.Events.EventArgs
{
    public class CanvasSelectModeEventArgs
    {
        public CanvasState State { get; set; }
    }

    public enum CanvasState
    {
        Draw,
        Select
    }
}
