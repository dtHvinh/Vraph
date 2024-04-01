using HamiltonVisualizer.Core;
using System.Windows;

namespace HamiltonVisualizer.Helpers
{
    public static class LinePolygonWrapperRepositionHelper
    {
        public static void Move(Point newPosition, LinePolygonWrapperAttachInfo attachInfo)
        {
            switch (attachInfo.AttachPosition)
            {
                case AttachPosition.Head:
                    attachInfo.LinePolygonWrapper.Body.X1 = newPosition.X;
                    attachInfo.LinePolygonWrapper.Body.Y1 = newPosition.Y;
                    attachInfo.LinePolygonWrapper.UpdateArrowHeadRotation();
                    break;

                case AttachPosition.Tail:
                    attachInfo.LinePolygonWrapper.Body.X2 = newPosition.X;
                    attachInfo.LinePolygonWrapper.Body.Y2 = newPosition.Y;
                    attachInfo.LinePolygonWrapper.UpdateArrowHead(newPosition);
                    break;
            }
        }
    }
}
