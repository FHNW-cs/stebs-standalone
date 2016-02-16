using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SyntaxHighlight
{
    /// <summary>
    /// Type of syntax rules
    /// </summary>
    public enum CodeTokenType
    {
        Keyword,
        String,
        Number,
        Comment,
        Register,
        Address,
        Label,
        Directive,
        None
    }
}
