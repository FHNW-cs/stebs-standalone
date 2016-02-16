using System.Windows;
using System.Windows.Media;

namespace Stebs.View.CustomControls
{
    /// <summary>
    /// Horizontal Line Custom Control
    /// </summary>
    public class HorizontalLine : LineBase
    {
        /// <summary>
        /// Draws the Horizontal Line-Control
        /// </summary>
        /// <param name="context"></param>
        protected override void InternalDrawLineGeometry(StreamGeometryContext context) {
            double X1 = 0;
            double Y1 = 0;
            double X2 = Width;
            double Y2 = 0;

            MaxHeight = StrokeThickness;
            MinHeight = StrokeThickness;
            Height = StrokeThickness;

            Point pt1 = new Point(X1, Y1);
            Point pt2 = new Point(X2, Y2);

            context.BeginFigure(pt1, true, false);
            context.LineTo(pt2, true, true);
        }
    }
}
