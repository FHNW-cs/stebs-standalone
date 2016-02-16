using System.Windows.Controls;
using IOInterfaceLib;
using System.Windows.Media.Imaging;

namespace Stebs.View
{
    /// <summary>
    /// Interaction logic for IOWrapperWindow.xaml
    /// </summary>
    public partial class IOWrapperWindow : AvalonDock.DockableContent, IIOWindow
    {
        /// <summary>
        /// The inner Class(a class which implements the interface IIOWindow)
        /// </summary>
        private IIOWindow inner;

        /// <summary>
        /// Redirect the Interrupt Event
        /// </summary>
        public event FireInterruptHandler FireInterruptEvent {
            add {
                inner.FireInterruptEvent += value;
            }
            remove {
                inner.FireInterruptEvent -= value;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="inner">The inner class</param>
        public IOWrapperWindow(IIOWindow inner) {
            this.inner = inner;
            InitializeComponent();

            // Add the window to the panel
            panel.Children.Add((UserControl)inner);
        }

        /// <summary>
        /// Returns the Name of the inner Window class
        /// </summary>
        /// <returns>the Name of the inner Window class</returns>
        public string GetName() {
            return inner.GetName();
        }

        /// <summary>
        /// Writes a value to the inner Window class
        /// </summary>
        /// <param name="input">Value to write</param>
        public void Write(byte input) {
            inner.Write(input);
        }

        /// <summary>
        /// Reads a Value from the inner Window Class
        /// </summary>
        /// <returns>The value read from the inner Window Class</returns>
        public byte Read() {
            return inner.Read();
        }

        /// <summary>
        /// The Icon to display in the RibbonMenu
        /// </summary>
        public BitmapImage RibbonIcon {
            get;
            set;
        }

    }
}
