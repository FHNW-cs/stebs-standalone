using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows;

namespace Stebs.View.CustomControls
{
    /// <summary>
    /// ALU Custom Control
    /// </summary>
    public class AluControl : Shape
    {

         protected override Geometry DefiningGeometry {
            get {
                // Create a StreamGeometry for describing the shape
                StreamGeometry geometry = new StreamGeometry();
                geometry.FillRule = FillRule.EvenOdd;

                using(StreamGeometryContext context = geometry.Open()) {
                    InternalDrawAluGeometry(context);
                }

                // Freeze the geometry for performance benefits
                geometry.Freeze();

                return geometry;
            }
        }

         protected void InternalDrawAluGeometry(StreamGeometryContext context) {
             Point pt1 = new Point(0, 0);
             Point pt2 = new Point(Width/6*2, 0);
             Point pt3 = new Point(Width/2, Height/3);
             Point pt4 = new Point(Width/6*4, 0);
             Point pt5 = new Point(Width, 0);
             Point pt6 = new Point(Width/6*5, Height);
             Point pt7 = new Point(Width/6, Height);

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
