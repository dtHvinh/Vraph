using HamiltonVisualizer.Core.CustomControls.Contracts;
using System.Windows.Controls;

namespace HamiltonVisualizer.Core.CustomControls.WPFCanvas
{
    public class DCContextMenu : ContextMenu, IAppContextMenu
    {
        private static DCContextMenu? _instance;

        public MenuItem SCC = null!;
        public MenuItem HamiltonCycle = null!;
        public MenuItem Quit = null!;

        public static DCContextMenu Instance
        {
            get
            {
                _instance ??= new DCContextMenu();
                return _instance;
            }
        }

        public void Initialize()
        {
            SCC = new()
            {
                Header = "Bộ Phận Liên Thông"
            };

            HamiltonCycle = new()
            {
                Header = "Chu trình hamilton"
            };

            Quit = new()
            {
                Header = "Huỷ",

            };
        }

        public DCContextMenu()
        {
            Initialize();

            AddItems(
                SCC,
                HamiltonCycle,
                Quit);
        }

        public void AddItems(params MenuItem[] objects)
        {
            foreach (var item in objects)
            {
                Items.Add(item);
            }
        }
    }
}
