using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows;

namespace Stebs.View.CustomControls
{
    /// <summary>
    /// And Custom Control
    /// </summary>
    public class And : Shape
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
                    InternalDrawAndGeometry(context);
                }

                // Freeze the geometry for performance benefits
                geometry.Freeze();

                return geometry;
            }
        }

         /// <summary>
         /// Draws the And-Control
         /// </summary>
         /// <param name="context"></param>
         protected void InternalDrawAndGeometry(StreamGeometryContext context) {
             Point pt1 = new Point(Width, 0);
             Point pt2 = new Point(Width/2, 0);
             Point pt3 = new Point(-Width/2, Height/2);
             Point pt4 = new Point(Width/2, Height);
             Point pt5 = new Point(Width, Height);
             
             context.BeginFigure(pt1, true, true);
             context.LineTo(pt2, true, false);
             context.BezierTo(pt2, pt3, pt4, true, true);
             context.LineTo(pt5, true, false);
         }
    }
}
