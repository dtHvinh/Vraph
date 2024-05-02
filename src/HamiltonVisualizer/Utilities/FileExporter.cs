using HamiltonVisualizer.Core.Collections;
using HamiltonVisualizer.Core.CustomControls.WPFBorder;
using HamiltonVisualizer.Core.CustomControls.WPFLinePolygon;
using System.IO;

namespace HamiltonVisualizer.Utilities;

internal static class FileExporter
{
    /// <summary>
    /// Write to file specified at <paramref name="path"/>.
    /// </summary>
    /// <returns>The number of elements has been written.</returns>
    public static int WriteTo(string path, ElementCollection data)
    {
        if (data.Nodes.Count == 0)
        {
            var fs = File.Create(path);
            fs.Close();
            return 0;
        }

        using StreamWriter write = new(path);
        int c = 0;

        // Write Nodes
        foreach (Node node in data.Nodes)
        {
            // 24,14:32
            write.WriteLineAsync($"{(node.NodeLabel.Text.Length == 0 ? "null" : node.NodeLabel.Text)},{Math.Round(node.Origin.X)}:{Math.Round(node.Origin.Y)}");
            c++;
        }
        write.WriteLineAsync("--");
        c++;
        foreach (GraphLine edge in data.Edges)
        {
            // 14:32,12:25
            write.WriteLineAsync($"{Math.Round(edge.From.Origin.X)}:{Math.Round(edge.From.Origin.Y)},{Math.Round(edge.To.Origin.X)}:{Math.Round(edge.To.Origin.Y)}");
            c++;
        }

        return c;
    }
}

