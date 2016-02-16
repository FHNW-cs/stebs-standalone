namespace IOHeaterLib
{
    using Helper;
    using System.Threading;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Interaction logic for OutputHeaterWindow.xaml
    /// </summary>
    public partial class IOHeaterWindow : UserControl, IOInterfaceLib.IIOWindow
    {
        public event IOInterfaceLib.FireInterruptHandler FireInterruptEvent;

        /// <summary>
        /// The Byte Value
        /// </summary>
        private byte heaterByte = "00010101".BinToByte();

        /// <summary>
        /// Current temprature
        /// </summary>
        private float currentTemp = 15.0f;
        
        /// <summary>
        /// Heater Thread
        /// </summary>
        private Thread heatingThread;

        /// <summary>
        /// Creats a new instance of IOHeaderWindow class
        /// </summary>
        public IOHeaterWindow()
        {
            InitializeComponent();

            heatingThread = new Thread(new ThreadStart(delegate
            {
                while (true)
                {
                    if (isHeating())
                    {
                        if (currentTemp < 50)
                        {
                            currentTemp += 0.2f;
                        }
                    }
                    else
                    {
                        if (currentTemp > -9)
                        {
                            currentTemp -= 0.1f;
                        }
                    }

                    setThermostat();

                    ReDraw();
                    Thread.Sleep(500);
                }
            }));
            heatingThread.IsBackground = true;
            heatingThread.Start();

            ReDraw();
        }

        /// <summary>
        /// The IO-Device Name
        /// </summary>
        /// <returns>The name as string</returns>
        public string GetName() {
            return "IO Heater";
        }

        /// <summary>
        /// Returns the current byte value
        /// </summary>
        /// <returns>the byte value</returns>
        public byte Read() {
            setThermostat();
            return heaterByte;
        }

        /// <summary>
        /// Sets the current byte value
        /// </summary>
        /// <param name="input">new byte value</param>
        public void Write(byte input) {
            heaterByte = input;
            ReDraw();
        }

        /// <summary>
        /// redraws the output in the XAML
        /// </summary>
        private void ReDraw() {
            this.InvokeIfRequired(() => {
                labelHeaterByte.Content = heaterByte.ToBinString();
                labelHeaterHex.Content = heaterByte.ToHexString();
                labelHeaterTwoComplement.Content = heaterByte.ToTwoComplementString();

                labelTargetTemp.Content = string.Format("{0:0.0}°C", GetTargetTemp());
                labelCurrentTemp.Content = string.Format("{0:0.0}°C", currentTemp);

                imageIsHeating.Visibility = isHeating() ? Visibility.Visible : Visibility.Hidden;
            });

        }

        /// <summary>
        /// Is the heater on or off?
        /// </summary>
        /// <returns>heater on or off</returns>
        private bool isHeating() {
            return (((heaterByte & "10000000".BinToByte()) != 0) ? true : false);
        }

        /// <summary>
        /// Returns the target temprature
        /// </summary>
        /// <returns>target temprature</returns>
        private int GetTargetTemp() {
            return heaterByte & "00111111".BinToByte();
        }

        /// <summary>
        /// Thermostat on or off
        /// </summary>
        private void setThermostat()
        {
            if (this.currentTemp >= this.GetTargetTemp())
            {
                heaterByte = (byte)(heaterByte | "01000000".BinToByte()); // SET
            }
            else
            {
                this.heaterByte = (byte)(this.heaterByte & "10111111".BinToByte()); // CLEAR
            }
        }

        /// <summary>
        /// Resets the current temprature
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnReset_Click(object sender, RoutedEventArgs e) {
            this.currentTemp = 15.0f;
            this.ReDraw();
        }

    }
}