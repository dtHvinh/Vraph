namespace HamiltonVisualizer.Events.EventArgs;

internal class NotificationEventArgs(string message)
{
    public string Message { get; set; } = message;
}
