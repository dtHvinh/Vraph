using Serilog;

namespace HamiltonVisualizer.Helpers;

public class LogHelper
{
    public static void Colorize(string? nodeLabel, string? nodeColor)
    {
        Log.Information(LogMessage.ColorizeNodeLabelWithColor, nodeLabel, nodeColor);
    }
}

public static class LogMessage
{
    public const string ColorizeNodeLabelWithColor = "Set node with label {0} background color as {1}";
}
