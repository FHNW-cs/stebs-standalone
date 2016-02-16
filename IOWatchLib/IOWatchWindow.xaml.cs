using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;
using Helper;

namespace IOWatchLib {
    /// <summary>
    /// Interaction logic for IOWatchWndow.xaml
    /// </summary>
    public partial class IOWatchWindow : UserControl, IOInterfaceLib.IIOWindow {
        public event IOInterfaceLib.FireInterruptHandler FireInterruptEvent;

        // Current time
        private DateTime time;

        // State variable used to determine what byte is to be addressed:
        // 2: seconds, 1: minutes, 0: hours
        private int state;

        // Buffered Timer buffer vars
        private byte hours;
        private byte minutes;
        private byte seconds;

        // State variable used to determine what interrupt mode is to select:
        // 0x80: no interrupt, 0x81: interupt every second, 0x82; every 2 seconds, 
        // 0x85: every 5 seconds
        private byte intState;
        private byte intCounter;

        /// <summary>
        /// Timer Thread
        /// </summary>
        private Thread timerThread;


        public IOWatchWindow() {
            InitializeComponent();
            hours = minutes = seconds = 0xFF;
            intState = 0x80;

            timerThread = new Thread(new ThreadStart(delegate {
                while (true) {
                    time = DateTime.Now;
                    ReDraw();

                    switch (intState) {
                        case 0x80:
                            break;
                        case 0x81:
                            if (FireInterruptEvent != null) {
                                FireInterruptEvent();
                            }
                            break;
                        case 0x82:
                            if (FireInterruptEvent != null && intCounter == 0) {
                                FireInterruptEvent();
                                intCounter = 2;
                            }
                            else { --intCounter; }
                            break;
                        case 0x85:
                            if (FireInterruptEvent != null && intCounter == 0) {
                                FireInterruptEvent();
                                intCounter = 5;
                            }
                            else { --intCounter; }
                            break;
                    }
                    Thread.Sleep(1000);
                }
            }));
            timerThread.IsBackground = true;
            timerThread.Start();

            ReDraw();
        }

        /// <summary>
        /// Get the name to set the window title.
        /// </summary>
        /// <returns></returns>
        public string GetName() {
            return "IO Watch";
        }

        /// <summary>
        /// Set a timer state.
        /// 0xFF: Store current time into timer buffer.
        /// 0x80: No interrupt
        /// 0x81: Interrupt every 1 sec
        /// 0x82: Interrupt every 2 sec
        /// 0x85: Interrupt every 5 sec
        /// 0x02: Answer seconds from buffer when port is read.
        /// 0x01: Answer minutes from buffer when port is read.
        /// 0x00: Answer hours from buffer when port is read.
        /// else: Answer an error code.
        /// </summary>
        /// <param name="input"></param>
        public void Write(byte input) {
            switch (input) {
                case 0x80:
                case 0x81:
                case 0x82:
                case 0x85:
                    intState = input;
                    // Strip off MSB of the counter value
                    intCounter = (byte)(input - 0x80);
                    break;
                case 0xFF:
                    DateTime timeBuffer = DateTime.Now;
                    seconds = ByteToBCD((byte)timeBuffer.Second);
                    minutes = ByteToBCD((byte)timeBuffer.Minute);
                    hours = ByteToBCD((byte)timeBuffer.Hour);
                    break;
                default:
                    state = input;
                    break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hex"></param>
        /// <returns></returns>
        private byte ByteToBCD(byte num) {
            return (byte)((num / 10) * 0x10 + (num % 10));
        }

        /// <summary>
        /// Read a timer byte. According to the state set by Write() answer
        /// seconds, minutes, hours or an error code 0xFF.
        /// </summary>
        /// <returns></returns>
        public byte Read() {
            switch (state) {
                case 2:
                    return seconds;
                case 1:
                    return minutes;
                case 0:
                    return hours;
                default:
                    return 0xFF;
            }
        }

        /// <summary>
        /// Redraw the current time to the XAML.
        /// </summary>
        private void ReDraw() {
            this.InvokeIfRequired(() => {
                byte currSeconds = (byte)time.Second;
                byte currMinutes = (byte)time.Minute;
                byte currHours = (byte)time.Hour;
                String minuteString = Convert.ToString(currMinutes).PadLeft(2, '0');
                String secondString = Convert.ToString(currSeconds).PadLeft(2, '0');
                timeLabel.Content = currHours.ToString() + ":" + minuteString + ":" + secondString;
            });
        }

    }
}