using HamiltonVisualizer.Core.CustomControls.WPFBorder;
using System.Windows;

namespace HamiltonVisualizer.Events.EventArgs.ForNode;
internal record class NodePositionChangedEventArgs(Point NewPosition, IEnumerable<Node> CollideNodes);
