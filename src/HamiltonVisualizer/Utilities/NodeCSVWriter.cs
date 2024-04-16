using HamiltonVisualizer.Core.Collections;
using HamiltonVisualizer.Core.CustomControls.WPFBorder;
using HamiltonVisualizer.Core.CustomControls.WPFLinePolygon;
using System.IO;
using System.Text;

namespace HamiltonVisualizer.Utilities;

public class NodeCSVWriter
{
    public static int WriteTo(string path, GraphElementsCollection data, bool isDirectional)
    {
        int c = 0;

        StringBuilder stringData = new();

        stringData.AppendLine($"g:{(isDirectional ? "d" : "u")}");
        // Write Nodes
        stringData.AppendLine("n:");
        foreach (Node node in data.Nodes)
        {
            // 24,14:32
            stringData.AppendLine($"{(node.NodeLabel.Text.Length == 0 ? "null" : node.NodeLabel.Text)},{Math.Round(node.Origin.X)}:{Math.Round(node.Origin.Y)}");
            c++;
        }

        stringData.AppendLine("e:");
        foreach (GraphLine edge in data.Edges)
        {
            // 14:32,12:25
            stringData.AppendLine($"{Math.Round(edge.From.Origin.X)}:{Math.Round(edge.From.Origin.Y)},{Math.Round(edge.To.Origin.X)}:{Math.Round(edge.To.Origin.Y)}");
            c++;
        }

        File.WriteAllText(path, stringData.ToString());

        return c;
    }
}
