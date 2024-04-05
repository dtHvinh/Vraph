using System.Windows.Controls;

namespace HamiltonVisualizer.Core.CustomControls.WPFCanvas
{
    public class CustomCanvas : Canvas
    {
        public CustomCanvas()
        {
            ContextMenu = DCContextMenu.Instance;
        }
    }
}
