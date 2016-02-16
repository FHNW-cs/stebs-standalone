using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Helper;

namespace IOLightLib
{
    /// <summary>
    /// Interaction logic for OutputLightWindow.xaml
    /// </summary>
    public partial class IOLightWindow : UserControl, IOInterfaceLib.IIOWindow
    {
        public event IOInterfaceLib.FireInterruptHandler FireInterruptEvent;

        /// <summary>
        /// The Byte Value
        /// </summary>
        private byte lightByte = 0;


        /// <summary>
        /// Constructor
        /// </summary>
        public IOLightWindow () {
            InitializeComponent ();
            ReDraw ();
        }

        /// <summary>
        /// The IO-Device Name
        /// </summary>
        /// <returns>The name as string</returns>
        public string GetName () {
            return "IO Light";
        }

        /// <summary>
        /// Returns the current byte value
        /// </summary>
        /// <returns>the byte value</returns>
        public byte Read () {
            return lightByte;
        }

        /// <summary>
        /// Sets the current byte value
        /// </summary>
        /// <param name="input">new byte value</param>
        public void Write (byte input) {
            lightByte = input;
            ReDraw ();
        }

        /// <summary>
        /// redraws the output in the XAML
        /// </summary>
        private void ReDraw () {
            labelLightByte.Content = lightByte.ToBinString ();
            labelLightHex.Content = lightByte.ToHexString ();
            labelLightTwoComplement.Content = lightByte.ToTwoComplementString();

            rightWalk.Visibility = ((lightByte & "00000001".BinToByte ()) != 0) ? Visibility.Visible :  Visibility.Hidden;
            rightGreen.Fill     = new SolidColorBrush (((lightByte & "00000010".BinToByte()) != 0) ? Colors.LawnGreen : Colors.Black);
            rightYellow.Fill    = new SolidColorBrush (((lightByte & "00000100".BinToByte()) != 0) ? Colors.Yellow : Colors.Black);
            rightRed.Fill       = new SolidColorBrush (((lightByte & "00001000".BinToByte ()) != 0) ? Colors.Red : Colors.Black);

            leftWalk.Visibility = ((lightByte & "00010000".BinToByte ()) != 0) ? Visibility.Visible :  Visibility.Hidden;
            leftGreen.Fill      = new SolidColorBrush (((lightByte & "00100000".BinToByte()) != 0) ? Colors.LawnGreen : Colors.Black);
            leftYellow.Fill     = new SolidColorBrush (((lightByte & "01000000".BinToByte()) != 0) ? Colors.Yellow : Colors.Black);
            leftRed.Fill        = new SolidColorBrush (((lightByte & "10000000".BinToByte ()) != 0) ? Colors.Red : Colors.Black);
        }

        
    }
}
