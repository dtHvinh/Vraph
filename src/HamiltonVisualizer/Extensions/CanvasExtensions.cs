using HamiltonVisualizer.Core.CustomControls.WPFLinePolygon;
using System.Windows.Controls;

namespace HamiltonVisualizer.Extensions
{
    public static class CanvasExtensions
    {
        /// <returns>First value is index of head, second value is index of body</returns>
        public static (int, int) Add(this UIElementCollection collection, Edge obj)
        {
            return (collection.Add(obj.Head), collection.Add(obj.Body));
        }

        public static void Remove(this UIElementCollection collection, Edge obj)
        {
            collection.Remove(obj.Head);
            collection.Remove(obj.Body);
        }
    }
}
