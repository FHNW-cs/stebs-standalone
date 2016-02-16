namespace Stebs.View.CustomControls
{
    using System.Windows.Media;
    using System.Windows.Shapes;
    using System.Windows;
    using System.Windows.Markup;
    using System.Collections.Generic;
    using System.ComponentModel;

    [ContentProperty("Rules")]
    /// <summary>
    /// Base class for the Line and Arrow CustomControls
    /// </summary>
    /// 
    public abstract class LineBase : Shape
    {
        /// <summary>
        /// Holds the configured state of the autohide property uses for this line
        /// </summary>
        public static readonly DependencyProperty AutoHideProperty = DependencyProperty.Register("AutoHide", typeof(bool), typeof(LineBase), new FrameworkPropertyMetadata(false));
        public static readonly DependencyProperty DashedProperty = DependencyProperty.Register("Dashed", typeof(bool), typeof(LineBase), new FrameworkPropertyMetadata(false));
        public static readonly DependencyProperty HighlightColorProperty = DependencyProperty.Register("HighlightColor", typeof(Color), typeof(LineBase));

        /// <summary>
        /// Creates a new instance of a Line
        /// </summary>
        public LineBase()
        {
            Rules = new List<Rule>();
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public List<Rule> Rules { get; private set; }


        /// <summary>
        /// If true, the control is only visible if it is also highlighted(used)
        /// </summary>
        public bool AutoHide
        {
            get { return(bool)base.GetValue(AutoHideProperty); }
            set { base.SetValue(AutoHideProperty, value); }
        }

        /// <summary>
        /// If true, the Line/Arrow is drawn with dashed lines
        /// </summary>
        public bool Dashed
        {
            get { return(bool)base.GetValue(DashedProperty); }
            set { base.SetValue(DashedProperty, value); }
        }


        public Color HighlightColor
        {
            get { return (Color)base.GetValue(HighlightColorProperty); }
            set { base.SetValue(HighlightColorProperty, value); }
        }
         
        /// <summary>
        /// Defines the Shape of the control
        /// </summary>
        protected override Geometry DefiningGeometry
        {
            get {
                // Dashing
                if (Dashed) {
                    DoubleCollection dashes = new DoubleCollection();
                    dashes.Add(2);
                    dashes.Add(4);
                    StrokeDashArray = dashes;
                }

                // Create a StreamGeometry for describing the shape
                StreamGeometry geometry = new StreamGeometry();
                geometry.FillRule = FillRule.EvenOdd;

                using(StreamGeometryContext context = geometry.Open()) {
                    InternalDrawLineGeometry(context);
                }

                // Freeze the geometry for performance benefits
                geometry.Freeze();

                return geometry;
            }
        }


        protected abstract void InternalDrawLineGeometry(StreamGeometryContext context);
    }
}
