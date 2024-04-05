using HamiltonVisualizer.Core.Contracts;
using HamiltonVisualizer.Core.CustomControls.Contracts;
using System.Windows;
using System.Windows.Controls;

#nullable disable
namespace HamiltonVisualizer.Core.CustomControls.WPFBorder
{
    public class NodeContextMenu : ContextMenu, IUIComponent, IAppContextMenu
    {
        private readonly Node _node;

        public MenuItem Information { get; set; }
        public MenuItem SelectOrReleaseSelect { get; set; }
        public MenuItem Delete { get; set; }
        public MenuItem DFS { get; set; }
        public MenuItem BFS { get; set; }

        public NodeContextMenu(Node node)
        {
            _node = node;
            Initialize();
            StyleUIComponent();
        }

        public void Initialize()
        {
            SelectOrReleaseSelect = new();
            SelectOrReleaseSelect.Click += SelectOrReleaseSelect_Click;

            Delete = new() { Header = "Xóa" };
            Delete.Click += Delete_Click;

            BFS = new() { Header = "Duyệt chiều rộng" };
            // Click event is set by MainWindow

            DFS = new() { Header = "Duyệt chiều sâu" };
            // Click event is set by MainWindow

            Information = new() { Header = "Thông tin" };
            Information.Click += (sender, e) =>
            {
                MessageBox.Show(_node.ToString("vi"), $"Đỉnh: [{_node.NodeLabel.Text}]");
            };

            AddItems(
                Information,
                SelectOrReleaseSelect,
                Delete,
                DFS,
                BFS);
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            _node.DeleteNode();
        }

        private void SelectOrReleaseSelect_Click(object sender, RoutedEventArgs e)
        {
            if (_node.IsSelected)
                _node.ReleaseSelectMode();
            else
                _node.SelectNode();
        }

        public void StyleUIComponent()
        {
        }

        public void AddItems(params MenuItem[] items)
        {
            foreach (var item in items)
            {
                Items.Add(item);
            }
        }

        protected override void OnOpened(RoutedEventArgs e)
        {
            if (_node.IsSelected)
            {
                SelectOrReleaseSelect.Header = "Huỷ chọn";
            }
            else
                SelectOrReleaseSelect.Header = "Chọn";

            base.OnOpened(e);
        }
    }
}
