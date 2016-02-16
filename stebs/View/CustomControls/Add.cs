using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows;

namespace Stebs.View.CustomControls
{
    /// <summary>
    /// Add Custom Control
    /// </summary>
    public class Add : Shape
    {
        /// <summary>
        /// Defines the Shape of the CustomControl
        /// </summary>
        protected override Geometry DefiningGeometry {
            get {
                // Create a StreamGeometry for describing the shape
                StreamGeometry geometry = new StreamGeometry();
                geometry.FillRule = FillRule.EvenOdd;

                using(StreamGeometryContext context = geometry.Open()) {
                    InternalDrawAddGeometry(context);
                }

                // Freeze the geometry for performance benefits
                geometry.Freeze();

                return geometry;
            }
        }

        /// <summary>
        /// Draws the Add-Control
        /// </summary>
        /// <param name="context"></param>
         protected void InternalDrawAddGeometry(StreamGeometryContext context) {
             Point pt1 = new Point(Width/6, 0);
             Point pt2 = new Point(Width/6*5, 0);
             Point pt3 = new Point(Width, Height);
             Point pt4 = new Point(Width/6*4, Height);
             Point pt5 = new Point(Width/6*3, Height/4*3);
             Point pt6 = new Point(Width/6*2, Height);
             Point pt7 = new Point(0, Height);

             context.BeginFigure(pt1, true, true);
             context.LineTo(pt2, true, false);
             context.LineTo(pt3, true, false);
             context.LineTo(pt4, true, false);
             context.LineTo(pt5, true, false);
             context.LineTo(pt6, true, false);
             context.LineTo(pt7, true, false);

         }
    }
}
