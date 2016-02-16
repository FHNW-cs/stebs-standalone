using System.Windows;
using System.Windows.Media;

namespace Stebs.View.CustomControls
{
    /// <summary>
    /// Horizontal Arrow Custom Control
    /// </summary>
    public class HorizontalArrow : ArrowBase
    {
        public enum HorizontalHeadPos {
            Left,
            Right,
            Both
        }

        public static readonly DependencyProperty HeadPosProperty = DependencyProperty.Register("HorizontalHeadPos", typeof(HorizontalHeadPos), typeof(HorizontalArrow), new FrameworkPropertyMetadata(HorizontalHeadPos.Right, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

        // The Position of the Arrow Head(Left or Right)
        public HorizontalHeadPos HeadPos {
            get { return(HorizontalHeadPos)base.GetValue(HeadPosProperty); }
            set { base.SetValue(HeadPosProperty, value); }
        }

        /// <summary>
        /// Draws the Horizontal Arrow-Control
        /// </summary>
        /// <param name="context"></param>
        protected override void InternalDrawLineGeometry(StreamGeometryContext context) {
            double X1 = 0;
            double Y1 = HeadHeight/2;
            double X2 = Width;
            double Y2 = HeadHeight/2;

            MaxHeight = HeadHeight;
            MinHeight = HeadHeight;
            Height = HeadHeight;

            Point pt1 = pt1 = new Point(X1, Y1);
            Point pt2 = new Point(X2, Y2);
            context.BeginFigure(pt1, true, false);
            context.LineTo(pt2, true, false);

            if (HorizontalHeadPos.Left.Equals(HeadPos)
                || HorizontalHeadPos.Both.Equals(HeadPos)
              )
            {
                Point pt3 = new Point(
                        pt1.X + HeadWidth,
                        pt1.Y - HeadHeight / 2);
                Point pt4 = new Point(
                       pt1.X + HeadWidth,
                       pt1.Y + HeadHeight / 2);

                context.BeginFigure(pt3, true, false);
                context.LineTo(pt1, true, false);
                context.LineTo(pt4, true, false);
            }

            if (HorizontalHeadPos.Right.Equals(HeadPos)
               || HorizontalHeadPos.Both.Equals(HeadPos)
             )
            {
                Point pt3 = new Point(
                        pt2.X - HeadWidth,
                        pt2.Y - HeadHeight / 2);
                Point pt4 = new Point(
                       pt2.X - HeadWidth,
                       pt2.Y + HeadHeight / 2);

                context.BeginFigure(pt3, true, false);
                context.LineTo(pt2, true, false);
                context.LineTo(pt4, true, false);
            }

        }
    }
}
