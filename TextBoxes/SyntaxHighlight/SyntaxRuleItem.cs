using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;


namespace SyntaxHighlight
{
    /// <summary>
    /// Highlight rule
    /// </summary>
    public class SyntaxRuleItem
    {
        /// <summary>
        /// Category of highlight rule
        /// </summary>
        public CodeTokenType RuleType
        {
            get;
            set;
        }

        /// <summary>
        /// Foreground brush
        /// </summary>
        public Brush Foreground
        {
            get;
            set;
        }
    }
}
