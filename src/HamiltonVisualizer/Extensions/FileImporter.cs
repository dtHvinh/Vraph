using HamiltonVisualizer.Core.CustomControls.WPFBorder;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;

namespace HamiltonVisualizer.Extensions;

public static partial class FileImporter
{
    [GeneratedRegex("[0-9]*:[0-9]*")]
    public static partial Regex PresentNode();

    public static void ReadFrom(this MainWindow mainWindow, string path)
    {
        try
        {
            string[] data = File.ReadAllLines(path);

            if (data.Length < 2)
                return;

            EnsureDataFormat.EnsurePresentGraphType(data[0]);

            int i = 2;
            // Start read nodes
            EnsureDataFormat.EnsurePresentNodeSection(data[1]);
            for (; i < data.Length; i++)
            {
                string line = data[i];

                if (EnsureDataFormat.IsPresentNode(line, out string[] parts))
                {
                    string label = parts[0];
                    string[] coordinatePart = parts[1].Split(':');
                    int x = int.Parse(coordinatePart[0]);
                    int y = int.Parse(coordinatePart[1]);

                    Node node = new(mainWindow._canvas, new Point(x, y), mainWindow._elementCollection.Nodes);
                    node.NodeLabel.Text = label.Equals("null") ? $"e{i}" : label;
                    node.NodeLabel.OnLabelSetFinished();
                    if (node.PhysicManager.IsNoCollide())
                        mainWindow.CreateNode(node);
                }
                else
                if (EnsureDataFormat.PresentEdgeSection().IsMatch(line))
                {
                    break; // goto read edges
                }
                else
                {
                    throw new ArgumentException("Lỗi ở dòng " + i);
                }
            }

            Dictionary<string, Node> founded = [];

            // Start read edges
            i += 1;
            for (; i < data.Length; i++)
            {
                string line = data[i];
                var nodeStrings = PresentNode().Matches(line);
                if (nodeStrings.Count != 2)
                {
                    MessageBox.Show("Lỗi ở dòng " + i);
                    return;
                }

                founded.TryGetValue(nodeStrings[0].Value, out Node? node1); // if found just get it from dictionary
                founded.TryGetValue(nodeStrings[1].Value, out Node? node2);

                if (node1 is null)
                {
                    string[] node1StringParts = nodeStrings[0].Value.Split(':');
                    Point point1 = new(int.Parse(node1StringParts[0]), int.Parse(node1StringParts[1]));
                    node1 = mainWindow._elementCollection.Nodes.FirstOrDefault(e => e.Origin.TolerantEquals(point1));
                    if (node1 is null)
                    {
                        throw new Exception();
                    }
                    founded.Add(nodeStrings[0].Value, node1);
                }

                if (node2 is null)
                {
                    string[] node2StringParts = nodeStrings[1].Value.Split(':');
                    Point point2 = new(int.Parse(node2StringParts[0]), int.Parse(node2StringParts[1]));
                    node2 = mainWindow._elementCollection.Nodes.FirstOrDefault(e => e.Origin.TolerantEquals(point2));
                    if (node2 is null)
                    {
                        throw new Exception();
                    }
                    founded.Add(nodeStrings[1].Value, node2);
                }

                mainWindow.CreateLine(node1, node2);
            }
        }
        catch (Exception)
        {
            MessageBox.Show("Lỗi đọc tệp tin!");
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

        public static void EnsurePresentNodeSection(string data)
        {
            if (!PresentNodeSection().IsMatch(data))
                throw new ArgumentException("Invalid data!");
        }

        public static void EnsurePresentGraphLineSection(string data)
        {
            if (!PresentGraphLine().IsMatch(data))
                throw new ArgumentException("Invalid graph line!");
        }

        public static bool IsPresentNode(string data, out string[] parts)
        {
            parts = data.Split(',');
            return parts.Length == 2;
        }

        [GeneratedRegex("^g:[du]$")]
        public static partial Regex PresentGraphType();

        [GeneratedRegex("^n:$")]
        public static partial Regex PresentNodeSection();

        [GeneratedRegex("^[0-9]*:[0-9]*,[0-9]*:[0-9]*$")]
        public static partial Regex PresentGraphLine();

        [GeneratedRegex("^e:$")]
        public static partial Regex PresentEdgeSection();

        private const string InvalidGraphType = "\'{0}\' is invalid, either of \'g:u\'(undirected graph) or \'g:d\'(directed graph) are";
    }
}

