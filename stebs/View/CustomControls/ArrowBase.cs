using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Stebs.View.CustomControls
{
    /// <summary>
    /// Base Class for the Arrow Custom Controls
    /// </summary>
    public abstract class ArrowBase : LineBase
    {
        public static readonly DependencyProperty HeadWidthProperty = DependencyProperty.Register("HeadWidth", typeof(double), typeof(ArrowBase), new FrameworkPropertyMetadata(10.0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));
        public static readonly DependencyProperty HeadHeightProperty = DependencyProperty.Register("HeadHeight", typeof(double), typeof(ArrowBase), new FrameworkPropertyMetadata(10.0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));
        

        /// <summary>
        /// The Width of the Arrow head
        /// </summary>
        public double HeadWidth {
            get { return(double)base.GetValue(HeadWidthProperty); }
            set { base.SetValue(HeadWidthProperty, value); base.SetValue(MinWidthProperty, value*2); }
        }
        /// <summary>
        /// The Height of the Arrow head
        /// </summary>
        public double HeadHeight {
            get { return(double)base.GetValue(HeadHeightProperty); }
            set { base.SetValue(HeadHeightProperty, value); base.SetValue(MinHeightProperty, value*2); }
        }
    }
}
