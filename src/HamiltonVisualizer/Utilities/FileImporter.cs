using HamiltonVisualizer.Core.CustomControls.WPFBorder;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;

namespace HamiltonVisualizer.Extensions;

internal static partial class FileImporter
{
    [GeneratedRegex("[0-9]*:[0-9]*")]
    public static partial Regex PresentNode();

    public static void ReadFrom(this MainWindow mainWindow, string path)
    {
        try
        {
            using StreamReader reader = new(path);

            string? line;
            int i = 0;
            int l = 0;
            while ((line = reader.ReadLine()) != null)
            {
                if (EnsureDataFormat.IsPresentNode(line, out string[] parts))
                {
                    string label = parts[0];
                    string[] coordinatePart = parts[1].Split(':');
                    int x = int.Parse(coordinatePart[0]);
                    int y = int.Parse(coordinatePart[1]);

                    Node node = new(mainWindow._canvas, new Point(x, y), mainWindow.ElementCollection.Nodes);
                    node.NodeLabel.Text = label.Equals("null") ? $"e{i++}" : label;
                    node.NodeLabel.OnLabelSetFinished();

                    if (!node.PhysicManager.HasCollisions())
                        mainWindow.CreateNode(node);
                    l++;
                }
                else if (line.Equals("--"))
                {
                    break; // goto read edges
                }
                else
                {
                    throw new ArgumentException("Lỗi ở dòng " + l);
                }
            }
            l += 1;
            Dictionary<string, Node> founded = [];

            while ((line = reader.ReadLine()) != null)
            {
                var nodeStrings = PresentNode().Matches(line);
                if (nodeStrings.Count != 2)
                {
                    throw new Exception("Lỗi ở dòng " + l);
                }

                founded.TryGetValue(nodeStrings[0].Value, out Node? node1); // if found just get it from dictionary
                founded.TryGetValue(nodeStrings[1].Value, out Node? node2);

                if (node1 is null)
                {
                    string[] node1StringParts = nodeStrings[0].Value.Split(':');
                    Point point1 = new(int.Parse(node1StringParts[0]), int.Parse(node1StringParts[1]));
                    node1 = mainWindow.ElementCollection.Nodes.FirstOrDefault(e => e.Origin.TolerantEquals(point1));
                    if (node1 is null)
                    {
                        throw new Exception(mainWindow.ElementCollection.Nodes.First().ToString());
                        throw new Exception($"Đỉnh ({node1StringParts[0]};{node1StringParts[1]}) không tìm thấy" + " Lỗi ở dòng " + l);
                    }
                    founded.Add(nodeStrings[0].Value, node1);
                }

                if (node2 is null)
                {
                    string[] node2StringParts = nodeStrings[1].Value.Split(':');
                    Point point2 = new(int.Parse(node2StringParts[0]), int.Parse(node2StringParts[1]));
                    node2 = mainWindow.ElementCollection.Nodes.FirstOrDefault(e => e.Origin.TolerantEquals(point2));
                    if (node2 is null)
                    {
                        throw new Exception($"Đỉnh ({node2StringParts[0]};{node2StringParts[1]}) không tìm thấy" + " Lỗi ở dòng " + l);
                    }
                    founded.Add(nodeStrings[1].Value, node2);
                }

                mainWindow.CreateLine(node1, node2);
                l++;
            }
        }
        catch (Exception e)
        {
            MessageBox.Show(e.Message);
            return;
        }
    }

    private static partial class EnsureDataFormat
    {
        public static void EnsurePresentGraphType(string data)
        {
            if (!PresentGraphType().IsMatch(data))
                throw new ArgumentException(string.Format(InvalidGraphType, data));
        }

        public static bool IsPresentNode(string data, out string[] parts)
        {
            parts = data.Split(',');
            return parts.Length == 2;
        }

        [GeneratedRegex("^g:[du]$")]
        public static partial Regex PresentGraphType();

        [GeneratedRegex("^[0-9]*:[0-9]*,[0-9]*:[0-9]*$")]
        public static partial Regex PresentGraphLine();

        private const string InvalidGraphType = "\'{0}\' is invalid, either of \'g:u\'(undirected graph) or \'g:d\'(directed graph) are";
    }
}

