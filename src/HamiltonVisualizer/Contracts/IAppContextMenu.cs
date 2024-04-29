using System.Windows.Controls;

namespace HamiltonVisualizer.Contracts
{
    internal interface IAppContextMenu
    {
        void Initialize();
        void AddItems(params MenuItem[] items);
    }
}
