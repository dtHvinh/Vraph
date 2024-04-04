using System.Windows;
using System.Windows.Controls;

namespace HamiltonVisualizer.Core.CustomControls.WPFLinePolygon
{
    public class GraphLineContextMenu : ContextMenu
    {
        private readonly GraphLine _line;

        public MenuItem Delete { get; set; }

        public GraphLineContextMenu(GraphLine line)
        {
            _line = line;

            Delete = new()
            {
                Header = "Xóa"
            };
            Items.Add(Delete);
            Delete.Click += Delete_Click;
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            _line.Delete();
        }
    }
}
