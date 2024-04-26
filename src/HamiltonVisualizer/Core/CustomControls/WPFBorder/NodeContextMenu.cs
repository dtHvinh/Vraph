using HamiltonVisualizer.Core.Contracts;
using HamiltonVisualizer.Core.CustomControls.Contracts;
using System.Windows;
using System.Windows.Controls;

#nullable disable
namespace HamiltonVisualizer.Core.CustomControls.WPFBorder
{
    internal class NodeContextMenu : ContextMenu, IUIComponent, IAppContextMenu
    {
        private readonly Node _node;

        public MenuItem Information { get; set; }
        public MenuItem SelectOrReleaseSelect { get; set; }
        public MenuItem Delete { get; set; }
        public MenuItem Algorithms { get; set; }
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

            Algorithms = new() { Header = "Thuật toán" };
            Algorithms.Items.Add(BFS);
            Algorithms.Items.Add(DFS);

            Information = new() { Header = "Thông tin" };
            Information.Click += (sender, e) =>
            {
                MessageBox.Show(_node.ToString("vi"), $"Thông tin");
            };

            AddItems(
                Information,
                SelectOrReleaseSelect,
                Algorithms,
                Delete);
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            _node.DeleteNode();
        }

        private void SelectOrReleaseSelect_Click(object sender, RoutedEventArgs e)
        {
            if (_node.IsSelected)
                _node.OnReleaseSelectMode();
            else
                _node.OnSelectNode();
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
