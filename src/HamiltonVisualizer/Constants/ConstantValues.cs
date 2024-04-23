using System.Windows.Media;

namespace HamiltonVisualizer.Constants
{
    public class ConstantValues
    {
        public static class Messages
        {
            public const string DeleteAllConfirmMessage = "Xác nhận xóa tất cả!";
            public static readonly string ConfirmBeforeImport = $"Thao tác này cần xóa đồ thị hiện tại {Environment.NewLine}Lưu dữ liệu đồ thị hiện tại trước khi nhập!";
        }

        public static class Control
        {
            public static readonly (int, int, int, int) ModeButtonMarginDefault = (-37, 0, 0, 0);
            public static readonly (int, int, int, int) ModeButtonOn = (39, 0, 0, 0);
        }

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
            public const double DrawingCanvasSidesHeight = 720;
            public const double DrawingCanvasSidesWidth = 1150;
        }

        public static class Time
        {
            public const int TransitionDelay = 500;
        }


        public static class AlgorithmNames
        {
            public const string DFS = "DFS";
            public const string Hamilton = "Hamilton";
        }
    }
}
