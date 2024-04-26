using System.Windows.Controls;

namespace HamiltonVisualizer.Core.CustomControls.WPFCanvas
{
    internal class CustomCanvas : Canvas
    {
        public CustomCanvas()
        {
            ContextMenu = DCContextMenu.Instance;
        }
    }
}
