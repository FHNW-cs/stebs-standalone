using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows;

namespace Stebs.View.CustomControls
{
    /// <summary>
    /// Mux Custom Control
    /// </summary>
    public class Mux : Shape
    {
        /// <summary>
        /// An enum with the possible directions
        /// </summary>
        public enum Directions
        {
            Top,
            Right,
            Bottom,
            Left
        }

        /// <summary>
        /// Property for the Direction
        /// </summary>
        public static readonly DependencyProperty DirectionProperty = DependencyProperty.Register("Direction", typeof(Directions), typeof(Mux), new FrameworkPropertyMetadata(Directions.Top, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        /// The Direction of the Control
        /// </summary>
        public Directions Direction {
            get { return(Directions)base.GetValue(DirectionProperty); }
            set { base.SetValue(DirectionProperty, value); }
        }

        /// <summary>
        /// Defines the Shape of the Control
        /// </summary>
        protected override Geometry DefiningGeometry {
            get {
                // Create a StreamGeometry for describing the shape
                StreamGeometry geometry = new StreamGeometry();
                geometry.FillRule = FillRule.EvenOdd;

                using(StreamGeometryContext context = geometry.Open()) {
                    InternalDrawMuxGeometry(context);
                }

                // Freeze the geometry for performance benefits
                geometry.Freeze();

                return geometry;
            }
        }

        /// <summary>
        /// Draws the Mux-Control
        /// </summary>
        /// <param name="context"></param>
        protected void InternalDrawMuxGeometry(StreamGeometryContext context) {
             Point pt1 = new Point();
             Point pt2 = new Point();
             Point pt3 = new Point();
             Point pt4 = new Point();

             switch (Direction) {
                 case Directions.Top:
                     pt1 = new Point(Width/6, 0);
                     pt2 = new Point(Width/6*5, 0);
                     pt3 = new Point(Width, Height);
                     pt4 = new Point(0, Height);
                     break;
                 case Directions.Right:
                     pt1 = new Point(0, 0);
                     pt2 = new Point(Width, Height/6);
                     pt3 = new Point(Width, Height/6*5);
                     pt4 = new Point(0, Height);
                     break;
                 case Directions.Bottom:
                     pt1 = new Point(0, 0);
                     pt2 = new Point(Width, 0);
                     pt3 = new Point(Width/6*5, Height);
                     pt4 = new Point(Width/6, Height);
                     break;
                 case Directions.Left:
                     pt1 = new Point(0, Height/6);
                     pt2 = new Point(Width, 0);
                     pt3 = new Point(Width, Height);
                     pt4 = new Point(0, Height/6*5);
                     break;
             }

             context.BeginFigure(pt1, true, true);
             context.LineTo(pt2, true, false);
             context.LineTo(pt3, true, false);
             context.LineTo(pt4, true, false);

         }
    }
}
