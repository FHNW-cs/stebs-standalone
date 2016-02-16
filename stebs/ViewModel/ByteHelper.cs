namespace Stebs.ViewModel
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Windows.Threading;
    using System.Text.RegularExpressions;

    /// <summary>
    /// static class which contains several extension methods
    /// Mainly for helping converting string to Hex-Numbers.
    /// </summary>
    public static class ByteHelper
    {


        // =====================================================================
        // BIN Functions
        // =====================================================================
        #region
        /// <summary>
        /// Converts a binary input string to a decimal byte
        /// </summary>
        /// <param name="input">8-Bit Binary string.</param>
        /// <returns>Value of the binary string as a decimal number as a byte</returns>
        public static byte BinToByte(this string input) {
            return Convert.ToByte(input, 2);
        }

        /// <summary>
        /// Converts a decimal byte input to a binary string 
        /// </summary>
        /// <param name="input">Decimal input number</param>
        /// <param name="spaceAfterFour">If set, inserts a ' after 4 bits</param>
        /// <returns>A binary string which represents the input value</returns>
        public static string ToBinString(this byte input, bool spaceAfterFour = false) {
            string re;
            re = Convert.ToString(input, 2);
            re = re.PadLeft(8, '0');

            if (spaceAfterFour) {
                re = re.Insert(4, "'");
            }

            return re;
        }

        /// <summary>
        /// Converts a decimal number to a string containing the number as a Two's Complement
        /// </summary>
        /// <param name="input">Decimal number</param>
        /// <returns>Input value as two's complement string</returns>
        public static string ToTwoComplementString(this byte input) {
            string re, binary;
            byte number;

            if (input == 0)
                return "+000";

            binary = Convert.ToString(input, 2).PadLeft(8, '0');

            if ((input & 128) == 0) {
                // Positiv
                re = "+";
                number = Convert.ToByte(binary.Substring(1, 7), 2);
                re += Convert.ToString(number, 10).PadLeft(3, '0');
            } else {
                // Negativ
                re = "-";

                number = Convert.ToByte(binary, 2);
                number -= 1;
                number = number.Not();
                re += Convert.ToString(number, 10).PadLeft(3, '0');
            }

            return re;
        }
        #endregion

        // =====================================================================
        // HEX Funtions
        // =====================================================================
        #region
        /// <summary>
        /// Converts a Hex-Input string to a decimal byte value
        /// </summary>
        /// <param name="input">Hex-Value as a string</param>
        /// <returns>Decimal byte value</returns>
        public static byte HexToByte(this string input) {
            return Convert.ToByte(input, 16);
        }

        /// <summary>
        /// Converts a Hex-Input string to a Decimal integer value
        /// </summary>
        /// <param name="input">Hex-Value as a string</param>
        /// <returns>Decimal integer value</returns>
        public static int HexToInt(this string input) {
            return Convert.ToInt32(input, 16);
        }

        /// <summary>
        /// Converts a decimal byte input to a Hex-string 
        /// </summary>
        /// <param name="input">Decimal input</param>
        /// <param name="digits">Number of digits in the output string</param>
        /// <returns>Hex-String</returns>
        public static string ToHexString(this byte input, int digits = 2) {
            string re;
            re = Convert.ToString(input, 16).ToUpper();
            re = re.PadLeft(digits, '0');
            return re;
        }

        /// <summary>
        /// Converts a decimal integer input to a Hex-string 
        /// </summary>
        /// <param name="input">Decimal input</param>
        /// <param name="digits">Number of digits in the output string</param>
        /// <returns>Hex-String</returns>
        public static string ToHexString(this int input, int digits) {
            string re;
            re = Convert.ToString(input, 16).ToUpper();
            re = re.PadLeft(digits, '0');
            return re;
        }
        #endregion

        // =====================================================================
        // Other functions
        // =====================================================================
        #region
        /// <summary>
        /// Inverts the bits of the input value
        /// </summary>
        /// <param name="input">Input value as byte</param>
        /// <returns>Inverted input value</returns>
        public static byte Not(this byte input) {
            StringBuilder binary = new StringBuilder();

            binary.Append(Convert.ToString(input, 2).PadLeft(8, '0'));

            for (int i = 0; i < 8; i++) {
                if (binary[i] == '0') {
                    binary[i] = '1';
                } else {
                    binary[i] = '0';
                }
            }

            return Convert.ToByte(binary.ToString(), 2);
        }

        /// <summary>
        /// Checks if the content in the input string is an Int
        /// </summary>
        /// <param name="input">Input string</param>
        /// <returns>True if the content in the input-string is an Int</returns>
        public static bool IsInt(this string input) {
            int iOut;
            return int.TryParse(input, out iOut);
        }

        private static Regex checkIsByteHexRegex = new Regex(@"^\s*[0-9a-fA-F]{1,2}\s*$");

        /// <summary>
        /// Checks if the content in the input string is a HEX-Value
        /// and has a maximum size of one byte (meaning two hex nibbles)
        /// </summary>
        /// <param name="input">Input string</param>
        /// <returns>True, if the Content in the input string is a HEX-Value</returns>
        public static bool IsHexValue(this string input) {
            return checkIsByteHexRegex.IsMatch(input);
        }
        /// <summary>
        /// Compares two arrays
        /// </summary>
        /// <typeparam name="T">Type of the array</typeparam>
        /// <param name="input">first array</param>
        /// <param name="p2">second array</param>
        /// <returns>True if the two arrays are equal</returns>
        public static bool ArraysEqual<T>(this T[] input, T[] p2) {
            if (ReferenceEquals(input, p2))
                return true;

            if (input == null || p2 == null)
                return false;

            if (input.Length != p2.Length)
                return false;

            EqualityComparer<T> comparer = EqualityComparer<T>.Default;
            for (int i = 0; i < input.Length; i++) {
                if (!comparer.Equals(input[i], p2[i])) return false;
            }
            return true;
        }

        public const int X8000 = 32768;
        public const int X4000 = 16384;
        public const int X2000 =  8192;
        public const int X1000 =  4096;
        public const int X0800 =  2048;
        public const int X0400 =  1024;
        public const int X0200 =   512;
        public const int X0100 =   256;
        public const int X0080 =   128;
        public const int X0040 =    64;
        public const int X0020 =    32;
        public const int X0010 =    16;
        public const int X0008 =     8;
        public const int X0004 =     4;
        public const int X0002 =     2;
        public const int X0001 =     1;

        /// <summary>
        /// Checks if the input string represents a Register
        /// </summary>
        /// <param name="input">Input string</param>
        /// <returns>True if the input string represents a register</returns>
        public static bool IsRegister(this string input) {
            if (input.Trim().ToUpper() == "AL" ||
                input.Trim().ToUpper() == "BL" ||
                input.Trim().ToUpper() == "CL" ||
                input.Trim().ToUpper() == "DL" ||
                input.Trim().ToUpper() == "SP") {

                return true;
            } else {
                return false;
            }
        }

        /// <summary>
        /// Cheacks if the input string-arrays, contains a given input-string
        /// </summary>
        /// <param name="array">array to check</param>
        /// <param name="input">searched string in the array</param>
        /// <returns>True, if the input-array contains the input-string</returns>
        public static bool Contains(this string[] array, string input) {
            foreach(string s in array) {
                if (s.Equals(input))
                    return true;
            }
            return false;
        }
        /// <summary>
        /// Checks if the current thread has access to the control
        /// If not, invoke the operation on the Dispatcher of this control
        /// Else, execute the operation normally
        /// </summary>
        /// <param name="control">The control, on which an operation should be executed</param>
        /// <param name="operation">The operation to execute</param>
        public static void InvokeIfRequired(this DispatcherObject control, Action operation) {
            if (control.Dispatcher.CheckAccess()) {
                operation();
            } else {
                control.Dispatcher.Invoke(DispatcherPriority.Normal, operation);
            }
        }
        #endregion
    }
}
