using System.Windows;

namespace HamiltonVisualizer.Extensions;

internal static class PointExtensions
{
    /// <summary>
    /// Compare two <see cref="Point"/> with tolerant
    /// </summary>
    /// <remarks>
    /// Because two <see cref="Core.CustomControls.Node.Node"/> are at least 2 time <see cref="Node.NodeWidth"/> distance so as long
    /// as the integer part are equal then they're equal.
    /// </remarks>
    /// <param name="obj"></param>
    /// <param name="other"></param>
    public static bool TolerantEquals(this Point obj, Point other)
    {
        return ((int)obj.X).Equals((int)other.X) && ((int)obj.Y).Equals((int)other.Y);
    }
}
