namespace Stebs.View
{

    using System.Windows.Input;
    using System.Windows;
    using Microsoft.Win32;
    using System.IO;
    using Stebs.Model;
    using Stebs.ViewModel;
    using NLog;
    using System.Threading.Tasks;
    using System.Threading;

    /// <summary>
    /// Interaction logic for AsmCodeWindow.xaml
    /// </summary>
    public partial class AsmCodeWindow : AvalonDock.DockableContent
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        // State of assembler code window contents: new or saved = true, not saved = false
        private bool isSaved = true;

        public bool ShowHighlightedLine {
            get;
            set;
        }

        private int[] codeLinesPos = new int[256];
        public int[] CodeLinesPos {
            set {
                codeLinesPos = value;
            }
        }

        /// <summary>
        /// The Processer ViewModel
        /// </summary>
        private ProcessorViewModel processorVM;


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="processorVM">The processor ViewModel</param>
 
        /// <param name="fileName">FileName of the File, to open the Window with</param>
        public AsmCodeWindow (ProcessorViewModel processorVM, string fileName) {
            InitializeComponent ();

            this.processorVM = processorVM;

            // Set the IP- or SP-Changed Event
            this.processorVM.ipspChangedEvent += RefreshCurrentLine;

            // Set the SyntaxLexer
            SyntaxTextBox.SyntaxLexer = new AssemblerHighlightingParser (processorVM.proc.DecoderEntries);

            // Open the Startup File
            CurrentFileName = fileName;
            if (string.IsNullOrEmpty (CurrentFileName)) {
                NewCommandHandler ();
            } else {
                LoadASM ();
            }
            RefreshCurrentLine ();
        }


        private int redrawPending = 0;

        /// <summary>
        /// Refresh the Highlighting of the Line of the Current assembler-command
        /// </summary>
        public void RefreshCurrentLine () {
            SyntaxTextBox.InvokeIfRequired (async () => {

                if (Interlocked.CompareExchange(ref redrawPending, 1, 0) == 0)
                {
                    await Task.Delay(100);

                    if (ShowHighlightedLine) {
                        SyntaxTextBox.HighlightedLine = codeLinesPos[processorVM.IP];
                        if (SyntaxTextBox.HighlightedLine >= 0 && SyntaxTextBox.HighlightedLine <= SyntaxTextBox.LineCount) {
                            SyntaxTextBox.ScrollToLine (SyntaxTextBox.HighlightedLine);
                        }
                    } else {
                        SyntaxTextBox.HighlightedLine = -1;
                        SyntaxTextBox.ScrollToHome ();
                    }

                    SyntaxTextBox.InvalidateVisual();
                    Interlocked.CompareExchange(ref redrawPending, 0, 1);
                }
            });
        }

        /// <summary>
        /// Get the Text from the Assembler Code Window
        /// </summary>
        /// <returns>The Text of the TextBox in the Assembler Code Window</returns>
        public string getText () {
            return SyntaxTextBox.Text;
        }

