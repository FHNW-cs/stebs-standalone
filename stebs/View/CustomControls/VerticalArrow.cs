using System.Windows;
using System.Windows.Media;
using System.Windows.Markup;

namespace Stebs.View.CustomControls
{
    /// <summary>
    /// Vertical Arrow Custom Control
    /// </summary>
    /// 
    public class VerticalArrow : ArrowBase
    {
        public enum VerticalHeadPos {
            Top,
            Bottom,
            Both
        }

        public static readonly DependencyProperty HeadPosProperty = DependencyProperty.Register("VerticalHeadPos", typeof(VerticalHeadPos), typeof(HorizontalArrow), new FrameworkPropertyMetadata(VerticalHeadPos.Bottom, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        /// The Position of the Arrow Head(Top or Bottom)
        /// </summary>
        public VerticalHeadPos HeadPos {
            get { return(VerticalHeadPos)base.GetValue(HeadPosProperty); }
            set { base.SetValue(HeadPosProperty, value); }
        }

        /// <summary>
        /// Draws the Vertical Arrow-Control
        /// </summary>
        /// <param name="context"></param>
        protected override void InternalDrawLineGeometry(StreamGeometryContext context) {
            double x1 = HeadWidth/2;
            double y1 = 0;
            double x2 = HeadWidth/2;
            double y2 = Height;

            MaxWidth = HeadWidth;
            MinWidth = HeadWidth;
            Width = HeadWidth;

            Point pt1 = new Point(x1, y1);
            Point pt2 = new Point(x2, y2);

            context.BeginFigure(pt1, true, false);
            context.LineTo(pt2, true, true);
            

            if (VerticalHeadPos.Top.Equals(HeadPos)
                || VerticalHeadPos.Both.Equals(HeadPos))
            {
                Point pt3 = new Point(
                        pt1.X - HeadWidth / 2,
                        pt1.Y + HeadHeight);
                Point pt4 = new Point(
                        x1 + HeadWidth / 2,
                        y1 + HeadHeight);

                context.BeginFigure(pt3, true, false);
                context.LineTo(pt1, true, false);
                context.LineTo(pt4, true, false);
            }
            
            if (VerticalHeadPos.Bottom.Equals(HeadPos)
               || VerticalHeadPos.Both.Equals(HeadPos))
            {
                Point pt3 = new Point(
                         pt2.X - HeadWidth / 2,
                         pt2.Y - HeadHeight);
                Point pt4 = new Point(
                        pt2.X + HeadWidth / 2,
                        pt2.Y - HeadHeight);

                context.BeginFigure(pt3, true, false);
                context.LineTo(pt2, true, false);
                context.LineTo(pt4, true, false);
            }
        }
    }
}
