using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows;

namespace Stebs.View.CustomControls
{
    /// <summary>
    /// Triangle Custom Control
    /// </summary>
    public class Triangle : Shape
    {
        /// <summary>
        /// Defines the Shape of the Control
        /// </summary>
        protected override Geometry DefiningGeometry {
            get {
                // Create a StreamGeometry for describing the shape
                StreamGeometry geometry = new StreamGeometry();
                geometry.FillRule = FillRule.EvenOdd;

                using(StreamGeometryContext context = geometry.Open()) {
                    InternalDrawTriangleGeometry(context);
                }

                // Freeze the geometry for performance benefits
                geometry.Freeze();

                return geometry;
            }
        }

         /// <summary>
         /// Draws the Triangle-Control
         /// </summary>
         /// <param name="context"></param>
         protected void InternalDrawTriangleGeometry(StreamGeometryContext context) {
             Point pt1 = new Point(Width/2, 0);
             Point pt2 = new Point(Width, Height);
             Point pt3 = new Point(0, Height);

             context.BeginFigure(pt1, true, true);
             context.LineTo(pt2, true, false);
             context.LineTo(pt3, true, false);
         }
    }
}
