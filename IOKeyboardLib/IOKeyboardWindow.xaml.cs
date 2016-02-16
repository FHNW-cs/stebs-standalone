using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace IOKeyboardLib
{
    /// <summary>
    /// Interaction logic for IOKeyboardLib.xaml
    /// </summary>
    public partial class IOKeyboardWindow : UserControl, IOInterfaceLib.IIOWindow
    {
        private byte keyboardByte = 0;

        public IOKeyboardWindow()
        {
            InitializeComponent();
        }

        public event IOInterfaceLib.FireInterruptHandler FireInterruptEvent;

        public string GetName()
        {
            return "IO Keyboard";
        }

        public byte Read()
        {
            return keyboardByte;
        }

        public void Write(byte input)
        {
            throw new NotImplementedException();
        }

        private void BtnEnterClick(object sender, RoutedEventArgs e)
        {
            keyboardByte = 0x0D;
            FireInterruptEvent();
        }

        private void BtnSpaceClick(object sender, RoutedEventArgs e)
        {
            keyboardByte = 0x20;
            FireInterruptEvent();
        }

        private void BtnBackspaceClick(object sender, RoutedEventArgs e)
        {
            keyboardByte = 0x08;
            FireInterruptEvent();
        }

        private void BtnCharClick(object sender, RoutedEventArgs e)
        {
            Button b = (Button)sender;
            keyboardByte = (byte)b.Content.ToString()[0];
            FireInterruptEvent();
        }

    }
}
