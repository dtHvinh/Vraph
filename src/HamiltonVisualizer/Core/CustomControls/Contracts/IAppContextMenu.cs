using System.Windows.Controls;

namespace HamiltonVisualizer.Core.CustomControls.Contracts
{
    internal interface IAppContextMenu
    {
        void Initialize();
        void AddItems(params MenuItem[] items);
    }
}
