using HamiltonVisualizer.GraphUIComponents.Interfaces;
using System.Windows.Controls;

#nullable disable
namespace HamiltonVisualizer.GraphUIComponents
{
    public class NodeContextMenu : ContextMenu, IUIComponent
    {
        private readonly Node _node;

        public MenuItem Connect { get; set; }
        public MenuItem Modify { get; set; }
        public MenuItem Delete { get; set; }

        public NodeContextMenu(Node node)
        {
            Initialize();
            SetUp();
            _node = node;
        }

        private void Initialize()
        {
            Connect = new()
            {
                Header = "Nối...",
            };
            Connect.Click += Connect_Click;

            Modify.Click += Modify_Click;

            Delete = new()
            {
                Header = "Xóa"
            };
            Delete.Click += Delete_Click;
        }

        private void Delete_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            _node.DeleteRequest();
        }

        private void Connect_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Modify_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void SetUp()
        {
            Items.Add(Connect);
            Items.Add(Modify);
            Items.Add(Delete);
        }

        public void StyleUIComponent()
        {
        }
    }
}
