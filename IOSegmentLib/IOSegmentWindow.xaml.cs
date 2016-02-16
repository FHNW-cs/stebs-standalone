using System;
using System.Windows;
using System.Windows.Controls;
using Helper;

namespace IOSegmentLib
{
    /// <summary>
    /// Interaction logic for OutputSegmentWindow.xaml
    /// </summary>
    public partial class IOSegmentWindow : UserControl, IOInterfaceLib.IIOWindow
    {
        public event IOInterfaceLib.FireInterruptHandler FireInterruptEvent;

        /// <summary>
        /// The Byte Value
        /// </summary>
        private byte segmentByte = 0;


        /// <summary>
        /// Constructor
        /// </summary>
        public IOSegmentWindow() {
            InitializeComponent();

            // Right and Left Clean Up
            Write("10000000".BinToByte());
            Write("00000000".BinToByte());
        }

        /// <summary>
        /// The IO-Device Name
        /// </summary>
        /// <returns>The name as string</returns>
        public string GetName() {
            return "IO 7 Segment";
        }

        /// <summary>
        /// Returns the current byte value
        /// </summary>
        /// <returns>the byte value</returns>
        public byte Read() {
            return segmentByte;
        }

        /// <summary>
        /// Sets the current byte value
        /// </summary>
        /// <param name="input">new byte value</param>
        public void Write(byte input) {
            segmentByte = input;
            ReWrite();
        }

        /// <summary>
        /// redraws the output in the XAML
        /// </summary>
        private void ReWrite() {
            labelSegmentByte.Content = segmentByte.ToBinString();
            labelSegmentHex.Content = segmentByte.ToHexString();
            labelSegmentTwoComplement.Content = segmentByte.ToTwoComplementString();

            // Left or Right Segment
            if ((segmentByte & "10000000".BinToByte()) == 0) {
                leftA.Visibility =((segmentByte & "00000001".BinToByte()) == 0) ? Visibility.Hidden : Visibility.Visible;
                leftB.Visibility =((segmentByte & "00000010".BinToByte()) == 0) ? Visibility.Hidden : Visibility.Visible;
                leftC.Visibility =((segmentByte & "00000100".BinToByte()) == 0) ? Visibility.Hidden : Visibility.Visible;
                leftD.Visibility =((segmentByte & "00001000".BinToByte()) == 0) ? Visibility.Hidden : Visibility.Visible;
                leftE.Visibility =((segmentByte & "00010000".BinToByte()) == 0) ? Visibility.Hidden : Visibility.Visible;
                leftF.Visibility =((segmentByte & "00100000".BinToByte()) == 0) ? Visibility.Hidden : Visibility.Visible;
                leftG.Visibility =((segmentByte & "01000000".BinToByte()) == 0) ? Visibility.Hidden : Visibility.Visible;
            } else {
                rightA.Visibility =((segmentByte & "00000001".BinToByte()) == 0) ? Visibility.Hidden : Visibility.Visible;
                rightB.Visibility =((segmentByte & "00000010".BinToByte()) == 0) ? Visibility.Hidden : Visibility.Visible;
                rightC.Visibility =((segmentByte & "00000100".BinToByte()) == 0) ? Visibility.Hidden : Visibility.Visible;
                rightD.Visibility =((segmentByte & "00001000".BinToByte()) == 0) ? Visibility.Hidden : Visibility.Visible;
                rightE.Visibility =((segmentByte & "00010000".BinToByte()) == 0) ? Visibility.Hidden : Visibility.Visible;
                rightF.Visibility =((segmentByte & "00100000".BinToByte()) == 0) ? Visibility.Hidden : Visibility.Visible;
                rightG.Visibility =((segmentByte & "01000000".BinToByte()) == 0) ? Visibility.Hidden : Visibility.Visible;
            }
        }

    }
}
