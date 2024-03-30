using System.Windows.Controls;

namespace HamiltonVisualizer.Core.CustomControls.Contracts
{
    public interface IAppContextMenu
    {
        void Initialize();
        void AddItems(params MenuItem[] items);
    }
}
