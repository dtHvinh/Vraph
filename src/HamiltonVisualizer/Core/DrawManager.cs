using HamiltonVisualizer.Constants;
using HamiltonVisualizer.GraphUIComponents;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace HamiltonVisualizer.Core
{
    /// <summary>
    /// Drawing manager.
    /// </summary>
    /// <param name="Canvas">The canvas on which this class draws.</param>
    public class DrawManager(Canvas Canvas)
    {
        /// <summary>
        /// Draw a <see cref="Line"/> and add to the collection.
        /// </summary>
        public bool Draw(Node src, Node dst, [NotNullWhen(true)] out Line? line)
        {
            // TODO: this method may return false if something happen

            Line myLine = new()
            {
                Stroke = Brushes.Black,
                X1 = src.Origin.X,
                X2 = dst.Origin.X,
                Y1 = src.Origin.Y,
                Y2 = dst.Origin.Y,
                StrokeThickness = 2,
            };
            // Line is sealed so :(((((((( have to inline.
            Panel.SetZIndex(myLine, ConstantValues.ZIndex.Line);

            myLine.SetValue(RenderOptions.EdgeModeProperty, EdgeMode.Aliased);
            Canvas.Children.Add(myLine);

            src.ReleaseSelectMode();
            dst.ReleaseSelectMode();

            line = myLine;

            return true;
        }
    }
}