        /// <summary>
        /// Handles the KeyDown-Event in the Search-TextBox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SearchTextBox_KeyDown (object sender, KeyEventArgs e) {
            // Search if Return is pressed
            if (e.Key == Key.Return) {
                string[] words = SearchTextBox.Text.Split (',', ' ', ';', '-');
                SyntaxTextBox.HighlightText.Clear ();

                foreach (string word in words)
                    SyntaxTextBox.HighlightText.Add (word);
            }
        }


        /// <summary>
        /// The current opened Filename
        /// </summary>
        public string CurrentFileName {
            get;
            set;
        }

        /// <summary>
        /// Opens a New File and saves the old file if needed
        /// </summary>
        /// <returns>True, if a new file is open</returns>
        public bool NewCommandHandler () {
            if (!UnsavedHandler ())
                return false;

            CurrentFileName = string.Empty;
            SyntaxTextBox.Text = "";

            this.Title = "Assembler Code Editor - New unsaved file";
            isSaved = true;      // After new nothing is to be saved

            processorVM.Reset ();
            return true;
        }


        /// <summary>
        /// Displays a MessageBox if the changed file is not saved yet
        /// </summary>
        /// <returns>True, if the File was saved</returns>
        public bool UnsavedHandler() {
            if (!isSaved) {
                MessageBoxResult res =
                    MessageBox.Show(
                        "Unsaved changes. Would you like to save them?", "Unsaved content",
                        MessageBoxButton.YesNoCancel, MessageBoxImage.Question
                    );
                switch (res) {
                    case MessageBoxResult.Yes:
                        SaveCommandHandler();
                        break;
                    case MessageBoxResult.No:
                        isSaved = true;
                        break;
                    case MessageBoxResult.Cancel:
                        return false;
                }
            }
            return true;
        }


        /// <summary>
        /// The possible save/no_save_cancel codes
        /// </summary>
        public enum ExitCode {
            NOTHING_TO_SAVE = 0,
            SAVED = 1,
            DONT_SAVE = 2,
            CANCELLED = 3,
        }


        /// <summary>
        /// Displays a MessageBox if exiting the appication and the changed file is not saved yet
        /// </summary>
        /// <returns>Exit code</returns>
        public ExitCode UnsavedExitHandler() {
            if (!isSaved) {
                // not saved
                MessageBoxResult res =
                    MessageBox.Show("Unsaved changes. Would you like to save them?",
                                    "Unsaved content",
                                    MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                switch (res) {
                    case MessageBoxResult.Yes:
                        if (SaveExitCommandHandler() == ExitCode.CANCELLED) {
                            // Answer "Canceled to save file" and therefore: " Canceled to exit application"
                            return ExitCode.CANCELLED;
                        }
                        // Answer: "file saved, about to exit application"
                        return ExitCode.SAVED;
                    case MessageBoxResult.No:
                        // Answer: "file not saved, about to exit application"
                        isSaved = true;
                        return ExitCode.DONT_SAVE;
                    case MessageBoxResult.Cancel:
                        // Answer: "Canceled to exit application"
                        return ExitCode.CANCELLED;
                }
            }
            // already saved
            return ExitCode.SAVED;
        }


        /// <summary>
        /// Displays the OpenFile Dialog and Loads the Assembler File into the TextBox
        /// </summary>
        /// <returns>A RecentFile object with the path and the name of the opened File</returns>
        public RecentFile OpenCommandHandler () {
            if (!UnsavedHandler())
                return null;

            OpenFileDialog o = new OpenFileDialog ();
            o.Filter = "Assembler|*.asm|All files|*.*";
            o.Multiselect = false;
            if (o.ShowDialog () == true) {
                CurrentFileName = o.FileName;
                LoadASM ();
                return new RecentFile () { Title = Path.GetFileName (o.FileName), Path = o.FileName };
            }
            return null;
        }

        /// <summary>
        /// Opens the SaveAs-Dialog if the current File is a new File or Saves the current File
        /// </summary>
        public void SaveCommandHandler () {
            if (CurrentFileName == string.Empty) {
                SaveAsCommandHandler ();
            } else {
                SaveASM ();
            }
        }


        /// <summary>
        /// Handles situation whether to save a file or not when exiting the application.
        /// Answers a code to let the command handler react appropriately, i.e. to close
        /// the application or not.
        /// </summary>
        /// <returns>Exit code</returns>
        public ExitCode SaveExitCommandHandler() {
            if (CurrentFileName == string.Empty) {
                if (SaveAsCommandHandler() == null) {
                    // File saving cancelled
                    return ExitCode.CANCELLED;
                }
                else {
                    // File saved
                    return ExitCode.SAVED;
                }
            }
            else {
                // Saved into old file
                SaveASM();
                return ExitCode.SAVED;
            }
        }


        /// <summary>
        /// Displays the SaveAs-Dialog and saves the Assembler-Code to this File
        /// </summary>
        /// <returns>A RecentFile object with the path and the name of the saved File</returns>
        public RecentFile SaveAsCommandHandler () {
            SaveFileDialog s = new SaveFileDialog ();
            s.Filter = "Assembler|*.asm";
            s.RestoreDirectory = true;
            if (s.ShowDialog () == true) {
                CurrentFileName = s.FileName;
                SaveASM ();
                this.Title = "Assembler Code Editor - Filename: " + Path.GetFileName (CurrentFileName);

                return new RecentFile () { Title = Path.GetFileName (s.FileName), Path = s.FileName };
            }
            return null;
        }

        /// <summary>
        /// Writes the Assembler-Code to the current File
        /// </summary>
        private void SaveASM () {
            using (FileStream stream = new FileStream (CurrentFileName, FileMode.Create)) {
                using (StreamWriter writer = new StreamWriter (stream)) {
                    writer.Write (SyntaxTextBox.Text);
                }
            }
            isSaved = true;
        }

        /// <summary>
        /// Loads the AssemblerCode into the TextBox from the current File
        /// </summary>
        private void LoadASM () {
            try {
                string text;
                using (FileStream stream = new FileStream (CurrentFileName, FileMode.Open,FileAccess.Read)) {
                    using (StreamReader reader = new StreamReader (stream)) {
                        text = reader.ReadToEnd ();
                    }
                }
                SyntaxTextBox.Text = text;
                isSaved = true; // After loading is nothing to save
                this.Title = "Assembler Code Editor - Filename: " + Path.GetFileName (CurrentFileName);
                processorVM.Reset ();
            } catch (FileNotFoundException e) {
                logger.Warn("Error trying to load asm file {0}", e);
                NewCommandHandler ();
            } catch (DirectoryNotFoundException e) {
                logger.Warn("Error trying to load asm file {0}", e);
                NewCommandHandler ();
            }
        }

        /// <summary>
        /// Set isSaved to false, if the Text in the TextBox has changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SyntaxTextBox_TextChanged (object sender, System.Windows.Controls.TextChangedEventArgs e) {
            isSaved = false;
        }

        /// <summary>
        /// Opens a file
        /// </summary>
        /// <param name="fileName">fileName of the File to open</param>
        public void OpenFile (string fileName) {
            CurrentFileName = fileName;
            LoadASM ();
        }
    }
}
