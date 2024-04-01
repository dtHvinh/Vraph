using HamiltonVisualizer.Core;
using System.Windows;

namespace HamiltonVisualizer.Helpers
{
    public static class EdgeRepositionHelper
    {
        public static void Move(Point newPosition, EdgeConnectInfo attachInfo)
        {
            switch (attachInfo.AttachPosition)
            {
                case ConnectPosition.Head:
                    attachInfo.Edge.Body.X1 = newPosition.X;
                    attachInfo.Edge.Body.Y1 = newPosition.Y;
                    attachInfo.Edge.UpdateArrowHeadRotation();
                    break;

                case ConnectPosition.Tail:
                    attachInfo.Edge.Body.X2 = newPosition.X;
                    attachInfo.Edge.Body.Y2 = newPosition.Y;
                    attachInfo.Edge.UpdateArrowHead();
                    break;
            }
        }
    }
}
