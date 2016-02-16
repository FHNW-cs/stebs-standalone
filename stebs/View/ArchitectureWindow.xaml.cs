using Stebs.ViewModel;
using Stebs.Model;
using Stebs.View.CustomControls;
using System.Diagnostics;
using System;
using System.Windows.Media;
using System.Windows.Controls;
using System.Threading;
using System.Threading.Tasks;

namespace Stebs.View
{
    /// <summary>
    /// Interaction logic for ArchitectureWindow.xaml
    /// </summary>
    public partial class ArchitectureWindow : AvalonDock.DockableContent
    {
        /// <summary>
        /// The Processor ViewModel
        /// </summary>
        private ProcessorViewModel processorVM;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="processorVM">The processor ViewModel</param>
        public ArchitectureWindow (ProcessorViewModel processorVM)
        {
            InitializeComponent();

            this.processorVM = processorVM;

            // Set the DataContext
            DataContext = processorVM;

            // Register the Events in the Processor ViewModel
            processorVM.mipChangedEvent += MIPChanged;
            processorVM.irChangedEvent += IRChanged;
            processorVM.aluChangedEvent += AluChanged;
        }

        //private int lastSelectedMIPIndex = 0;

        /// <summary>
        /// Is called from the ViewModel, if the MIP (Micro Instruction Pointer) has changed
        /// Selects the current Micro Instruction in the MPM (Micro Program Memory) DataGrid and Scrolls it into View
        /// </summary>
        public void MIPChanged () {
            gridMPM.InvokeIfRequired (() => {
                if (gridMPM.SelectedItem == null)
                    return;

                if (gridMPM.Items.Count > 0)
                {
                    var border = VisualTreeHelper.GetChild(gridMPM, 0) as Decorator;
                    if (border != null)
                    {
                        var scroll = border.Child as ScrollViewer;
                        if (scroll != null)
                            scroll.ScrollToVerticalOffset(gridMPM.SelectedIndex - 1); // .ScrollToEnd();
                    }
                }

            });
        }

        //private int lastSelectedIRIndex = 0;
        private int redrawPendingIR = 0;

        /// <summary>
        /// Is called from the ViewModel, if the IR (Instruction Register) has changed
        /// Selects the current Instruction in the Opcode Decoder DataGrid and Scrolls it into View
        /// </summary>
        public void IRChanged () {
            gridDecoder.InvokeIfRequired (async () => {
                if (gridDecoder.SelectedItem == null)
                    return;

                if (Interlocked.CompareExchange(ref redrawPendingIR, 1, 0) == 0)
                {
                    await Task.Delay(100);

                    if (gridDecoder.Items.Count > 0)
                    {
                        var border = VisualTreeHelper.GetChild(gridDecoder, 0) as Decorator;
                        if (border != null)
                        {
                            var scroll = border.Child as ScrollViewer;
                            if (scroll != null)
                                scroll.ScrollToVerticalOffset(gridDecoder.SelectedIndex - 1); // .ScrollToEnd();
                        }
                    }
                    Interlocked.CompareExchange(ref redrawPendingIR, 0, 1);
                }
            });
        }

        private int redrawPendingAlu = 0;
        private volatile Alu.Cmd nextOp;

        /// <summary>
        /// Is called from the ViewModel, if the ALU Command has changed
        /// Selects the current ALU Command in the ALU DataGrid and Scrolls it into View
        /// </summary>
        public void AluChanged (Alu.Cmd op) {
            gridALU.InvokeIfRequired (async () => {
                nextOp = op;

                if (Interlocked.CompareExchange(ref redrawPendingAlu, 1, 0) == 0)
                {
                    await Task.Delay(100);

                    if (gridALU.Items.Count > 0)
                    {
                        var border = VisualTreeHelper.GetChild(gridALU, 0) as Decorator;
                        if (border != null)
                        {
                            var scroll = border.Child as ScrollViewer;
                            if (scroll != null)
                                scroll.ScrollToVerticalOffset(gridALU.SelectedIndex - 1); // .ScrollToEnd();
                        }
                    }

                    Interlocked.CompareExchange(ref redrawPendingAlu, 0, 1);

                    gridALU.SelectedItem = new ProcessorViewModel.AluDataGridEntry { Op = nextOp };
                }
            });
        }

        /// <summary>
        /// Adds the Lines and Arrows on the Architecture View to a List in the ProcessorViewModel, which is used for highlight the arrows and lines
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Architecture_Loaded (object sender, System.Windows.RoutedEventArgs e) {
            if (processorVM.arrows.Count > 0) {
                return;
            }

            // Loop through all Children in the grid
            foreach (object o in grid.Children) {

                // Check if its a derrived Type of LineBase
                if (o is LineBase) {
                    LineBase s = (LineBase)o;

                    if( s.Rules.Count > 0)
                    {
                        processorVM.arrows.Add (s);
                    }
                }
            }
            processorVM.ResetVisualization ();
        }

        /// <summary>
        /// Is called by clicking the HWInterrupt-Button on the ArchitectureView
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HWInterrupt_Click (object sender, System.Windows.RoutedEventArgs e) {
            processorVM.IRF = 1;
        }
    }
}
