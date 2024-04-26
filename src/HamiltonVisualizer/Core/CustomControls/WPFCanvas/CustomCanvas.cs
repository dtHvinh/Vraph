using System.Windows.Controls;

namespace HamiltonVisualizer.Core.CustomControls.WPFCanvas
{
    internal sealed class CustomCanvas : Canvas
    {
        public CustomCanvas()
        {
            ContextMenu = DCContextMenu.Instance;
        }
    }
}
