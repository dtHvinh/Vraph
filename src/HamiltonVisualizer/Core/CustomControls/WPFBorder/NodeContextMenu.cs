using HamiltonVisualizer.GraphUIComponents.Interfaces;
using System.Windows.Controls;
// TODO: Add instructment window
#nullable disable
namespace HamiltonVisualizer.Core.CustomControls.WPFBorder
{
    public class NodeContextMenu : ContextMenu, IUIComponent
    {
        private readonly Node _node;

        public MenuItem Connect { get; set; }
        public MenuItem Delete { get; set; }

        public NodeContextMenu(Node node)
        {
            Initialize();
            SetUp();
            _node = node;

            StyleUIComponent();
        }

        private void Initialize()
        {
            Connect = new()
            {
                Header = "Chọn...",
            };
            Connect.Click += Connect_Click;

            Delete = new()
            {
                Header = "Xóa"
            };
            Delete.Click += Delete_Click;
        }

        private void Delete_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            _node.DeleteNode();
        }

        private void Connect_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            _node.SelectNode();
        }

        private void SetUp()
        {
            Items.Add(Connect);
            Items.Add(Delete);
        }

        public void StyleUIComponent()
        {
        }
    }
}
