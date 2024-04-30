using System.Windows;

namespace HamiltonVisualizer.Extensions;

internal static class PointExtensions
{
    public static bool TolerantEquals(this Point obj, Point other)
    {
        return ((int)obj.X).Equals((int)other.X) && ((int)obj.Y).Equals((int)other.Y);
    }
}
