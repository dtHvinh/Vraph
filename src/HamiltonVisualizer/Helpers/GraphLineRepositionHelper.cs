using HamiltonVisualizer.Core;
using System.Windows;

namespace HamiltonVisualizer.Helpers
{
    public static class GraphLineRepositionHelper
    {
        public static void Move(Point newPosition, GraphLineConnectInfo attachInfo)
        {
            switch (attachInfo.AttachPosition)
            {
                case ConnectPosition.Head:
                    attachInfo.Edge.Body.X1 = newPosition.X;
                    attachInfo.Edge.Body.Y1 = newPosition.Y;
                    attachInfo.Edge.OnHeadPositionChanged();
                    break;

                case ConnectPosition.Tail:
                    attachInfo.Edge.Body.X2 = newPosition.X;
                    attachInfo.Edge.Body.Y2 = newPosition.Y;
                    attachInfo.Edge.OnTailPositionChanged();
                    break;
            }
        }
    }
}
