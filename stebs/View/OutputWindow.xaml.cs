using System;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace Stebs.View
{
    /// <summary>
    /// Interaction logic for OutputWindow.xaml
    /// </summary>
    public partial class OutputWindow : AvalonDock.DockableContent, IOutputWindow
    {
        private static SolidColorBrush blackBrush = new SolidColorBrush(Colors.Black);
        private static SolidColorBrush redBrush = new SolidColorBrush(Colors.Red);
        private static SolidColorBrush greenBrush = new SolidColorBrush(Colors.Green);

        /// <summary>
        /// Constructor
        /// </summary>
        public OutputWindow() {
            InitializeComponent();

            // Clears the Text in the Textbox
            Clear();
        }

        /// <summary>
        /// Writes normal Text with a black brush
        /// </summary>
        /// <param name="text">text to write</param>
        public void WriteOutput(string text) {
            Message(text, blackBrush);
        }

        /// <summary>
        /// Writes a Error Message with a red Brush
        /// </summary>
        /// <param name="text">Text to write</param>
        public void WriteError(string text) {
            Message(text, redBrush);
        }

        /// <summary>
        /// Writes a Success Message with a green Brush
        /// </summary>
        /// <param name="text">Text to write</param>
        public void WriteSuccess(string text) {
            Message(text, greenBrush);
        }

        /// <summary>
        /// Writes a Message in the Outputwindow-Textbox with the given color
        /// </summary>
        /// <param name="text">Text to write</param>
        /// <param name="color">Color of the Text</param>
        private void Message(string text, SolidColorBrush color) {
            int off = Math.Max(textOutput.Document.ContentStart.GetOffsetToPosition(textOutput.Document.ContentEnd) - 2, 0);

            TextRange range = new TextRange(textOutput.Document.ContentEnd, textOutput.Document.ContentEnd);
            range.Text = text + "\r";

            TextRange mark = new TextRange(textOutput.Document.ContentStart.GetPositionAtOffset(off, LogicalDirection.Forward), textOutput.Document.ContentEnd);
            mark.ApplyPropertyValue(TextElement.ForegroundProperty, color);

            scrlView.ScrollToEnd();
        }

        /// <summary>
        /// Is called by clicking the Clear-Button on the View
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClear_Click(object sender, RoutedEventArgs e) {
            Clear();
        }

        /// <summary>
        /// Clears the Text in the Textbox
        /// </summary>
        public void Clear() {
            textOutput.Document.Blocks.Clear();
        }
    }
}
