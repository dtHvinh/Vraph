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
    }
}
