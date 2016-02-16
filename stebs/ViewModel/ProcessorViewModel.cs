namespace Stebs.ViewModel
{
    using System.Windows.Input;
    using System.Text;
    using Stebs.Model;
    using Stebs.View;
    using Stebs.View.CustomControls;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Windows;
    using System.Windows.Media;
    using NLog;
    using Stebs.Commands;

    /// <summary>
    /// Connection between the Model and the View
    /// 
    /// Has to logic to execute the steps on the processor
    /// </summary>
    public class ProcessorViewModel : ViewModelBase
    {
        /// <summary>
        /// The logger instance of this clas
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The time how long the hwinterrupt access may be blocked before
        /// an application exception happens. A value >0 is the time in seconds.
        /// Taking 0 means, that there's only a check for the lock
        /// and -1 is equal to infinit wait.
        /// </summary>
        private const int HW_INTERRUPT_LOCK_TIMEOUT = -1;

        /// <summary>
        /// Lock to protect or lock the IRF flag, changed by hardware interrupts.
        /// The Interrupt Flag (IRF) can be changed by a hardware interrupt
        /// Since the value also may be changed by the processor
        /// or the value has to be locked for atomic reading
        /// this lock can be used.
        /// </summary>
        private ReaderWriterLock hwInteruptLock;

        // ========================================================
        // Brushes
        // ========================================================
        #region
        private static Brush yellowBrush = new SolidColorBrush(Colors.Yellow);
        private static Brush transparentBrush = new SolidColorBrush(Colors.Transparent);
        private static Brush blackBrush = new SolidColorBrush(Colors.Black);
        private static Brush redBrush = new SolidColorBrush(Colors.Red);
        private static Brush blueBrush = new SolidColorBrush(Colors.Blue);
        #endregion

        // ========================================================
        // Events
        // ========================================================
        #region
        /// Event which is fired when a ALU Command is executed, so the View can display the right alu-command
        public event ALUChangedHandler aluChangedEvent;
        public delegate void ALUChangedHandler(Alu.Cmd op);

        // Event which is fired when the MIP is changed, so the View can display the corresponding Microcode-Entry
        public event MIPChangedHandler mipChangedEvent;
        public delegate void MIPChangedHandler();

        // Event which is fired when the IR changed, so the View can display the corresonding Assembler-Command
        public event IRChangedHandler irChangedEvent;
        public delegate void IRChangedHandler();

        // Event which is fired when the RAM has changed, so the View can update the RAM
        public event RAMChangedHandler ramChangedEvent;
        public delegate void RAMChangedHandler();

        // Event which is fired when the IP oder SP has changed, so the RAM-View can update the highlighting of these two values
        public event IPSPChangedHandler ipspChangedEvent;
        public delegate void IPSPChangedHandler();
        #endregion

        // ========================================================
        // Properties
        // ========================================================
        #region
        /// <summary>
        /// Represents an ALU-Command
        /// </summary>
        public struct AluDataGridEntry
        {
            public int Alu
            {
                get
                {
                    return (int)Op;
                }
            }
            public Alu.Cmd Op
            {
                get;
                set;
            }
        }

        /// <summary>
        /// Returns a List of all the ALU Commands
        /// </summary>
        public List<AluDataGridEntry> AluEntries
        {
            get;
            private set;
        }

        private DecoderEntryViewModel currentDecoderEntry;
        
        /// <summary>
        /// The list of all decoder entries
        /// </summary>
        public IEnumerable<DecoderEntryViewModel> DecoderEntries
        {
            get;
            private set;
        }

        /// <summary>
        /// The Current Decoder Entry, displays a MessageBox if it is an illegal Opcode
        /// </summary>
        public DecoderEntryViewModel CurrentDecoderEntry {
            get { return currentDecoderEntry; }
            private set {
                currentDecoderEntry = value;
                OnPropertyChanged("CurrentDecoderEntry");
            }
        }

        /// <summary>
        /// The List of all MPM Entries
        /// </summary>
        public IEnumerable<MPMEntryViewModel> MPMEntries
        {
            get;
            private set;
        }

        private MPMEntryViewModel currentMPMEntry;

        /// <summary>
        /// The current MPM Entry, displays a MessageBox if it is an illegal MIP
        /// </summary>
        public MPMEntryViewModel CurrentMPMEntry
        {
            get { return currentMPMEntry; }
            private set
            {
                currentMPMEntry = value;
                OnPropertyChanged("CurrentMPMEntry");
            }

        }

        /// <summary>
        /// Property for the SEL Register on the Processor
        /// </summary>
        public byte SEL
        {
            get
            {
                return proc.SEL;
            }
            
            set
            {
                proc.SEL = value;
                OnPropertyChanged("SEL");

                BG_SEL = yellowBrush;
            }
        }

        /// <summary>
        /// Property for the AL Register on the Processor
        /// </summary>
        public byte AL
        {
            get
            {
                return proc.AL;
            }
            set
            {
                proc.AL = value;
                OnPropertyChanged("AL");
                BG_AL = yellowBrush;
            }
        }

        /// <summary>
        /// Gets or sets the BL Register on the Processor
        /// </summary>
        public byte BL
        {
            get
            {
                return proc.BL;
            }
            set
            {
                proc.BL = value;
                OnPropertyChanged("BL");

                BG_BL = yellowBrush;
            }
        }

        /// <summary>
        /// Gets or sets the CL Register on the Processor
        /// </summary>
        public byte CL
        {
            get
            {
                return proc.CL;
            }
            set
            {
                proc.CL = value;
                OnPropertyChanged("CL");

                BG_CL = yellowBrush;
            }
        }

        /// <summary>
        /// Property for the DL Register on the Processor
        /// </summary>
        public byte DL
        {
            get
            {
                return proc.DL;
            }
            set
            {
                proc.DL = value;
                OnPropertyChanged("DL");

                BG_DL = yellowBrush;
            }
        }

        /// <summary>
        /// Property for the SP Register on the Processor
        /// </summary>
        public byte SP {
            get {
                return proc.SP;
            }
            set {
                proc.SP = value;
                OnPropertyChanged("SP");
                if (ipspChangedEvent != null) {
                    ipspChangedEvent();
                }

                BG_SP = yellowBrush;
            }
        }

        /// <summary>
        /// Property for the IP Register on the Processor
        /// </summary>
        public byte IP {
            get {
                return proc.IP;
            }
            set {
                proc.IP = value;
                OnPropertyChanged("IP");
                if (ipspChangedEvent != null) {
                    ipspChangedEvent();
                }

                BG_IP = yellowBrush;
            }
        }

        /// <summary>
        /// Property for the MBR Register on the Processor
        /// </summary>
        public byte MBR {
            get {
                return proc.MBR;
            }
            set {
                proc.MBR = value;
                OnPropertyChanged("MBR");

                BG_MBR = yellowBrush;
            }
        }
        
        /// <summary>
        /// Property for the MAR Register on the Processor
        /// </summary>
        public byte MAR {
            get {
                return proc.MAR;
            }
            set {
                proc.MAR = value;
                OnPropertyChanged("MAR");

                BG_MAR = yellowBrush;
            }
        }

        /// <summary>
        /// Property for the MDR Register on the Processor
        /// </summary>
        public byte MDR {
            get {
                return proc.MDR;
            }
            set {
                proc.MDR = value;
                OnPropertyChanged("MDR");

                BG_MDR = yellowBrush;
            }
        }

        public byte ALU_X
        {
            get
            {
                return proc.alu.X;
            }
            set
            {
                proc.alu.X = value;
                OnPropertyChanged("ALU_X");
                BG_ALU_X = yellowBrush;
            }
        }

        public byte ALU_Y
        {
            get
            {
                return proc.alu.Y;
            }
            set
            {
                proc.alu.Y = value;
                OnPropertyChanged("ALU_Y");
                BG_ALU_Y = yellowBrush;
            }
        }

        public Boolean ALC
        {
            get
            {
                return CurrentMPMEntry.Model.Alu != Alu.Cmd.NOP;
            }
        }

        /// <summary>
        /// Property for the RES Register on the Processor
        /// </summary>
        public byte RES {
            get {
                return proc.alu.Res;
            }
            set {
                proc.alu.Res = value;
                OnPropertyChanged("RES");

                BG_RES = yellowBrush;
            }
        }

        /// <summary>
        /// Property for the MIP Register on the Processor
        /// </summary>
        public int MIP {
            get {
                return proc.MIP;
            }
            set {

                var queryMPMEntry = from entry in MPMEntries
                            where entry.Model.Addr == proc.MIP
                            select entry;


                if (queryMPMEntry.Count() < 1)
                {
                    ShowIllegalMipDialog();
                    Stop();
                    ResetExecutionStates();
                    return;
                }
                CurrentMPMEntry = queryMPMEntry.First();

                proc.MIP = value;
                OnPropertyChanged("MIP");
                OnPropertyChanged("ALC");

                BG_MIP = yellowBrush;
                if (mipChangedEvent != null) {
                    mipChangedEvent.BeginInvoke(null, null);
                }
            }
        }

        /// <summary>
        /// Property for the IR Register on the Processor
        /// </summary>
        public byte IR {
            get {
                return proc.IR;
            }
            set {
                proc.IR = value;
                OnPropertyChanged("IR");
                if (irChangedEvent != null) {
                    irChangedEvent.BeginInvoke(null,null);
                }

                BG_IR = yellowBrush;
            }
        }

        /// <summary>
        /// Property for the S Flag on the Processor
        /// </summary>
        public int S {
            get {
                return proc.S ? 1 : 0;
            }
        }
        /// <summary>
        /// Property for the O Flag on the Processor
        /// </summary>
        public int O {
            get {
                return proc.O ? 1 : 0;
            }
        }

        /// <summary>
        /// Property for the Z Flag on the Processor
        /// </summary>
        public int Z {
            get {
                return proc.Z ? 1 : 0;
            }
        }

        /// <summary>
        /// Property for the I Flag on the Processor
        /// </summary>
        public int I {
            get {
                return proc.I ? 1 : 0;
            }
        }

        /// <summary>
        /// Property for the IRF Flag on the Processor
        /// </summary>
        public int IRF {
            get {
                hwInteruptLock.AcquireReaderLock(HW_INTERRUPT_LOCK_TIMEOUT);
                try
                {
                    return proc.IRF ? 1 : 0;
                }
                finally
                {
                    hwInteruptLock.ReleaseReaderLock();
                }
            }
            set {
                hwInteruptLock.AcquireWriterLock(HW_INTERRUPT_LOCK_TIMEOUT);
                try
                {
                    proc.IRF = value == 0 ? false : true;
                    OnPropertyChanged("IRF");

                    BG_IRF = yellowBrush;
                }
                finally
                {
                    hwInteruptLock.ReleaseWriterLock();
                }
            }
        }
        /// <summary>
        /// Property for the SR Register on the Processor
        /// </summary>
        public byte SR {
            get {
                return proc.SR;
            }
            set {
                proc.SR = value;
                OnPropertyChanged("S");
                OnPropertyChanged("O");
                OnPropertyChanged("Z");
                OnPropertyChanged("I");
                OnPropertyChanged("SR");

                BG_S = yellowBrush;
                BG_O = yellowBrush;
                BG_Z = yellowBrush;
                BG_I = yellowBrush;
            }
        }
        /// <summary>
        /// Property for the RAM-Data in the DATA-Bus on the Processor
        /// </summary>
        public byte[] RAM {
            get {
                return proc.ExternalData.RamData;
            }
            set {
                proc.ExternalData.RamData = value;
                if (ramChangedEvent != null) {
                    ramChangedEvent();
                }
            }
        }
        #endregion

        // ========================================================
        // Background Properties
        // ========================================================
        #region
        private Brush Brush_BG_SEL = transparentBrush;
        public Brush BG_SEL {
            get {
                return Brush_BG_SEL;
            }
            set {
                Brush_BG_SEL = value;
                Brush_BG_SEL.Freeze();
                OnPropertyChanged("BG_SEL");
            }
        }
        private Brush Brush_BG_AL = transparentBrush;
        public Brush BG_AL {
            get {
                return Brush_BG_AL;
            }
            set {
                Brush_BG_AL = value;
                Brush_BG_AL.Freeze();
                OnPropertyChanged("BG_AL");
            }
        }
        private Brush Brush_BG_BL = transparentBrush;
        public Brush BG_BL {
            get {
                return Brush_BG_BL;
            }
            set {
                Brush_BG_BL = value;
                Brush_BG_BL.Freeze();
                OnPropertyChanged("BG_BL");
            }
        }
        private Brush Brush_BG_CL = transparentBrush;
        public Brush BG_CL {
            get {
                return Brush_BG_CL;
            }
            set {
                Brush_BG_CL = value;
                Brush_BG_CL.Freeze();
                OnPropertyChanged("BG_CL");
            }
        }

        private Brush Brush_BG_DL = transparentBrush;
        public Brush BG_DL {
            get {
                return Brush_BG_DL;
            }
            set {
                Brush_BG_DL = value;
                Brush_BG_DL.Freeze();
                OnPropertyChanged("BG_DL");
            }
        }
        private Brush Brush_BG_SP = transparentBrush;
        public Brush BG_SP {
            get {
                return Brush_BG_SP;
            }
            set {
                Brush_BG_SP = value;
                Brush_BG_SP.Freeze();
                OnPropertyChanged("BG_SP");
            }
        }
        private Brush Brush_BG_IP = transparentBrush;
        public Brush BG_IP {
            get {
                return Brush_BG_IP;
            }
            set {
                Brush_BG_IP = value;
                Brush_BG_IP.Freeze();
                OnPropertyChanged("BG_IP");
            }
        }
        private Brush Brush_BG_MIP = transparentBrush;
        public Brush BG_MIP {
            get {
                return Brush_BG_MIP;
            }
            set {
                Brush_BG_MIP = value;
                Brush_BG_MIP.Freeze();
                OnPropertyChanged("BG_MIP");
            }
        }
        private Brush Brush_BG_MBR = transparentBrush;
        public Brush BG_MBR {
            get {
                return Brush_BG_MBR;
            }
            set {
                Brush_BG_MBR = value;
                Brush_BG_MBR.Freeze();
                OnPropertyChanged("BG_MBR");
            }
        }
        private Brush Brush_BG_MAR = transparentBrush;
        public Brush BG_MAR {
            get {
                return Brush_BG_MAR;
            }
            set {
                Brush_BG_MAR = value;
                Brush_BG_MAR.Freeze();
                OnPropertyChanged("BG_MAR");
            }
        }
        private Brush Brush_BG_MDR = transparentBrush;
        public Brush BG_MDR {
            get {
                return Brush_BG_MDR;
            }
            set {
                Brush_BG_MDR = value;
                Brush_BG_MDR.Freeze();
                OnPropertyChanged("BG_MDR");
            }
        }
        private Brush Brush_BG_RES = transparentBrush;
        public Brush BG_RES {
            get {
                return Brush_BG_RES;
            }
            set {
                Brush_BG_RES = value;
                Brush_BG_RES.Freeze();
                OnPropertyChanged("BG_RES");
            }
        }
        private Brush Brush_BG_IR = transparentBrush;
        public Brush BG_IR {
            get {
                return Brush_BG_IR;
            }
            set {
                Brush_BG_IR = value;
                Brush_BG_IR.Freeze();
                OnPropertyChanged("BG_IR");
            }
        }
        private Brush Brush_BG_S = transparentBrush;
        public Brush BG_S {
            get {
                return Brush_BG_S;
            }
            set {
                Brush_BG_S = value;
                Brush_BG_S.Freeze();
                OnPropertyChanged("BG_S");
            }
        }
        private Brush Brush_BG_O = transparentBrush;
        public Brush BG_O {
            get {
                return Brush_BG_O;
            }
            set {
                Brush_BG_O = value;
                Brush_BG_O.Freeze();
                OnPropertyChanged("BG_O");
            }
        }
        private Brush Brush_BG_Z = transparentBrush;
        public Brush BG_Z {
            get {
                return Brush_BG_Z;
            }
            set {
                Brush_BG_Z = value;
                Brush_BG_Z.Freeze();
                OnPropertyChanged("BG_Z");
            }
        }
        private Brush Brush_BG_I = transparentBrush;
        public Brush BG_I {
            get {
                return Brush_BG_I;
            }
            set {
                Brush_BG_I = value;
                Brush_BG_I.Freeze();
                OnPropertyChanged("BG_I");
            }
        }
        private Brush Brush_BG_IRF = transparentBrush;
        public Brush BG_IRF {
            get {
                return Brush_BG_IRF;
            }
            set {
                Brush_BG_IRF = value;
                Brush_BG_IRF.Freeze();
                OnPropertyChanged("BG_IRF");
            }
        }

        private Brush brush_BG_ALU_X = transparentBrush;
        public Brush BG_ALU_X
        {
            get
            {
                return brush_BG_ALU_X;
            }
            set
            {
                brush_BG_ALU_X = value;
                brush_BG_ALU_X.Freeze();
                OnPropertyChanged("BG_ALU_X");
            }
        }

        private Brush brush_BG_ALU_Y = transparentBrush;
        public Brush BG_ALU_Y
        {
            get
            {
                return brush_BG_ALU_Y;
            }
            set
            {
                brush_BG_ALU_Y = value;
                brush_BG_ALU_Y.Freeze();
                OnPropertyChanged("BG_ALU_Y");
            }
        }
        #endregion

        // ========================================================
        // Auto Execution
        // ========================================================
        #region
        /// <summary>
        /// Stores the information wheter the automatic execution
        /// shall be performed or not. Note that this is the termination
        /// criteria for a running runThread. If this 
        /// value is false the tread is signaled to terminate
        /// (but may still run for a little while)
        /// </summary>
        private bool autoExecute;

        /// <summary>
        /// Starts the running thread
        /// </summary>
        /// <param name="stepType">Type of stepping</param>
        public void Run(StepType stepType)
        {
            if (runThread != null && runThread.IsAlive)
            {
                return;
            }
            autoExecute = true;

            runThread = new Thread(new ThreadStart(delegate
            {
                while (autoExecute)
                {
                    switch (stepType)
                    {
                        case StepType.MICRO:
                            MicroStep();
                            break;
                        case StepType.MACRO:
                            MacroStep();
                            break;
                        case StepType.INSTRUCTION:
                            InstructionStep();
                            break;
                    }
                    Thread.Sleep(Speed);
                }
            }));
            runThread.IsBackground = true;
            runThread.Start();
        }

        /// <summary>
        /// Stops the running thread
        /// </summary>
        public void Stop()
        {
            if (runThread != null)
            {
                autoExecute = false;
                if(!Thread.CurrentThread.Equals(runThread))
                {
                    runThread.Join();
                }
            }
        }

        #endregion

        /// <summary>
        /// Contains all the lines and arrows from the View, which are part of the visualization
        /// </summary> 
        public List<LineBase> arrows = new List<LineBase>();

        /// <summary>
        ///  Contains the IO device windows, for writing and reading from them
        /// </summary>
        public Dictionary<int, IOWrapperWindow> ioWindows = new Dictionary<int, IOWrapperWindow>();

        /// <summary>
        /// The Model of the processor
        /// </summary>
        public Processor proc;


        /// <summary>
        /// The thread for the auto run mode
        /// </summary>
        private Thread runThread;


        /// <summary>
        /// Constructor
        /// </summary>
        public ProcessorViewModel(Processor processor) {
            hwInteruptLock = new ReaderWriterLock();
            this.proc = processor;
            InitDecoderEntryViewModelList();
            InitMPMEntryViewModelList();
            InitAluEntries();
            CopyRegistersCommand = new CopyRegistersCommand(this);
            Reset();
        }

        /// <summary>
        /// Resets all the visualization
        /// </summary>
        /// <param name="showMicrosteps">True if microsteps are visualized and therefore have to be reset</param>
        public void ResetVisualization()
        {
            ResetBackgrounds();
            ResetArrows();
        }


        // ========================================================
        // Button-Functions which are called from the view
        // ========================================================

        /// <summary>
        /// Is called from the view and calls ExecuteMicroStep
        /// </summary>
        public void MicroStep()
        {
            ExecuteMicroStep();
        }

        /// <summary>
        /// Is called from the view and executes MicroSteps until a MacroStep is done
        /// </summary>
        public void MacroStep()
        {
            ResetVisualization();
            do
            {
                ExecuteMicroStep(false);
            }
            while ((MIP % 8 != 0) && !HasHalted && !(IsRunning && !autoExecute));
        }

        /// <summary>
        /// Is called from the view and executes the rest of the micro steps of the current assembler-command 
        /// </summary>
        public void InstructionStep()
        {
            ResetVisualization();
            do
            {
                ExecuteMicroStep(false);
            }
            while (MIP != 0 && !HasHalted && !(IsRunning && !autoExecute));
        }

        /// <summary>
        /// Resets all the registers, ram, visualization, stops the thread
        /// </summary>
        public void Reset()
        {
            // Stops the running thread
            if (IsRunning)
            {
                Stop();
            }

            // Reset the registers
            AL = 0;
            BL = 0;
            CL = 0;
            DL = 0;
            IP = 0;
            SP = 191;
            SR = 0;
            IR = 0;
            IRF = 0;
            SEL = 0;
            MBR = 0;
            MAR = 0;
            MDR = 0;
            RES = 0;
            ALU_X = 0;
            ALU_Y = 0;
            CurrentDecoderEntry = null;
            MIP = 0;
            
            // Reset the RAM
            Array.Clear(RAM, 0, Model.RAM.RAM_SIZE);
            if (ramChangedEvent != null)
            {
                ramChangedEvent();
            }

            // Reset the visualization
            ResetVisualization();

            ResetExecutionStates();
        }

        /// <summary>
        /// Resets the execution states which determine
        /// wether an application can be runned or not
        /// </summary>
        private void ResetExecutionStates()
        {
            // Reset the states
            HasHalted = false;
            ReadyToRun = false;
        }

        /// <summary>
        /// The sleep time of the thread(if the value is small -> the speed of the thread is fast)
        /// </summary>
        public int Speed
        {
            get;
            set;
        }

        /// <summary>
        /// Is set if the program was in auto run mode, and is now paused
        /// </summary>
        public bool HasHalted
        {
            get;
            set;
        }

        /// <summary>
        /// Is set if the program is ready to run(if the assembler code is assembled and the RAM is filled)
        /// </summary>
        public bool ReadyToRun
        {
            get;
            set;
        }

        /// <summary>
        /// Is true, when the running thread is alive
        /// </summary>
        public bool IsRunning
        {
            get
            {
                if (runThread == null)
                {
                    return false;
                }
                return runThread.IsAlive;
            }
        }

        public enum StepType
        {
            MICRO,
            MACRO,
            INSTRUCTION,
            NONE
        }


        /// <summary>
        /// Reset all background colors
        /// </summary>
        private void ResetBackgrounds() {
            BG_AL  = transparentBrush;
            BG_BL  = transparentBrush;
            BG_CL  = transparentBrush;
            BG_DL  = transparentBrush;
            BG_SP  = transparentBrush;
            BG_MBR = transparentBrush;
            BG_MAR = transparentBrush;
            BG_MDR = transparentBrush;
            BG_I   = transparentBrush;
            BG_O   = transparentBrush;
            BG_S   = transparentBrush;
            BG_Z   = transparentBrush;
            BG_RES = transparentBrush;
            BG_IR  = transparentBrush;
            BG_IP  = transparentBrush;
            BG_MIP = transparentBrush;
            BG_SEL = transparentBrush;
            BG_IRF = transparentBrush;
            BG_ALU_X = transparentBrush;
            BG_ALU_Y = transparentBrush;
        }
        
        /// <summary>
        /// resets the colors of the arrows
        /// </summary>
        private void ResetArrows() {
            if (arrowsHighlighted)
            {
                foreach (LineBase s in arrows)
                {
                    s.InvokeIfRequired(() =>
                    {
                        // Hide the arrow, if the Property AutoHide is set
                        if (s.AutoHide)
                        {
                            s.Stroke = transparentBrush;

                            // otherwise set the color to black
                        }
                        else
                        {
                            s.Stroke = blackBrush;
                        }
                    });
                }
                arrowsHighlighted = false;
            }
        }

        /// <summary>
        /// Remebers if there are any arrows
        /// that have been highlighted since the last
        /// execution of micro, macro or instruction step
        /// </summary>
        private bool arrowsHighlighted = false;

        /// <summary>
        /// Executes one micro step
        /// </summary>
        /// <param name="showMicroSteps">true: visualize the arrows\nfalse: don't visualize the arrows</param>
        private void ExecuteMicroStep(bool showMicroSteps = true) {
            // Reset the Visualization
            if (showMicroSteps)
            {
                ResetVisualization();
            }
            
            
            // get MicroStep
            MPMEntry mpmEntry = proc.mpmEntries[MIP];
            SetNextMIP(mpmEntry.Na, mpmEntry.Crit, mpmEntry.Val);

            // Write VAL to Data-Bus
            WriteValToDataBus(mpmEntry.Ev, mpmEntry.Val);

            // Read or write to a bus
            TransferData(mpmEntry);

            try
            {
                // Execute Alu Command
                AluCommand(mpmEntry.Alu, mpmEntry.Af);

                // Reset the IRF Flag
                if (mpmEntry.Cif)
                {
                    IRF = 0;
                }

                // Visualize the arrows
                // NOTE: the backgrounds of the registers are set by setting the properties
                if (showMicroSteps)
                {
                    Visualization(mpmEntry);
                }
            }
            catch (DivideByZeroException)
            {
                MessageBox.Show(String.Format("Division by Zero detected!\nExecution stopped"), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Stop();
                HasHalted = true;
            }
        }

        /// <summary>
        /// Transfers data from and to busses according to the instruction
        /// in the mpmentry
        /// </summary>
        /// <param name="entry">
        /// The instruction which contains
        /// the information from where the data shall be taken
        /// and where the data goes to.
        /// </param>
        private void TransferData(MPMEntry entry)
        {
           
           // Memory 
           // READ from Register
           Reg2Bus(entry);

           // WRITE to Register
           Bus2Reg(entry);
        }

        /// <summary>
        /// Visualize the arrows in the View
        /// </summary>
        /// <param name="mpmEntry">the current micro code entry</param>
        private void Visualization(MPMEntry mpmEntry) {

            hwInteruptLock.AcquireReaderLock(HW_INTERRUPT_LOCK_TIMEOUT);
            try
            {
                // VISUALIZATION
                foreach (LineBase s in arrows)
                {
                    string vis_from = VisualizationHelper.GetFromReg(mpmEntry, proc.SEL);
                    string vis_to = VisualizationHelper.GetToReg(mpmEntry, proc.SEL);
                    string vis_alc = VisualizationHelper.GetAluCommand(mpmEntry);
                    string vis_af = VisualizationHelper.GetAffectedFlag(mpmEntry);
                    string vis_mip = VisualizationHelper.GetMIP(mpmEntry, IsJump, IsJumpSuccessful, IsInterrupt);
                    string vis_ev = VisualizationHelper.GetEnableValue(mpmEntry);
                    string vis_cif = VisualizationHelper.GetCif(mpmEntry);
                    string vis_rw = VisualizationHelper.GetRw(mpmEntry);
                    foreach (Rule rule in s.Rules)
                    {
                        s.InvokeIfRequired(() =>
                        {

                            bool applies = rule.AppliesFrom(vis_from);
                            applies = applies && rule.AppliesTo(vis_to);
                            applies = applies && rule.AppliesAlc(vis_alc);
                            applies = applies && rule.AppliesAf(vis_af);
                            applies = applies && rule.AppliesMip(vis_mip);
                            applies = applies && rule.AppliesEv(vis_ev);
                            applies = applies && rule.AppliesCif(vis_cif);
                            applies = applies && rule.AppliesRw(vis_rw);
                            applies = applies && !rule.Inverted;
                            if (applies)
                            {
                                s.Stroke = new SolidColorBrush(s.HighlightColor);
                                arrowsHighlighted = true;
                            }
                        });
                    }
                }
                
            }
            finally
            {
                hwInteruptLock.ReleaseReaderLock();
            }
        }


        /// <summary>
        /// Set the next MIP
        /// </summary>
        /// <param name="na">Next MIP address flag</param>
        /// <param name="crit">Criteria for the jumps</param>
        /// <param name="val">Value to jump if jump is successful</param>
        private void SetNextMIP( MPMEntry.NA na, MPMEntry.CRIT crit, int val) {
            hwInteruptLock.AcquireReaderLock(HW_INTERRUPT_LOCK_TIMEOUT);
            try
            {
                switch (na)
                {
                    case MPMEntry.NA.NEXT: // Next Branch Address
                        if (IsJump(crit) && IsJumpSuccessful(crit))
                        {
                            MIP += val;

                        }
                        else
                        {
                            MIP++;
                        }
                        break;
                    case MPMEntry.NA.DECODE: // Decoder Address
                        if (CurrentDecoderEntry != null
                            && CurrentDecoderEntry.Model.OpCode == 0)
                        {
                            HasHalted = true;
                            Stop();
                        }
                        
                        UpdateCurrentDecoderEntry();   
                        MIP = CurrentDecoderEntry.Model.MPMAddress;
                        break;
                        
                    case MPMEntry.NA.FETCH:

                        // Interrupt?
                        if (IsInterrupt())
                        {
                            MIP = 0x10;
                        }
                        else
                        {
                            MIP = 0x00;
                        }
                        break;
                }
            }
            finally
            {
                hwInteruptLock.ReleaseReaderLock();
            }
        }

        /// <summary>
        /// Copies a value(if requiered) to the Data-Bus
        /// </summary>
        /// <param name="enableValue">Flag, just copy if the flag is true</param>
        /// <param name="val">Value to copy</param>
        private void WriteValToDataBus(bool enableValue, int val) {
            if (enableValue) {
                proc.Data.Write((byte)val);
            }
        }

        /// <summary>
        /// Copies a value from a register to a bus
        /// </summary>
        /// <param name="bs">Bus flag</param>
        /// <param name="Regs">Registers flag</param>
        private void Reg2Bus(MPMEntry entry ) {

            // Get the selected bus
            IBus bus = GetBus(entry);
            if (bus == null) {
                return;
            }

            // Checks which from register is set and writes its value to the selected bus
            if (SRCRegister.FROM_SEL_REF.Equals(entry.Src))
            {
                switch (SEL)
                {
                    case (byte)Processor.REGISTER.AL:
                        bus.Write(AL);
                        break;
                    case (byte)Processor.REGISTER.BL:
                        bus.Write(BL);
                        break;
                    case (byte)Processor.REGISTER.CL:
                        bus.Write(CL);
                        break;
                    case (byte)Processor.REGISTER.DL:
                        bus.Write(DL);
                        break;
                    case (byte)Processor.REGISTER.SP:
                        bus.Write(SP);
                        break;
                    default:
                        ShowIllegalSelValueDialog();
                        Stop();
                        ResetExecutionStates();
                        return;
                }
                return;
            }

            if (SRCRegister.FROM_DATA.Equals(entry.Src))
            {
                bus.Write(ReadFromDataBus(entry));
            }

            if (SRCRegister.FROM_IP.Equals(entry.Src))
            {
                bus.Write(IP);
                return;
            }

            if (SRCRegister.FROM_MBR.Equals(entry.Src))
            {
                bus.Write(MBR);
                return;
            }

            if (SRCRegister.FROM_MAR.Equals(entry.Src)) { 
                bus.Write(MAR);
                return;
            }

            if (SRCRegister.FROM_MDR.Equals(entry.Src))
            { 
                bus.Write(MDR);
                return;
            }

            if (SRCRegister.FROM_RES.Equals(entry.Src))
            { 
                bus.Write(RES);
                return;
            }

            if (SRCRegister.FROM_SR.Equals(entry.Src))
            { 
                bus.Write(SR);
            }
        }

        private void ShowIllegalSelValueDialog()
        {
            StringBuilder errorMessage = new StringBuilder();
            errorMessage.AppendLine(String.Format("Invalid SEL: {0:X2}", SEL));
            errorMessage.AppendLine("Stebs tried to resolve the value from SEL to a");
            errorMessage.AppendLine("known register but cannot match it.");
            errorMessage.AppendLine();
            errorMessage.Append("Valid values are: ");
            errorMessage.Append(String.Format("AL: {0:X2}, ", (byte)Processor.REGISTER.AL));
            errorMessage.Append(String.Format("BL: {0:X2}, ", (byte)Processor.REGISTER.BL));
            errorMessage.Append(String.Format("CL: {0:X2}, ", (byte)Processor.REGISTER.CL));
            errorMessage.Append(String.Format("DL: {0:X2}, ", (byte)Processor.REGISTER.DL));
            errorMessage.Append(String.Format("SP: {0:X2}  ", (byte)Processor.REGISTER.SP));
            MessageBox.Show(errorMessage.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        /// <summary>
        /// Copies a value from a register to a bus
        /// </summary>
        /// <param name="bs">Bus flag</param>
        /// <param name="Regs">Registers flag</param>
        private void Bus2Reg(MPMEntry entry) {

            // Get the selected bus
            IBus bus = GetBus(entry);
            if (bus == null)
            {
                return;
            }

            // Checks which to register is set and writes the value on the selected bus in the register
            if ( DESTRegister.TO_SEL.Equals(entry.Dst))
            {
                SEL = bus.Read();
                return;
            }

            if (DESTRegister.TO_SEL_REF.Equals(entry.Dst))
            {   // to [SEL]
                switch (SEL) {
                    case(byte)Processor.REGISTER.AL:
                        AL = bus.Read();
                        break;
                    case(byte)Processor.REGISTER.BL:
                        BL = bus.Read();
                        break;
                    case(byte)Processor.REGISTER.CL:
                        CL = bus.Read();
                        break;
                    case(byte)Processor.REGISTER.DL:
                        DL = bus.Read();
                        break;
                    case(byte)Processor.REGISTER.SP:
                        SP = bus.Read();
                        break;
                    default:
                        ShowIllegalSelValueDialog();
                        Stop();
                        ResetExecutionStates();
                        break;
                }
                return;
            }

            if (DESTRegister.TO_IP.Equals(entry.Dst))
            {
                IP = bus.Read();
                return;
            }

            if (DESTRegister.TO_MBR.Equals(entry.Dst))
            { 
                MBR = bus.Read();
                return;
            }
            
            if (DESTRegister.TO_MAR.Equals( entry.Dst))
            {   
                MAR = bus.Read();
                return;
            }

            if (DESTRegister.TO_MDR.Equals(entry.Dst))
            { 
                MDR = bus.Read();
                if (!entry.Rw)
                {
                    WriteDataBusToIoM(entry);
                }
                return;
            }
            
            if (DESTRegister.TO_RES.Equals(entry.Dst)) { 
                RES = bus.Read();
                return;
            }

            if (DESTRegister.TO_SR.Equals( entry.Dst))
            {
                SR = bus.Read();
                return;
            }

            if (DESTRegister.TO_IR.Equals(entry.Dst))
            {
                IR = bus.Read();
                return;
            }

            if (DESTRegister.TO_X.Equals(entry.Dst))
            {
                ALU_X = bus.Read();
                return;
            }

            if( DESTRegister.TO_Y.Equals(entry.Dst))
            {
                ALU_Y = bus.Read();
                return;
            }
        }

        
        /// <summary>
        /// Reads from the DATA-Bus (RAM or Interface Bus)
        /// into MDR
        /// </summary>
        /// <param name="entry">the MPMEntry</param>
        private byte ReadFromDataBus(MPMEntry entry) {
            AutoResetEvent ready = new AutoResetEvent(false);
            byte value = 0;
            // IO
            if (entry.IoM)
            {
                if (ioWindows.ContainsKey(MAR)) {
                    ioWindows[MAR].InvokeIfRequired(() => {
                        try {
                            value = ioWindows[MAR].Read();
                        }
                        catch (NotImplementedException) {
                            logger.Warn("IO-Device IN not supported!");
                            ShowIOReadNotSupportedDialog();
                            Stop();
                            ResetExecutionStates();
                        }
                        catch (Exception) {
                            logger.Error("Error occured in the IO Device");
                            MessageBox.Show("Error occured in the IO Device", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }

                        if (ioWindows[MAR].State == AvalonDock.DockableContentState.Hidden) {
                            ioWindows[MAR].Show();
                        }
                        ready.Set();
                    });
                    ready.WaitOne();
                } else {
                    logger.Warn("Undefined IO Port");
                    ShowUndefinedIOPortDialog();
                    Stop();
                    ResetExecutionStates();
                }

            }
            else
            {
                // RAM
                value = proc.ExternalData.Read();
            }
            return value;
        }

        /// <summary>
        /// Shows a dialog that an undefined IO port was used
        /// </summary>
        private static void ShowUndefinedIOPortDialog()
        {
            StringBuilder errorMsg = new StringBuilder();
            errorMsg.AppendLine("Undefined IO Port!");
            errorMsg.AppendLine("The port address you use is not known to stebs.");
            errorMsg.AppendLine();
            errorMsg.AppendLine("Please have a look at your Memory Address Register (MAR)");
            errorMsg.AppendLine("and compare it with the port addresses of all your loaded");
            errorMsg.AppendLine("interfaces (the number in brackets on each interface panel).");
            errorMsg.AppendLine();
            errorMsg.AppendLine("Correct your code and assemble it again.");
            MessageBox.Show(errorMsg.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        /// <summary>
        /// Shows a dialog that reading from a given IO-Port is now allowed
        /// </summary>
        private static void ShowIOReadNotSupportedDialog()
        {
            StringBuilder errorMsg = new StringBuilder();
            errorMsg.AppendLine("IO Device IN not supported!");
            errorMsg.AppendLine("You tried to read from an IO Device which does not support");
            errorMsg.AppendLine("reading.");
            errorMsg.AppendLine();
            errorMsg.AppendLine("Please have a look at your Memory Address Register (MAR) to");
            errorMsg.AppendLine("identify which IO Device you tried to read from.");
            errorMsg.AppendLine();
            errorMsg.AppendLine("Correct your code and assemble it again.");
            MessageBox.Show(errorMsg.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        /// <summary>
        /// Shows a dialog that writing to a specific IO port
        /// is now allowed
        /// </summary>
        private static void ShowIOWriteNotSupportedDialog()
        {
            StringBuilder errorMsg = new StringBuilder();
            errorMsg.AppendLine("IO Device OUT not supported!");
            errorMsg.AppendLine("You tried to write to an IO Device, which does not support");
            errorMsg.AppendLine("writing.");
            errorMsg.AppendLine();
            errorMsg.AppendLine("Please have a look at your Memory Address Register (MAR) to");
            errorMsg.AppendLine("identify which IO Device you tried to write to.");
            errorMsg.AppendLine();
            errorMsg.AppendLine("Correct your code and assemble it again.");
            MessageBox.Show(errorMsg.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        /// <summary>
        /// Writes to the DATA-Bus(RAM or IO-Device)
        /// </summary>
        /// <param name="entry">Indicates wheter the bus reads or writes a value</param>
        private void WriteDataBusToIoM(MPMEntry entry)
        {
            // WRITING?
            if (entry.Rw)
            {
                return;     
            }

            // IO
            if (entry.IoM) {
                if (ioWindows.ContainsKey(MAR)) {
                    AutoResetEvent ready = new AutoResetEvent(false);
                    ioWindows[MAR].InvokeIfRequired(() => {
                        try {
                            ioWindows[MAR].Write(MDR);
                        } catch (NotImplementedException) {
                            logger.Warn("IO Write not supported");
                            ShowIOWriteNotSupportedDialog();
                            Stop();
                            ResetExecutionStates();

                        } catch (Exception) {
                            logger.Error("Error occured in the IO Device");
                            MessageBox.Show("Error occured in the IO Device", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }

                        if (ioWindows[MAR].State == AvalonDock.DockableContentState.Hidden) {
                            ioWindows[MAR].Show();
                        }
                        ready.Set();
                    });
                    ready.WaitOne();
                }
                else
                {
                    ShowUndefinedIOPortDialog();
                    Stop();
                    ResetExecutionStates();
                }

            // Mem
            } else {
                proc.ExternalData.Write(MDR);
                if (ramChangedEvent != null) {
                    ramChangedEvent();
                }
            }
        }

        /// <summary>
        /// Executes an ALU-Command
        /// </summary>
        /// <param name="op">Alu operation</param>
        /// <param name="affectFlags">Affects the status-flags if set to true</param>
        private void AluCommand(Alu.Cmd op, bool affectFlags) {
            if (op != Alu.Cmd.NOP) {
                proc.alu.Execute(op, affectFlags);
                if (aluChangedEvent != null) {
                    aluChangedEvent.BeginInvoke(op,null,null);
                }

                // Call OnPropertyChanged so the View is updated and set the Background color of the RES register
                OnPropertyChanged("RES");
                BG_RES = yellowBrush;

                // if the flags are affected, call the OnPropertyChanged to update the View, and set the background colors of the status-flags
                if (affectFlags) {
                    OnPropertyChanged("S");
                    OnPropertyChanged("O");
                    OnPropertyChanged("Z");
                    OnPropertyChanged("SR");

                    BG_S = yellowBrush;
                    BG_O = yellowBrush;
                    BG_Z = yellowBrush;
                }
            }
		}

        /// <summary>
        /// Checks if a Jump is requested
        /// </summary>
        /// <param name="crit">Criteria flag</param>
        /// <returns>True if a Jump is requested</returns>
        private bool IsJump(MPMEntry.CRIT crit) {
            return crit != 0;
 
        }

        /// <summary>
        /// Checks if the Jump is successful
        /// </summary>
        /// <param name="crit">Criteria flag</param>
        /// <returns>True if the jump was successful</returns>
        private bool IsJumpSuccessful(MPMEntry.CRIT crit) {
            // Checks if the criteria flag and its corresponding status-flag are set
            return crit == MPMEntry.CRIT.S && proc.S
                || crit == MPMEntry.CRIT.O && proc.O
                || crit == MPMEntry.CRIT.Z && proc.Z
                || crit == MPMEntry.CRIT.NS && !proc.S
                || crit == MPMEntry.CRIT.NO && !proc.O
                || crit == MPMEntry.CRIT.NZ && !proc.Z;
           
        }

        /// <summary>
        /// Checks if there's an interrupt
        /// </summary>
        /// <returns>True if there's an interrupt</returns>
        private bool IsInterrupt() {
            hwInteruptLock.AcquireReaderLock(HW_INTERRUPT_LOCK_TIMEOUT);
            try
            {
                // Checks if the Interrupt flag and the Allow interrupt flag is set
                return IRF == 1 && I == 1;
            }
            finally
            {
                hwInteruptLock.ReleaseReaderLock();
            }
        }

        /// <summary>
        /// Returns the selected bus
        /// </summary>
        /// <param name="Bs">Bus flag</param>
        /// <returns>returns the selected bus</returns>
        private IBus GetBus(MPMEntry entry) {
            return proc.Data;
        }

        /// <summary>
        /// Initialises the decoder entry list
        /// </summary>
        private void InitDecoderEntryViewModelList()
        {
            List<DecoderEntryViewModel> entries = new List<DecoderEntryViewModel>();
            foreach (DecoderEntry entry in proc.DecoderEntries)
            {
                entries.Add(new DecoderEntryViewModel(entry));
            }
            DecoderEntries = entries;
        }

        /// <summary>
        /// Initialises the mpm entry list
        /// </summary>
        private void InitMPMEntryViewModelList()
        {
            List<MPMEntryViewModel> entries = new List<MPMEntryViewModel>();
            foreach (MPMEntry entry in proc.mpmEntries.Values)
            {
                entries.Add(new MPMEntryViewModel(entry));
            }
            MPMEntries = entries;
        }

        /// <summary>
        /// Initialises the ALU data grid
        /// </summary>
        private void InitAluEntries()
        {
            List<AluDataGridEntry> entries = new List<AluDataGridEntry>();
            entries.Add(new AluDataGridEntry { Op = Alu.Cmd.ADD });
            entries.Add(new AluDataGridEntry { Op = Alu.Cmd.SUB });
            entries.Add(new AluDataGridEntry { Op = Alu.Cmd.MUL });
            entries.Add(new AluDataGridEntry { Op = Alu.Cmd.DIV });
            entries.Add(new AluDataGridEntry { Op = Alu.Cmd.MOD });
            entries.Add(new AluDataGridEntry { Op = Alu.Cmd.DEC });
            entries.Add(new AluDataGridEntry { Op = Alu.Cmd.INC });
            entries.Add(new AluDataGridEntry { Op = Alu.Cmd.OR });
            entries.Add(new AluDataGridEntry { Op = Alu.Cmd.XOR });
            entries.Add(new AluDataGridEntry { Op = Alu.Cmd.NOT });
            entries.Add(new AluDataGridEntry { Op = Alu.Cmd.AND });
            entries.Add(new AluDataGridEntry { Op = Alu.Cmd.SHR });
            entries.Add(new AluDataGridEntry { Op = Alu.Cmd.SHL });
            entries.Add(new AluDataGridEntry { Op = Alu.Cmd.ROR });
            entries.Add(new AluDataGridEntry { Op = Alu.Cmd.ROL });
            AluEntries = entries;
        }

        /**
         * Command that is executed when copying registers is requested
         */
        public ICommand CopyRegistersCommand
        {
            get;
            private set;
        }

        /// <summary>
        /// Shows a error dialog that an illegal opcode was found.
        /// 
        /// </summary>
        private void ShowIllegalOpcodeDialog()
        {
            StringBuilder errorMessage = new StringBuilder();
            errorMessage.AppendLine(String.Format("Illegal Opcode: {0:X2}", proc.IR));
            errorMessage.AppendLine("This may happen if there is an invalid value in the");
            errorMessage.AppendLine("Instruction Register (IR).");
            errorMessage.AppendLine();
            errorMessage.AppendLine("The processor tried to resolve this value to an opcode and");
            errorMessage.AppendLine("could not find a match in the opcode table.");
            errorMessage.AppendLine();
            errorMessage.AppendLine("Please have a look at your RAM. There is a red marked position");
            errorMessage.AppendLine("which indicates your current point of execution. The highlighted");
            errorMessage.AppendLine("value is probably the invalid code. Try to find out where this");
            errorMessage.AppendLine("value comes from.");
            errorMessage.AppendLine();
            errorMessage.AppendLine("Correct your code and assemble it again.");
            MessageBox.Show(errorMessage.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        /// <summary>
        /// Sets the current decoder entry depending on the processor IR value
        /// </summary>
        private void UpdateCurrentDecoderEntry()
        {
            var query =
                from decEntry in DecoderEntries
                where decEntry.Model.OpCode == proc.IR
                select decEntry;

            if (query.Count() != 1)
            {
                ShowIllegalOpcodeDialog();
                Stop();
                ResetExecutionStates();
                return;
            }
            CurrentDecoderEntry = query.First();
            OnPropertyChanged("CurrentDecoderEntry");
        }

        /// <summary>
        /// Shows a dialog that the current MIP value is not valid.
        /// </summary>
        private void ShowIllegalMipDialog()
        {
            StringBuilder errorMessage = new StringBuilder();
            errorMessage.AppendLine(String.Format("Illegal MIP: {0:X2}", proc.MIP));
            errorMessage.AppendLine("There is a value in the MIP register which can not be");
            errorMessage.AppendLine("resolved to an instruction address.");
            errorMessage.AppendLine();
            errorMessage.AppendLine("This can happen if there is a mistake in the processor");
            errorMessage.AppendLine("microcode data (i.e. ROM1.data, ROM2.data)");
            MessageBox.Show(errorMessage.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
