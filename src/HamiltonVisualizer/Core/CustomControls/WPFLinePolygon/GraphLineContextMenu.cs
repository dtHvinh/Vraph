using System.Windows.Controls;

namespace HamiltonVisualizer.Core.CustomControls.WPFLinePolygon
{
    public class GraphLineContextMenu(GraphLine line) : ContextMenu
    {
        private readonly GraphLine _line = line;

        public MenuItem Delete { get; set; }

    }
}
