using System.Windows.Controls;
using System.Threading;
using System.Windows;
using Helper;

namespace IOInterruptLib
{
    /// <summary>
    /// Interaction logic for IOInterruptWindow.xaml
    /// </summary>
    public partial class IOInterruptWindow : UserControl, IOInterfaceLib.IIOWindow
    {

        /// <summary>
        /// Interrupt thread
        /// </summary>
        private Thread interruptThread;

        public event IOInterfaceLib.FireInterruptHandler FireInterruptEvent;

        /// <summary>
        /// Constructor
        /// </summary>
        public IOInterruptWindow() {
            InitializeComponent();
            
        }

        /// <summary>
        /// Returns the current byte value
        /// </summary>
        /// <returns>always 0</returns>
        public byte Read() {
            return 0;
        }

        /// <summary>
        /// Sets the current byte value
        /// </summary>
        /// <param name="input">new byte value</param>
        public void Write(byte input) {
        }

        /// <summary>
        /// The IO-Device Name
        /// </summary>
        /// <returns>The name as string</returns>
        public string GetName() {
            return "IO Interrupt";
        }

        /// <summary>
        /// the periodic interrupt is enabled or disabled
        /// </summary>
        private void chbPeriodIR_Click(object sender, System.Windows.RoutedEventArgs e) {
            if (chbPeriodIR.IsChecked == true) {
                if (interruptThread == null) {

                    interruptThread = new Thread(new ThreadStart(delegate {
                        int sleep = 0;
                        while(true) {
                            Interrupt();
                            sliderPeriodIR.InvokeIfRequired(() => {
                                sleep =(int)(sliderPeriodIR.Value * 1000);
                            });
                            Thread.Sleep(sleep);
                        }
                    }));

                    interruptThread.IsBackground = true;

                    interruptThread.Start();
                }
            } else {
                if (interruptThread != null) {
                    interruptThread.Abort();
                    interruptThread = null;
                }
            }
        }

        /// <summary>
        /// The single interrupt button was pressed
        /// </summary>
        private void btnSingleIR_Click(object sender, RoutedEventArgs e) {
            Interrupt();
        }

        /// <summary>
        ///  Fires the interrupt event
        /// </summary>
        private void Interrupt()
        {
            if (FireInterruptEvent != null) {
                FireInterruptEvent();
            }
        }

    }
}
