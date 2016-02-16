
namespace Stebs.View.CustomControls
{
    using System.Windows.Media;
    using System.Windows.Shapes;
    using System.Windows;
    using System.Windows.Controls;
    using System.Linq;
    using System.Windows.Markup;
    using System.Collections.Generic;
    using System.ComponentModel;
    
    /// <summary>
    /// Base class for the Line and Arrow CustomControls
    /// </summary>
    /// 

    public class Rule : FrameworkElement
    {
        public static readonly DependencyProperty MipProperty = DependencyProperty.Register("Mip", typeof(string), typeof(Rule), new FrameworkPropertyMetadata(string.Empty));
        public static readonly DependencyProperty FromProperty = DependencyProperty.Register("From", typeof(string), typeof(Rule), new FrameworkPropertyMetadata(string.Empty));
        public static readonly DependencyProperty ToProperty = DependencyProperty.Register("To", typeof(string), typeof(Rule), new FrameworkPropertyMetadata(string.Empty));
        public static readonly DependencyProperty AluCmdProperty = DependencyProperty.Register("Alc", typeof(string), typeof(Rule), new FrameworkPropertyMetadata(string.Empty));
        public static readonly DependencyProperty AffectedFlagProperty = DependencyProperty.Register("Af", typeof(string), typeof(Rule), new FrameworkPropertyMetadata(string.Empty));
        public static readonly DependencyProperty EnableValueProperty = DependencyProperty.Register("Ev", typeof(string), typeof(Rule), new FrameworkPropertyMetadata(string.Empty));
        public static readonly DependencyProperty CifProperty = DependencyProperty.Register("Cif", typeof(string), typeof(Rule), new FrameworkPropertyMetadata(string.Empty));
        public static readonly DependencyProperty RwProperty = DependencyProperty.Register("Rw", typeof(string), typeof(Rule), new FrameworkPropertyMetadata(string.Empty));
        public static readonly DependencyProperty InvertedPropetty = DependencyProperty.Register("Inverted", typeof(bool), typeof(Rule), new FrameworkPropertyMetadata(false));

        /// <summary>
        /// On which next MIP Types the control should be highlighted(DEC,FETCH,JUMP,NOJMP,NEXT,IN)
        /// </summary>
        public string Mip
        {
            get { return (string)base.GetValue(MipProperty); }
            set { base.SetValue(MipProperty, value); }
        }

        public bool AppliesMip(string value)
        {
            return string.Empty.Equals(Mip)
                || Mip.Split(',').Contains(value);
        }


        /// <summary>
        /// On which From Registers the control should be highlighted
        /// </summary>
        public string From
        {
            get { return (string)base.GetValue(FromProperty); }
            set { base.SetValue(FromProperty, value); }
        }

        public bool AppliesFrom(string value)
        {
            return string.Empty.Equals(From)
                || From.Split(',').Contains(value);
        }

        /// <summary>
        /// On which To Registers the control should be highlighted
        /// </summary>
        public string To
        {
            get { return (string)base.GetValue(ToProperty); }
            set { base.SetValue(ToProperty, value); }
        }

        public bool AppliesTo(string value)
        {
           return string.Empty.Equals(To)
               || To.Split(',').Contains(value);
        }
       
        /// <summary>
        /// If the line should only be highlighted wether there's a alu command or not
        /// Possible values are not set string.empty = undefined, bool.TrueString if
        /// highlighting is expected on alu commands and and bool.FalseString
        /// if highlithing shoul be perfomred when no alu command is executed.
        /// </summary>
        public string Alc
        {
            get { return (string)base.GetValue(AluCmdProperty); }
            set { base.SetValue(AluCmdProperty, value); }
        }

        public bool AppliesAlc(string value)
        {
            return string.Empty.Equals(Alc)
                || Alc.Split(',').Contains(value);
        }

        /// <summary>
        /// If the line should be highlighted if the enable value flag
        /// is set or unset. Possible values are string.empty (undefined)
        /// boolean.TrueString (highlight when the enable flag is set)
        /// and boolean.FalseStrng (highlight when the enable flag is unset)
        /// </summary>
        public string Ev
        {
            get { return (string)base.GetValue(EnableValueProperty); }
            set { base.SetValue(EnableValueProperty, value); }
        }

        public bool AppliesEv(string value)
        {
            return string.Empty.Equals(Ev)
                || Ev.Equals(value);
        }

        /// <summary>
        /// Wether the line should only be highlighted if there's an affected
        /// flag set. Possible values are not set (string.empty), bool.TrueString 
        /// (highlight when the affected flag is set) or
        /// bool.FalseString (highlight when the affected flag is unset)
        /// </summary>
        public string Af
        {
            get { return (string)base.GetValue(AffectedFlagProperty); }
            set { base.SetValue(AffectedFlagProperty, value); }
        }

        public bool AppliesAf(string value)
        {
            return string.Empty.Equals(Af)
                || Af.Equals(value);
        }

        /// <summary>
        /// Wether the line should only be highlighted if there's an clear interrupt
        /// flag set. Possible values are not set. Possible values are 
        /// string.empty to indicated undefined state, bool.TrueString which means
        /// highight if the flag is set and bool.FalseString (=highlight when the CIF
        /// is unset)
        /// </summary>
        public string Cif
        {
            get { return (string)base.GetValue(CifProperty); }
            set { base.SetValue(CifProperty, value); }
        }

        public bool AppliesCif(string value)
        {
            return string.Empty.Equals(Cif)
                || Cif.Equals(value);
        }

        ///

        // Wether the line should be highlighted only if the instruction
        // reads or writes.
        public string Rw
        {
            get { return (string)base.GetValue(CifProperty); }
            set { base.SetValue(CifProperty, value); }
        }

        public bool AppliesRw(string value)
        {
            return string.Empty.Equals(Rw)
                || Rw.Split(',').Contains(value);
        }

        public bool Inverted
        {
            get { return (bool)base.GetValue(InvertedPropetty); }
            set { base.SetValue(InvertedPropetty, value); }
        }
    }
}
