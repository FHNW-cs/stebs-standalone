using System;
using System.Windows.Data;
using System.Globalization;
using System.Windows;

namespace Stebs.ViewModel
{
    /// <summary>
    /// A converter for the registers window
    /// </summary>
    [ValueConversion(typeof(byte), typeof(String))]
    public class RegistersViewConverter : IValueConverter
    {
        /// <summary>
        /// Converts the value to a string with three different number systems
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            return Convert(value);
            /*
            byte data =(byte)value;
            return data.ToBinString(true) + "   " + data.ToTwoComplementString() + "   " + data.ToHexString();
             */
        }

        /// <summary>
        /// Not used
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            return DependencyProperty.UnsetValue;
        }

        public static string Convert(object value)
        {
            byte data = (byte)value;
            return data.ToBinString(true) + "   " + data.ToTwoComplementString() + "   " + data.ToHexString();
        }
    }
}
