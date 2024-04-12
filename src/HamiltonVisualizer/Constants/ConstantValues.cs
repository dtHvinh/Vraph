using System.Windows.Media;

namespace HamiltonVisualizer.Constants
{
    public class ConstantValues
    {
        public static class Control
        {
            public static readonly (int, int, int, int) ModeButtonMarginDefault = (-37, 0, 0, 0);
            public static readonly (int, int, int, int) ModeButtonOn = (39, 0, 0, 0);
        }

        /// <summary>
        /// Represents the present in ascending order 
        /// </summary>
        public static class ZIndex
        {
            public const int Line = 1;
            public const int Node = 2;
        }

        public static class ControlColors
        {
            public static readonly SolidColorBrush NodeDefaultBackground = Brushes.White;
            public static readonly SolidColorBrush NodeTraversalBackground = Brushes.Aquamarine;
            public static readonly SolidColorBrush NodeDeleteBackground = Brushes.Red;
            public static readonly SolidColorBrush NodeSelectBackground = Brushes.LightGreen;
        }

        public static class ControlSpecifications
        {
            public const double NodeWidth = 34;

            /// <summary>
            /// The drawing canvas width and height.
            /// </summary>
            public const double DrawingCanvasSidesLength = 1440;
        }

        public static class Time
        {
            public const int TransitionDelay = 500;
        }
    }
}
