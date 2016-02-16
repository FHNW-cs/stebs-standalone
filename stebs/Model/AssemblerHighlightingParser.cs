using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using SyntaxHighlight;
using Stebs.ViewModel;
using System;

namespace Stebs.Model
{
    /// <summary>
    /// Class for parsing the assembler code, to fill the _tokens-List on the SyntaxHighlight-TextBox
    /// </summary>
    public class AssemblerHighlightingParser : ISyntaxLexer
    {
        private char[] anyNewWord = new char[] { ' ', '\n', '\r', ',' };

        /// <summary>
        /// List of all possible assembler-commands
        /// </summary>
        private List<string> commands = new List<string>();

        /// <summary>
        /// Constructor
        /// </summary>
        public AssemblerHighlightingParser(IEnumerable<DecoderEntry> decoderEntries) {
            // Get all the possible assembler-commands from the excel-file and add them to the commands-list
            foreach(DecoderEntry d in decoderEntries)
            {
                string command = d.InstructionType.Mnemonic;

                if (commands.Contains(command) == false) {
                    commands.Add(command);
                }
            }
        }

        /// <summary>
        /// Parse the affected text of the textbox
        /// </summary>
        /// <param name="text">whole text of the text<box/param>
        /// <param name="caret_position">current position of the cursor</param>
        /// <param name="changes">A list with all the changes</param>
        public override void Parse(string text, int caret_position, ICollection<TextChange> changes) {

            if (changes != null) {
                foreach(TextChange change in changes) {
                    // Get the start and end position of the changed lines
                    int start_pos = text.LastIndexOf(Environment.NewLine, change.Offset);
                    if (start_pos < 0) {
                        start_pos = 0;
                    } else {
                        start_pos += 2;
                    }
                    int end_pos = text.IndexOf(Environment.NewLine, change.Offset + change.AddedLength);
                    if (end_pos < 0) {
                        end_pos = text.Length;
                    }

                    // Parse all?
                    if (start_pos == 0 && end_pos == text.Length) {
                        _tokens.Clear();
                    }

                    // Remove all deleted tokens
                    _tokens.RemoveAll(t => { return((t.Start >= change.Offset && t.Start < change.Offset + change.RemovedLength) ||(t.End > change.Offset && t.End <= change.Offset + change.RemovedLength)); });

                    // Move all positions from the tokens behind the change
                    var affectedTokens = from t in _tokens
                                         where t.Start >= change.Offset
                                         select t;
                    foreach(CodeToken t in affectedTokens) {
                        t.Start +=(change.AddedLength - change.RemovedLength);
                        t.End +=(change.AddedLength - change.RemovedLength);
                    }

                    // Remove all tokens in the changed lines
                    _tokens.RemoveAll(t => { return((t.Start >= start_pos && t.Start <= end_pos) ||(t.End >= start_pos && t.End <= end_pos)); });

                    // Parse the changed section again
                    Parse(text, start_pos, end_pos);
                }

            } else {
                // Parse the whole text
                Parse(text, 0, text.Length);
            }
        }

        /// <summary>
        /// Parses the text form start_pos to end_pos
        /// </summary>
        /// <param name="text">Whole text of the textbox</param>
        /// <param name="start_pos">position where to start parsing</param>
        /// <param name="end_pos">position where to stop parsing</param>
        public void Parse(string text, int start_pos, int end_pos) {
            int line_start = start_pos;
            int line_end = 0;

            int word_start = start_pos;
            int word_end = 0;

            int com_start = start_pos;

            do {
                // get the end position of the current line
                line_end = text.IndexOf(Environment.NewLine, line_start);
                if (line_end == -1) {
                    line_end = text.Length;
                }

                do {
                    // get the end position of the current word
                    word_end = text.IndexOfAny(anyNewWord, word_start);
                    if (word_end == -1) {
                        word_end = text.Length;
                    }

                    // check if there is a comment-charakter in the current word
                    com_start = text.IndexOf(';', word_start);
                    if (com_start != -1 && com_start < word_end) {
                        // Remove all tokens from the start of the comment to the end of the line
                        _tokens.RemoveAll(t => { return((t.Start > com_start && t.Start < line_end) ||(t.End > com_start && t.End < line_end)); });
                        // and add the comment token and stop parsing this line
                        _tokens.Add(new CodeToken() { Start = com_start, End = line_end, TokenType = CodeTokenType.Comment });
                        break;
                    }

                    // extract the current word and check if it is a token
                    if (word_end > word_start) {
                        string word = text.Substring(word_start, word_end - word_start);
                        CodeTokenType type = getTokenType(word);
                        if (type != CodeTokenType.None)
                        {
                            _tokens.Add(new CodeToken() { Start = word_start, End = word_end, TokenType = type });
                        }
                    }

                    // set the new word start position
                    word_start = word_end + 1;
                }
                while(word_start <= line_end);

                // set the new line start position
                line_start = line_end+2;
                // set the new word start position
                word_start = line_start;
            } while(line_start <= end_pos);
        }

        /// <summary>
        /// Returns the TokenType of the input-word
        /// </summary>
        /// <param name="word">a word</param>
        /// <returns>the TokenType of the input-word</returns>
        private CodeTokenType getTokenType(string word) {
            int pos = 0;

            word = word.ToUpper();

            // Is the word a keyword(assembler-command)?
            if (word == "END" || commands.Contains(word)) {
                return CodeTokenType.Keyword;
            }

            // Is the word a directive?
            switch (word) {
                case "ORG":
                case "DB":
                    return CodeTokenType.Directive;
            }

            // Is the word a register?
            switch (word) {
                case "AL":
                case "BL":
                case "CL":
                case "DL":
                case "SP":
                    return CodeTokenType.Register;
            }

            // Is the word a HexValue?
            if (word.IsHexValue()) {
                return CodeTokenType.Number;
            }

            // Is the word a string or a char?
            if ((word[0] == '\"' && word[word.Length-1] == '\"') ||(word[0] == '\'' && word[word.Length-1] == '\'')) {
                return CodeTokenType.String;
            }

            // Is the word an address?
            if (word[0] == '[' && word[word.Length-1] == ']') {
                return CodeTokenType.Address;
            }

            // Is the word a label?
            pos = word.IndexOf(":");
            if (pos != -1 && pos == word.Length-1) {
                return CodeTokenType.Label;
            }

            return CodeTokenType.None;
        }
    }
}
