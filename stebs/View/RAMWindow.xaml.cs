using Stebs.ViewModel;
using System.Threading;
using System.Threading.Tasks;

namespace Stebs.View
{
    /// <summary>
    /// Interaction logic for RAMWindow.xaml
    /// </summary>
    public partial class RAMWindow : AvalonDock.DockableContent
    {
        /// <summary>
        /// The processor ViewModel
        /// </summary>
        private ProcessorViewModel procVM;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="processorVM">THe Processor ViewModel</param>
        public RAMWindow(ProcessorViewModel processorVM) {
            InitializeComponent();

            procVM = processorVM;

            // Set the events in the ViewModel
            procVM.ramChangedEvent += ReWrite;
            procVM.ipspChangedEvent += ReWrite;

            // Update the TextBox
            ReWrite();
        }

        private int redrawPending = 0;

        /// <summary>
        /// Updates the Textbox
        /// </summary>
        private void ReWrite() {
            RAMTextBox.InvokeIfRequired(async () => {
                RAMTextBox.ram = procVM.RAM;
                RAMTextBox.ip = procVM.IP;
                RAMTextBox.sp = procVM.SP;

                if (Interlocked.CompareExchange(ref redrawPending, 1, 0) == 0)
                {
                    await Task.Delay(100);

                    RAMTextBox.InvalidateVisual();
                    Interlocked.CompareExchange(ref redrawPending, 0, 1);
                }
            });
        }

        /// <summary>
        /// Is called, by changing the Mode to ASCII
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radioAscii_Click(object sender, System.Windows.RoutedEventArgs e) {
            RAMTextBox.IsHex = false;
            ReWrite();
        }

        /// <summary>
        /// Is called by changing the Mode to HEX
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radioHex_Click(object sender, System.Windows.RoutedEventArgs e) {
            RAMTextBox.IsHex = true;
            ReWrite();
        }
    }
}
