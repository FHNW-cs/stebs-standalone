namespace Stebs.View {
    using System;
    using System.Windows;
    using System.Windows.Media.Imaging;
    using AvalonDock;
    using Microsoft.Win32;
    using System.Windows.Controls.Ribbon;
    using System.IO;
    using Stebs.ViewModel;
    using Stebs.Model;
    using System.Text;
    using System.Threading;
    using System.Linq;
    using System.Windows.Input;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using NLog;
    using Stebs.IO;
    using System.Security;

    using assembler;
    using assembler.support;


    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        /// <summary>
        /// The window which displays the RAM values
        /// </summary>
        private RAMWindow ramWindow;

        /// <summary>
        /// The window which contains the editable assembler code
        /// </summary>
        private AsmCodeWindow asmCodeWindow;

        /// <summary>
        /// The window which shows the assembler output
        /// </summary>
        private OutputWindow outputWindow = new OutputWindow();

        /// <summary>
        /// The window which shows the register values
        /// </summary>
        private RegistersWindow registersWindow;

        /// <summary>
        /// The windows which shows the CPU architecture
        /// </summary>
        private ArchitectureWindow architectureWindow;

        /// <summary>
        /// User hints
        /// </summary>
        private Hints hints;

        /// <summary>
        /// Logging instance of the main window
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The processor ViewModel
        /// </summary>
        private ProcessorViewModel processorVM;


        /// <summary>
        /// Gets the speed from the processor ViewModel
        /// used for the binding in the Ribbons
        /// </summary>
        public int Speed {
            get { return processorVM.Speed; }
            set { processorVM.Speed = value; }
        }

        /// <summary>
        /// Contains the list of files which were recently used
        /// </summary>
        private ObservableCollection<RecentFile> mostRecentFiles = new ObservableCollection<RecentFile>();

        /// <summary>
        /// List of the recently used files
        /// </summary>
        public ObservableCollection<RecentFile> MostRecentFiles {
            get { return mostRecentFiles; }
        }

        /// <summary>
        /// Contains the reader for the instructions etc.
        /// </summary>
        private static string instructionString = null;

        /// <summary>
        /// The reader with all the instructions etc. from file  INSTRUCTION.data
        /// </summary>
        public static string InstructionString {
            get { return instructionString; }
        }
  
        /// <summary>
        /// Constructor
        /// </summary>
        public MainWindow() {
            logger.Trace("Constructor MainWindow invoked:");
            try {
                InitializeComponent();

                try {
                    IProcessorParser parser = GetProcessorParser();
                    logger.Info("Parser initialized");

                    // Creates all the windows
                    processorVM = new ProcessorViewModel(parser.Parse());
                    logger.Info("ProcessorViewModel initialized");

                    architectureWindow = new ArchitectureWindow(processorVM);
                    registersWindow = new RegistersWindow(processorVM);
                    ramWindow = new RAMWindow(processorVM);

                    String fileName = Properties.Settings.Default.LastFile;
                    if (App.mArgs.Length > 0) {
                        fileName = App.mArgs[0];
                        if (!Path.IsPathRooted(fileName)) {
                            fileName = Path.Combine(App.realAppStartPath, fileName);
                            fileName = Path.GetFullPath(fileName);
                        }
                    }

                    asmCodeWindow = new AsmCodeWindow(processorVM, fileName);

                    // Load and parse the Hints-XML
                    hints = new Hints();

                    logger.Trace("Add IO windows");
                    // Load and add the IO-plugins
                    foreach (IOWrapperWindow ioW in IOWindowLoader.LoadPlugins()) {
                        AddIOWindow(ioW);
                    }

                    // Add the other windows
                    ArchitecturePane.Items.Add(architectureWindow);
                    RAMPane.Items.Add(ramWindow);
                    MacroPane.Items.Add(registersWindow);
                    AssemblerPane.Items.Add(asmCodeWindow);
                    OutputPane.Items.Add(outputWindow);
                    logger.Info("Windows added");

                    CheckControls();

                    // Set the DataContext to itself
                    DataContext = this;
                    LoadProperties();
                    InitCommands();
                }
                catch (FileNotFoundException ex) {
                    ShowFileNotFoundDialog(ex);
                    Process.GetCurrentProcess().Kill();
                }
                catch (SecurityException ex) {
                    ShowSecurityProblemDialog(ex);
                    Process.GetCurrentProcess().Kill();
                }
                catch (UnauthorizedAccessException ex) {
                    ShowUnauthorisedAccessDialog(ex);
                    Process.GetCurrentProcess().Kill();
                }
                catch (ProcessorParserException ex) {
                    ShowProcessorParserException(ex);
                    Process.GetCurrentProcess().Kill();
                }
            }
            catch (Exception e) {
                ShowUnhandledExceptionDialog(e);
                Process.GetCurrentProcess().Kill();
            }
        }

        /// <summary>
        /// Shows an error dialog which displays a message that there is
        /// something wrong in the processor parser data.
        /// </summary>
        /// <param name="ex">The Exception which triggered the problem</param>
        private static void ShowProcessorParserException(ProcessorParserException ex) {
            logger.Fatal("Terminate stebs: {0}", ex);
            StringBuilder errorMsg = new StringBuilder();
            errorMsg.AppendLine("Processor data exception");
            errorMsg.AppendLine("There are invalid values in the processor stearing files.");
            errorMsg.AppendLine("which can't be understood by stebs. Please contact your");
            errorMsg.AppendLine("tutors and show them this message.");
        }

        /// <summary>
        /// Shows an error dialog that displays a program error
        /// (an uncaught exception).
        /// </summary>
        /// <param name="ex">The Exception which triggered the problem</param>
        private static void ShowUnhandledExceptionDialog(Exception e) {
            logger.Fatal("Problem with initializing stebs: {0}", e);
            StringBuilder errorMsg = new StringBuilder();
            errorMsg.AppendLine("Oops, this is an unhandled error. ");
            errorMsg.AppendLine("The program has to be terminated.");
            errorMsg.AppendLine("The error message was: ");
            errorMsg.AppendLine(e.Message);
            errorMsg.AppendLine(e.StackTrace);
            MessageBox.Show(errorMsg.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        /// <summary>
        /// Shows an error dialog that an access was unauthorised while executing stebs.
        /// </summary>
        /// <param name="ex">The Exception which triggered the problem</param>
        private static void ShowUnauthorisedAccessDialog(UnauthorizedAccessException ex) {
            logger.Fatal("Terminate stebs: {0}", ex);
            StringBuilder errorMsg = new StringBuilder();
            errorMsg.AppendLine("Unauthorized access");
            errorMsg.AppendLine("An access required by stebs is not allowed. This may");
            errorMsg.AppendLine("happen, if the path to a file is protected by the operating system");
            errorMsg.AppendLine("(e.g. if the file or directory is read resp. write protected).");
            errorMsg.AppendLine();
            errorMsg.AppendLine("Please reinstall stebs (e.g. on a local directory.) or change the");
            errorMsg.AppendLine("required permissions.");
            MessageBox.Show(errorMsg.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        /// <summary>
        /// Shows an error dialog which displays a message that 
        /// there's a security problem while executing stebs.
        /// </summary>
        /// <param name="ex">The Exception which triggered the problem</param>
        private static void ShowSecurityProblemDialog(SecurityException ex) {
            logger.Fatal("Terminate stebs: {0}", ex);
            StringBuilder errorMsg = new StringBuilder();
            errorMsg.AppendLine("Not enough permission to execute stebs.");
            errorMsg.AppendLine("Please correct the execution permissions");
            errorMsg.AppendLine("or try to run stebs as administrator.");
            MessageBox.Show(errorMsg.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        /// <summary>
        /// Shows an error dialog which displays a message that a file could not be found.
        /// </summary>
        /// <param name="ex">The Exception which triggered the problem</param>
        private static void ShowFileNotFoundDialog(FileNotFoundException ex) {
            StringBuilder errorMsg = new StringBuilder();
            errorMsg.AppendLine(String.Format("Can't find the file: {0}.", ex.FileName));
            errorMsg.AppendLine("Please reinstall stebs.");
            MessageBox.Show(errorMsg.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            logger.Fatal("Terminate stebs: {0}", ex);
        }


        /// <summary>
        /// Creates a new processor parser.
        /// </summary>
        /// <returns></returns>
        private static IProcessorParser GetProcessorParser() {
            StreamReader rom1Reader = null;
            StreamReader rom2Reader = null;
            StreamReader instrReader = null;
            logger.Trace("Initialize Streamreader for datafiles rom1, rom2 and instruction");
            rom1Reader = new StreamReader(GetFileStream(@"res/rom1.data"));
            rom2Reader = new StreamReader(GetFileStream(@"res/rom2.data"));
            instrReader = new StreamReader(GetFileStream(@"res/instruction.data"));
            logger.Trace("StreamReaders initialized");

            // Added for Java assembler: Read instructions etc.
            StreamReader instructionReader = new StreamReader(GetFileStream(@"res/instruction.data"));
            instructionString = instructionReader.ReadToEnd();
            logger.Trace("INSTRUCTION.data read");

            return new RawParser(rom1Reader, rom2Reader, instrReader);
        }


        /// <summary>
        /// Opens a file stream for reading.
        /// </summary>
        /// <param name="filepath">The file</param>
        /// <returns></returns>
        private static Stream GetFileStream(String filepath) {
            return new FileStream(filepath, FileMode.Open, FileAccess.Read, FileShare.Read);
        }


        /// <summary>
        /// Loads the default property values which are stored  
        /// to remember the last settings.
        /// </summary>
        private void LoadProperties() {
            logger.Trace("Method invoked: LoadProperties");
            processorVM.Speed = Properties.Settings.Default.Speed;
            architectureWindow.ScaleSlider.Value = Properties.Settings.Default.ZoomArchitecture;
            outputWindow.ScaleSlider.Value = Properties.Settings.Default.ZoomOutput;
            registersWindow.ScaleSlider.Value = Properties.Settings.Default.ZoomRegisters;
            ramWindow.ScaleSlider.Value = Properties.Settings.Default.ZoomRAM;
            asmCodeWindow.ScaleSlider.Value = Properties.Settings.Default.ZoomCode;
            foreach (string s in Properties.Settings.Default.RecentFileList.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries)) {
                MostRecentFiles.Add(new RecentFile() { Title = Path.GetFileName(s), Path = s });
            }
        }


        /// <summary>
        /// Initialises all commands which can be executed by the user (including the
        /// keyboard shortcuts).
        /// </summary>
        private void InitCommands() {
            logger.Trace("Initialise key bindings");
            RoutedCommand ResetCmd = new RoutedCommand();
            this.CommandBindings.Add(new CommandBinding(ResetCmd, ResetExecuted));
            this.InputBindings.Add(new InputBinding(ResetCmd, new KeyGesture(Key.F2)));
            Reset.Command = ResetCmd;

            RoutedCommand AssembleCmd = new RoutedCommand();
            this.CommandBindings.Add(new CommandBinding(AssembleCmd, AssembleExecuted));
            this.InputBindings.Add(new InputBinding(AssembleCmd, new KeyGesture(Key.F4)));
            Assemble.Command = AssembleCmd;

            RoutedCommand InstructionStepCmd = new RoutedCommand();
            this.CommandBindings.Add(new CommandBinding(InstructionStepCmd, InstructionStepExecuted, Run_StepCanExecute));
            this.InputBindings.Add(new InputBinding(InstructionStepCmd, new KeyGesture(Key.F9)));
            InstructionStep.Command = InstructionStepCmd;

            RoutedCommand MacroStepCmd = new RoutedCommand();
            this.CommandBindings.Add(new CommandBinding(MacroStepCmd, MacroStepExecuted, Run_StepCanExecute));
            this.InputBindings.Add(new InputBinding(MacroStepCmd, new KeyGesture(Key.F10)));
            MacroStep.Command = MacroStepCmd;

            RoutedCommand MicroStepCmd = new RoutedCommand();
            this.CommandBindings.Add(new CommandBinding(MicroStepCmd, MicroStepExecuted, Run_StepCanExecute));
            this.InputBindings.Add(new InputBinding(MicroStepCmd, new KeyGesture(Key.F11)));
            MicroStep.Command = MicroStepCmd;

            RoutedCommand RunInstructionCmd = new RoutedCommand();
            this.CommandBindings.Add(new CommandBinding(RunInstructionCmd, RunInstructionExecuted, Run_StepCanExecute));
            this.InputBindings.Add(new InputBinding(RunInstructionCmd, new KeyGesture(Key.F5)));
            RunInstruction.Command = RunInstructionCmd;

            RoutedCommand RunMacroCmd = new RoutedCommand();
            this.CommandBindings.Add(new CommandBinding(RunMacroCmd, RunMacroExecuted, Run_StepCanExecute));
            this.InputBindings.Add(new InputBinding(RunMacroCmd, new KeyGesture(Key.F6)));
            RunMacro.Command = RunMacroCmd;

            RoutedCommand RunMicroCmd = new RoutedCommand();
            this.CommandBindings.Add(new CommandBinding(RunMicroCmd, RunMicroExecuted, Run_StepCanExecute));
            this.InputBindings.Add(new InputBinding(RunMicroCmd, new KeyGesture(Key.F7)));
            RunMicro.Command = RunMicroCmd;

            RoutedCommand Continue_PauseCmd = new RoutedCommand();
            this.CommandBindings.Add(new CommandBinding(Continue_PauseCmd, Continue_PauseExecuted, Continue_PauseCanExecute));
            this.InputBindings.Add(new InputBinding(Continue_PauseCmd, new KeyGesture(Key.F8)));
            Continue.Command = Continue_PauseCmd;
            Pause.Command = Continue_PauseCmd;

            RoutedCommand RestartCmd = new RoutedCommand();
            this.CommandBindings.Add(new CommandBinding(RestartCmd, RestartExecuted, RestartCanExecute));
            this.InputBindings.Add(new InputBinding(RestartCmd, new KeyGesture(Key.F3)));
            Restart.Command = RestartCmd;

            RoutedCommand OpenCmd = new RoutedCommand();
            this.CommandBindings.Add(new CommandBinding(OpenCmd, OpenExecuted));
            this.InputBindings.Add(new InputBinding(OpenCmd, new KeyGesture(Key.O, ModifierKeys.Control)));
            MenuItemOpen.Command = OpenCmd;

            RoutedCommand NewCmd = new RoutedCommand();
            this.CommandBindings.Add(new CommandBinding(NewCmd, NewExecuted));
            this.InputBindings.Add(new InputBinding(NewCmd, new KeyGesture(Key.N, ModifierKeys.Control)));
            MenuItemNew.Command = NewCmd;

            RoutedCommand SaveCmd = new RoutedCommand();
            this.CommandBindings.Add(new CommandBinding(SaveCmd, SaveExecuted));
            this.InputBindings.Add(new InputBinding(SaveCmd, new KeyGesture(Key.S, ModifierKeys.Control)));
            MenuItemSave.Command = SaveCmd;

            RoutedCommand SaveAsCmd = new RoutedCommand();
            this.CommandBindings.Add(new CommandBinding(SaveAsCmd, SaveAsExecuted));
            this.InputBindings.Add(new InputBinding(SaveAsCmd, new KeyGesture(Key.S, ModifierKeys.Alt)));
            MenuItemSaveAs.Command = SaveAsCmd;

            RoutedCommand HelpCmd = new RoutedCommand();
            this.CommandBindings.Add(new CommandBinding(HelpCmd, HelpExecuted));
            this.InputBindings.Add(new InputBinding(HelpCmd, new KeyGesture(Key.F1)));
            Help.Command = HelpCmd;

            RoutedCommand HintCmd = new RoutedCommand();
            this.CommandBindings.Add(new CommandBinding(HintCmd, HintExecuted));
            this.InputBindings.Add(new InputBinding(HintCmd, new KeyGesture(Key.F12)));
            Hint.Command = HintCmd;
            logger.Trace("Key bindings are set");

            // Command to handle "application closing calls from icon", i.e. double-clicking left handside icon
            CommandManager.RegisterClassCommandBinding(
                typeof(MainWindow), new CommandBinding(ApplicationCommands.Close, CloseApplicationExecuted)
            );
        }


        /// <summary>
        /// Callback handler to be invoked when the application is closed.
        /// Saves the currently open file, the properties and the layout.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closed(object sender, EventArgs e) {
            logger.Info("Method Window_Closed invoked:");

            // Save the Layout
            try {
                dockManager.SaveLayout(Properties.Settings.Default.LayoutFileName);
            }
            catch (Exception) {
                MessageBox.Show("Could not save layout", "Layout Error", MessageBoxButton.OK);
            }

            // Is the current file saved?
            asmCodeWindow.UnsavedHandler();

            // Save properties
            Properties.Settings.Default.LastFile = asmCodeWindow.CurrentFileName;
            Properties.Settings.Default.Speed = processorVM.Speed;
            Properties.Settings.Default.ZoomArchitecture = architectureWindow.ScaleSlider.Value;
            Properties.Settings.Default.ZoomOutput = outputWindow.ScaleSlider.Value;
            Properties.Settings.Default.ZoomRegisters = registersWindow.ScaleSlider.Value;
            Properties.Settings.Default.ZoomRAM = ramWindow.ScaleSlider.Value;
            Properties.Settings.Default.ZoomCode = asmCodeWindow.ScaleSlider.Value;
            Properties.Settings.Default.RecentFileList = string.Join(";", (from s in MostRecentFiles select s.Path).Distinct().Take(8).ToArray());
            Properties.Settings.Default.Save();
            logger.Info("Properties saved and application shut down");
        }


        /// <summary>
        /// Callback handler to be invoked when the window is loaded.
        /// </summary>
        /// <param name="sender">The sender which notified the event</param>
        /// <param name="e">The routed event which notified</param>
        private void Window_Loaded(object sender, RoutedEventArgs e) {
            Pipeserver.pipeName = "stebspipe";
            Pipeserver.owner = this;
            Pipeserver.ownerInvoker = new Invoker(this);
            ThreadStart pipeThread = new ThreadStart(Pipeserver.createPipeServer);
            Thread listenerThread = new Thread(pipeThread);
            listenerThread.SetApartmentState(ApartmentState.STA);
            listenerThread.IsBackground = true;
            listenerThread.Start();
        }


        /// <summary>
        /// Is called at the startup of the application to restore the last layout.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dockManager_Loaded(object sender, RoutedEventArgs e) {
            // Restore the layout
            if (File.Exists(Properties.Settings.Default.LayoutFileName)) {
                try {
                    dockManager.RestoreLayout(Properties.Settings.Default.LayoutFileName);
                }
                catch (Exception) {
                    MessageBox.Show("LayoutFileName", "Layout Error", MessageBoxButton.OK);
                }
            }
        }


        /// <summary>
        /// Is called if clicking the AddDevice button. Displays the FileOpen Dialog
        /// to add a new IO device.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddDevice_Click(object sender, RoutedEventArgs e) {
            OpenFileDialog fileDlg = new OpenFileDialog();

            if (fileDlg.ShowDialog() == true) {
                // Load the window
                IOWrapperWindow ioW = IOWindowLoader.LoadIOWindow(fileDlg.FileName);
                // Add the window
                AddIOWindow(ioW);
            }
        }


        /// <summary>
        /// Adds a new IO device window to the application.
        /// </summary>
        /// <param name="ioW">The new IO device window</param>
        private void AddIOWindow(IOWrapperWindow ioW) {
            // Check if there are already IO devices with the same name
            var sameNames = from w in processorVM.ioWindows
                            where w.Value.GetName() == ioW.GetName()
                            select w;
            string copy_string = "";

            if (sameNames.Count() > 0) {
                copy_string = "_" + sameNames.Count();
            }
            ioW.Name += copy_string;

            // Set the port number of the new IO device
            int IOAdr = 0x00;
            if (ioW.Name == "IOLight") {
                IOAdr = 0x00;
            }
            else if (ioW.Name == "IOHeater") {
                IOAdr = 0x01;
            }
            else if (ioW.Name == "IO7Segment") {
                IOAdr = 0x02;
            }
            else if (ioW.Name == "IOInterrupt") {
                IOAdr = 0xFF;
            }
            else {
                var customIOKeys = from k in processorVM.ioWindows.Keys
                                   where k >= 0x0A && k != 0xFF
                                   select k;
                if (customIOKeys.Count() > 0) {
                    IOAdr = customIOKeys.Max() + 1;
                }
                else {
                    IOAdr = 0x10;
                }
            }

            // Add the window to the list
            processorVM.ioWindows.Add(IOAdr, ioW);
            // Cut off "IO". Suppress Port info if it's an interrupt (0xFF)
            ioW.Title =
                ioW.GetName().Substring(2) + copy_string +
                (IOAdr != 0xFF ? " [Port " + IOAdr.ToHexString(2) + "]": " Source");

            // Register the fire interrupt event
            ioW.FireInterruptEvent += Interrupt;

            // Display the window
            ((DockableContent)ioW).Show(dockManager);

            // Create a ribbon button and its event handler
            RibbonButton rb = new RibbonButton();
            rb.Click += (s, e) => ((DockableContent)ioW).Show(dockManager);
            rb.Label = ioW.GetName().Substring(2) + copy_string;
            rb.SmallImageSource = (ioW.RibbonIcon != null) ?
                ioW.RibbonIcon : new BitmapImage(new Uri("res\\default.png", UriKind.Relative));
            GroupDevice.Items.Add(rb);
            rb.ToolTipTitle = ioW.GetName().Substring(2) + " Window";
            rb.ToolTipDescription = "Show the " + ioW.GetName().Substring(2) + " window";
            logger.Trace("Io window {0} added", ioW.Name);
        }


        /// <summary>
        /// Is called from the IO device through an event. Sets the IRF (interrupt flag)
        /// on the processor view model to true.
        /// </summary>
        private void Interrupt() {
            InteruptExecutedDelegate handler = new InteruptExecutedDelegate(SetInterruptFlag);
            handler.BeginInvoke(null, null);
        }


        /// <summary>
        /// Delegate to set the interrupt flag ansynchonously.
        /// </summary>
        private delegate void InteruptExecutedDelegate();


        /// <summary>
        /// Sets the processor interrupt flag.
        /// </summary>
        private void SetInterruptFlag() {
            if (processorVM.IRF != 1) {
                processorVM.IRF = 1;
            }
        }


        // ==============================================================================
        // Command handler functions
        // ==============================================================================
        #region
        /// <summary>
        /// Displays the RAM window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RAM_Click(object sender, RoutedEventArgs e) {
            ramWindow.Show();
        }

        /// <summary>
        /// Displays the architecture window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Architecture_Click(object sender, RoutedEventArgs e) {
            architectureWindow.Show();
        }

        /// <summary>
        /// Displays the assembler code window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Code_Click(object sender, RoutedEventArgs e) {
            asmCodeWindow.Show();
        }

        /// <summary>
        /// Displays the output window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Output_Click(object sender, RoutedEventArgs e) {
            outputWindow.Show();
        }

        /// <summary>
        /// Displays the registers window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Registers_Click(object sender, RoutedEventArgs e) {
            registersWindow.Show();
        }

        /// <summary>
        /// Resets the assembler code window.
        /// </summary>
        private void ResetAsmCodeWindow() {
            asmCodeWindow.ShowHighlightedLine = false;
            asmCodeWindow.RefreshCurrentLine();
            processorVM.ReadyToRun = false;
            CheckControls();
            outputWindow.Clear();
        }

        /// <summary>
        /// Opens a new file in the assembler code window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NewExecuted(object sender, ExecutedRoutedEventArgs e) {
            if (asmCodeWindow.NewCommandHandler()) {
                ResetAsmCodeWindow();
            }
        }

        /// <summary>
        /// Opens a recent file in the assembler code window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenExecuted(object sender, ExecutedRoutedEventArgs e) {
            logger.Trace("Methode invoked: OpenExecuted");
            RecentFile f = asmCodeWindow.OpenCommandHandler();
            if (f != null) {
                ResetAsmCodeWindow();
                MostRecentFiles.Insert(0, f);
            }
            logger.Info("Open file in assembler code window");
        }

        /// <summary>
        /// Saves the file in the assembler code window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveExecuted(object sender, ExecutedRoutedEventArgs e) {
            asmCodeWindow.SaveCommandHandler();
        }

        /// <summary>
        /// Saves the file in the assembler code window under a new name.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveAsExecuted(object sender, ExecutedRoutedEventArgs e) {
            RecentFile f = asmCodeWindow.SaveAsCommandHandler();
            if (f != null) {
                MostRecentFiles.Insert(0, f);
            }
        }

        /// <summary>
        /// Assembles the code in the assembler code window and sets the machine code in the RAM.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AssembleExecuted(object sender, ExecutedRoutedEventArgs e) {
            this.Cursor = Cursors.Wait;
            AsyncCallback callback = new AsyncCallback(HandleAssembleExecutedCallback);
            AssembleExecutedDelegate handle = new AssembleExecutedDelegate(HandleAssembleExecuted);
            handle.BeginInvoke(callback, null);
        }

        private void HandleAssembleExecutedCallback(IAsyncResult ar) {
            this.InvokeIfRequired(() => {
                CheckControls();
                this.Cursor = Cursors.Arrow;
            });
        }

        private delegate void AssembleExecutedDelegate();

        private void HandleAssembleExecuted() {
            processorVM.Stop();
            string assemblerCode = string.Empty;
            this.InvokeIfRequired(() => {
                assemblerCode = asmCodeWindow.getText();
                AssemblerParser asm = new AssemblerParser(processorVM, outputWindow);

                // Assemble the code
                if (asm.Assemble(assemblerCode)) {
                    outputWindow.Clear();
                    // Write code list
                    outputWindow.WriteOutput(Common.getCodeList().toString());

                    // If successful copy the machine code into the RAM
                    processorVM.RAM = asm.MachineCode;
                    asmCodeWindow.CodeLinesPos = asm.RamPosToCodeLine;

                    asmCodeWindow.ShowHighlightedLine = true;
                    asmCodeWindow.RefreshCurrentLine();

                    processorVM.ReadyToRun = true;
                }
                else {
                    processorVM.ReadyToRun = false;
                    asmCodeWindow.ShowHighlightedLine = false;
                    asmCodeWindow.RefreshCurrentLine();
                }
                stepType = ProcessorViewModel.StepType.NONE;
            });
        }

        /// <summary>
        /// Executes an assembler code assembly.
        /// </summary>
        public void assemble() {
            this.AssembleExecuted(this, null);
        }

        /// <summary>
        /// Execute a microstep in the processor ViewModel.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MicroStepExecuted(object sender, ExecutedRoutedEventArgs e) {
            processorVM.MicroStep();
            CheckControls();
        }

        /// <summary>
        /// Executes a macrostep in the processor ViewModel.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MacroStepExecuted(object sender, ExecutedRoutedEventArgs e) {
            processorVM.MacroStep();
            CheckControls();
        }

        /// <summary>
        /// Executes an instruction step in the processor ViewModel.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InstructionStepExecuted(object sender, ExecutedRoutedEventArgs e) {
            processorVM.InstructionStep();
            CheckControls();
        }

        /// <summary>
        /// Saves the last step type used in the auto-run mode.
        /// </summary>
        private ProcessorViewModel.StepType stepType = ProcessorViewModel.StepType.NONE;


        /// <summary>
        /// Checks if the commands for starting the auto-run mode are enabled.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Run_StepCanExecute(object sender, CanExecuteRoutedEventArgs e) {
            logger.Trace("ENTER Run_StepCanExecute");
            e.CanExecute = !processorVM.IsRunning && processorVM.ReadyToRun && !processorVM.HasHalted;
            logger.Trace("LEAVE Run_StepCanExecute");
        }

        /// <summary>
        /// Starts the auto-run mode with instruction steps.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RunInstructionExecuted(object sender, ExecutedRoutedEventArgs e) {
            stepType = ProcessorViewModel.StepType.INSTRUCTION;
            processorVM.Run(stepType);
            CheckControls();
        }

        /// <summary>
        /// Starts the auto-run mode with macrosteps.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RunMacroExecuted(object sender, ExecutedRoutedEventArgs e) {
            stepType = ProcessorViewModel.StepType.MACRO;
            processorVM.Run(stepType);
            CheckControls();
        }

        /// <summary>
        /// Starts the auto-run mode with microsteps.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RunMicroExecuted(object sender, ExecutedRoutedEventArgs e) {
            stepType = ProcessorViewModel.StepType.MICRO;
            processorVM.Run(stepType);
            CheckControls();
        }

        /// <summary>
        /// Checks if the continue/pause command is enabled.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Continue_PauseCanExecute(object sender, CanExecuteRoutedEventArgs e) {
            e.CanExecute = processorVM.ReadyToRun && (stepType != ProcessorViewModel.StepType.NONE) && !processorVM.HasHalted;
        }

        /// <summary>
        /// Continues or pauses the auto-run mode.
        /// </summary>
        /// <param name="sender">the control which started execution</param>
        /// <param name="e">the event arguments of the control</param>
        private void Continue_PauseExecuted(object sender, ExecutedRoutedEventArgs e) {
            this.Cursor = Cursors.Wait;
            AsyncCallback callback = new AsyncCallback(Continue_PauseExecutedCallback);
            Continue_PauseExecutedDelegate handler = new Continue_PauseExecutedDelegate(Continue_PauseExecutedHandler);
            handler.BeginInvoke(callback, null);
        }

        private delegate void Continue_PauseExecutedDelegate();

        private void Continue_PauseExecutedHandler() {
            if (processorVM.IsRunning) {
                processorVM.Stop();
            }
            else {
                processorVM.Run(stepType);
            }
        }

        /// <summary>
        /// Callback which is called when the pause has been successfully executed.
        /// </summary>
        /// <param name="ar"></param>
        private void Continue_PauseExecutedCallback(IAsyncResult ar) {
            this.InvokeIfRequired(() => {
                CheckControls();
                this.Cursor = Cursors.Arrow;
            });
        }

        /// <summary>
        /// Checks if the restart command is enabled.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RestartCanExecute(object sender, CanExecuteRoutedEventArgs e) {
            e.CanExecute = processorVM.ReadyToRun && (stepType != ProcessorViewModel.StepType.NONE);
        }

        /// <summary>
        /// Restarts the assembler programm in the auto-run mode with the last step type.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RestartExecuted(object sender, ExecutedRoutedEventArgs e) {
            this.Cursor = Cursors.Wait;
            AsyncCallback callback = new AsyncCallback(RestartExecutedCallback);
            RestartExecutedDelegate handler = new RestartExecutedDelegate(RestartExecutedHandler);
            handler.BeginInvoke(callback, null);
        }

        private delegate void RestartExecutedDelegate();

        private void RestartExecutedHandler() {
            ProcessorViewModel.StepType tmp = stepType;
            HandleAssembleExecuted();
            stepType = tmp;
            Continue_PauseExecutedHandler();
        }

        private void RestartExecutedCallback(IAsyncResult result) {
            this.InvokeIfRequired(() => {
                CheckControls();
                this.Cursor = Cursors.Arrow;
            });
        }

        /// <summary>
        /// Resets the processor.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ResetExecuted(object sender, ExecutedRoutedEventArgs e) {
            this.Cursor = Cursors.Wait;
            AsyncCallback resetCallback = new AsyncCallback(ResetProcessCallback);
            ResetProcessorDelegate handler = new ResetProcessorDelegate(processorVM.Reset);
            handler.BeginInvoke(ResetProcessCallback, null);
            outputWindow.Clear();
        }

        public void ResetExecuted() {
            this.ResetExecuted(this, null);
        }

        /// <summary>
        /// Delegate used to execute a processor reset.
        /// </summary>
        private delegate void ResetProcessorDelegate();

        /// <summary>
        /// Callback called when the processor reset has finished.
        /// </summary>
        /// <param name="result"></param>
        private void ResetProcessCallback(IAsyncResult result) {
            this.InvokeIfRequired(() => {
                asmCodeWindow.ShowHighlightedLine = false;
                asmCodeWindow.RefreshCurrentLine();

                stepType = ProcessorViewModel.StepType.NONE;
                CheckControls();
                this.Cursor = Cursors.Arrow;
            });
        }


        // ===================================================================================
        // Load and save layout functions
        // ===================================================================================
        #region
        /// <summary>
        /// Loads the micro layout.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MicroLayout_Click(object sender, RoutedEventArgs e) {
            if (File.Exists(Properties.Settings.Default.MicroLayoutFile)) {
                try {
                    dockManager.RestoreLayout(Properties.Settings.Default.MicroLayoutFile);
                }
                catch (Exception) {
                    MessageBox.Show("MicroLayoutFile", "Layout Error", MessageBoxButton.OK);
                }
            }
        }

        /// <summary>
        /// Loads the registers layout.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RegistersLayout_Click(object sender, RoutedEventArgs e) {
            if (File.Exists(Properties.Settings.Default.RegistersLayoutFile)) {
                try {
                    dockManager.RestoreLayout(Properties.Settings.Default.RegistersLayoutFile);
                }
                catch (Exception) {
                    MessageBox.Show("RegistersLayoutFile", "Layout Error", MessageBoxButton.OK);
                }
            }
        }

        /// <summary>
        /// Loads the floating layout.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FloatingLayout_Click(object sender, RoutedEventArgs e) {
            if (File.Exists(Properties.Settings.Default.FloatingLayoutFile)) {
                try {
                    dockManager.RestoreLayout(Properties.Settings.Default.FloatingLayoutFile);
                }
                catch (Exception) {
                    MessageBox.Show("FloatingLayoutFile", "Layout Error", MessageBoxButton.OK);
                }
            }
        }

        /// <summary>
        /// Loads the last layout.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LastLayout_Click(object sender, RoutedEventArgs e) {
            if (File.Exists(Properties.Settings.Default.LayoutFileName)) {
                try {
                    dockManager.RestoreLayout(Properties.Settings.Default.LayoutFileName);
                }
                catch (Exception) {
                    MessageBox.Show("LayoutFileName", "Layout Error", MessageBoxButton.OK);
                }
            }
        }

        /// <summary>
        /// Loads the interrupt layout
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InterruptLayout_Click(object sender, RoutedEventArgs e) {
            if (File.Exists(Properties.Settings.Default.InterruptLayoutFile)) {
                try {
                    dockManager.RestoreLayout(Properties.Settings.Default.InterruptLayoutFile);
                }
                catch (Exception) {
                    MessageBox.Show("InterruptLayoutFile", "Layout Error", MessageBoxButton.OK);
                }
            }
        }

        /// <summary>
        /// Loads the all layout.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AllLayout_Click(object sender, RoutedEventArgs e) {
            if (File.Exists(Properties.Settings.Default.AllLayoutFile)) {
                try {
                    dockManager.RestoreLayout(Properties.Settings.Default.AllLayoutFile);
                }
                catch (Exception) {
                    MessageBox.Show("AllLayoutFile", "Layout Error", MessageBoxButton.OK);
                }
            }
        }

        /// <summary>
        /// Loads the custom 1 layout.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Custom1_Load_Click(object sender, RoutedEventArgs e) {
            if (File.Exists(Properties.Settings.Default.Custom1LayoutFile)) {
                try {
                    dockManager.RestoreLayout(Properties.Settings.Default.Custom1LayoutFile);
                }
                catch (Exception) {
                    MessageBox.Show("Custom1LayoutFile", "Layout Error", MessageBoxButton.OK);
                }
            }
        }

        /// <summary>
        /// Saves the custom 1 layout.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Custom1_Save_Click(object sender, RoutedEventArgs e) {
            dockManager.SaveLayout(Properties.Settings.Default.Custom1LayoutFile);
        }

        /// <summary>
        /// Loads the custom 2 layout.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Custom2_Load_Click(object sender, RoutedEventArgs e) {
            if (File.Exists(Properties.Settings.Default.Custom2LayoutFile)) {
                try {
                    dockManager.RestoreLayout(Properties.Settings.Default.Custom2LayoutFile);
                }
                catch (Exception) {
                    MessageBox.Show("Custom2LayoutFile", "Layout Error", MessageBoxButton.OK);
                }
            }
        }

        /// <summary>
        /// Saves the custom 2 layout.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Custom2_Save_Click(object sender, RoutedEventArgs e) {
            dockManager.SaveLayout(Properties.Settings.Default.Custom2LayoutFile);
        }

        /// <summary>
        /// Loads the custom 3 layout.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Custom3_Load_Click(object sender, RoutedEventArgs e) {
            if (File.Exists(Properties.Settings.Default.Custom3LayoutFile)) {
                try {
                    dockManager.RestoreLayout(Properties.Settings.Default.Custom3LayoutFile);
                }
                catch (Exception) {
                    MessageBox.Show("Custom3LayoutFile", "Layout Error", MessageBoxButton.OK);
                }
            }
        }

        /// <summary>
        /// Saves the custom 3 layout.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Custom3_Save_Click(object sender, RoutedEventArgs e) {
            dockManager.SaveLayout(Properties.Settings.Default.Custom3LayoutFile);
        }
        #endregion


        /// <summary>
        /// Opens the about dialog.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void About_Click(object sender, RoutedEventArgs e) {
            MessageBox.Show("stebs - student training eight bit simulator\n" +
                            "V4.1\n" +
                            "\n" +
                            "Copyright © 2011 - 2015\n" +
                            "FHNW, HT Windisch, Switzerland\n" +
                            "Bachelor Thesis, 2011:\n" +
                            "Ivo Nussbaumer & Thomas Schilling\n" +
                            "\n" +
                            "Students' Project, 2013: \n" +
                            "Nicolas Weber, Roman Holzner,\n" +
                            "Josiane Manera, Jurij Chamin\n" +
                            "\n" +
                            "Supervisor: Ruedi Müller",
                            "About stebs - student training eight bit simulator",
                            MessageBoxButton.OK, MessageBoxImage.Information);
        }

        /// <summary>
        /// Opens the user guide.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HelpExecuted(object sender, ExecutedRoutedEventArgs e) {
            System.Diagnostics.Process.Start(Properties.Settings.Default.HelpFile);
        }

        /// <summary>
        /// Shows a hint.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HintExecuted(object sender, ExecutedRoutedEventArgs e) {
            hints.Show();
        }

        /// <summary>
        /// Opens the selected file in the recent file list box in the ribbon menu.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e) {
            RecentFile rf = (RecentFile)RecentListBox.SelectedItem;

            if (rf != null) {
                asmCodeWindow.OpenFile(rf.Path);
                ResetAsmCodeWindow();
            }

            Ribbon.ApplicationMenu.IsDropDownOpen = false;
        }
        #endregion


        /// <summary>
        /// Sets the enabled states of some controls in the ribbon menu.
        /// </summary>
        private void CheckControls() {
            Run.IsEnabled = !processorVM.IsRunning && processorVM.ReadyToRun && !processorVM.HasHalted;
            SpeedSlider.IsEnabled = processorVM.ReadyToRun && !processorVM.HasHalted;
            SpeedLabel.IsEnabled = processorVM.ReadyToRun && !processorVM.HasHalted;

            Continue.Visibility = (!processorVM.IsRunning && processorVM.ReadyToRun && (stepType != ProcessorViewModel.StepType.NONE) && !processorVM.HasHalted) ? Visibility.Visible : Visibility.Collapsed;
            Pause.Visibility = (Continue.Visibility == System.Windows.Visibility.Collapsed) ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// Accessor for the assembler code window.
        /// </summary>
        /// <returns></returns>
        public AsmCodeWindow GetAsmCodeWindow() {
            return this.asmCodeWindow;
        }


        /// <summary>
        /// Invoke handler on closing event, i.e. on exiting the application for Alt-F4,
        /// Exit buttons clicked or icon double-clicked.
        /// </summary>
        /// <returns></returns>
        private void RibbonWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            switch (asmCodeWindow.UnsavedExitHandler()) {
                // 0: nothing to save, exit
                case AsmCodeWindow.ExitCode.NOTHING_TO_SAVE:
                    break;
                // 1: saved, exit
                case AsmCodeWindow.ExitCode.SAVED:
                    break;
                // 2: not saved, exit
                case AsmCodeWindow.ExitCode.DONT_SAVE:
                    break;
                // 3: cancel, don't exit
                case AsmCodeWindow.ExitCode.CANCELLED:
                    e.Cancel = true;
                    break;
            }
        }


        private void CloseApplicationExecuted(object sender, ExecutedRoutedEventArgs e) {
            Close();
        }
    }
}