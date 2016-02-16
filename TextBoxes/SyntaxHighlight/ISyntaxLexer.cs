using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace SyntaxHighlight
{

    /// <summary>
    /// Abstract token parser
    /// </summary>
    public abstract class ISyntaxLexer
    {
        protected List<CodeToken> _tokens;

        /// <summary>
        /// Constructor
        /// </summary>
        public ISyntaxLexer()
        {
            _tokens = new List<CodeToken>();
        }

        /// <summary>
        /// Parse code 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="carret_position"></param>
        public abstract void Parse(string text, int caret_position, ICollection<TextChange> changes);


        /// <summary>
        /// List of tokens - result of parsing
        /// </summary>
        public IEnumerable<CodeToken> Tokens
        {
            get
            {
                return _tokens.AsReadOnly();
            }
        }
    }
}
