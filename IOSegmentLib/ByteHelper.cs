namespace Helper
{
    using System;
    using System.Text;

    /// <summary>
    /// static class which contains several extension methods
    /// Mainly for helping converting string to Hex-Numbers.
    /// </summary>
    public static class ByteHelper
    {
        /// <summary>
        /// Converts a binary input string to a decimal byte
        /// </summary>
        /// <param name="input">8-Bit Binary string.</param>
        /// <returns>Value of the binary string as a decimal number as a byte</returns>
        public static byte BinToByte(this string input)
        {
            return Convert.ToByte(input, 2);
        }

        /// <summary>
        /// Converts a decimal byte input to a binary string 
        /// </summary>
        /// <param name="input">Decimal input number</param>
        /// <param name="spaceAfterFour">If set, inserts a ' after 4 bits</param>
        /// <returns>A binary string which represents the input value</returns>
        public static string ToBinString(this byte input, bool spaceAfterFour = false)
        {
            string re;
            re = Convert.ToString(input, 2);
            re = re.PadLeft(8, '0');

            if (spaceAfterFour)
            {
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
            {
                return "+000";
            }

            binary = Convert.ToString(input, 2).PadLeft(8, '0');

            if ((input & 128) == 0)
            {
                // Positiv
                re = "+";
                number = Convert.ToByte(binary.Substring(1, 7), 2);
                re += Convert.ToString(number, 10).PadLeft(3, '0');
            }
            else
            {
                // Negativ
                re = "-";

                number = Convert.ToByte(binary, 2);
                number -= 1;
                number = number.Not();
                re += Convert.ToString(number, 10).PadLeft(3, '0');
            }

            return re;
        }

        /// <summary>
        /// Converts a Hex-Input string to a decimal byte value
        /// </summary>
        /// <param name="input">Hex-Value as a string</param>
        /// <returns>Decimal byte value</returns>
        public static byte HexToByte(this string input)
        {
            return Convert.ToByte(input, 16);
        }

        /// <summary>
        /// Converts a Hex-Input string to a Decimal integer value
        /// </summary>
        /// <param name="input">Hex-Value as a string</param>
        /// <returns>Decimal integer value</returns>
        public static int HexToInt(this string input)
        {
            return Convert.ToInt32(input, 16);
        }

        /// <summary>
        /// Converts a decimal byte input to a Hex-string 
        /// </summary>
        /// <param name="input">Decimal input</param>
        /// <param name="digits">Number of digits in the output string</param>
        /// <returns>Hex-String</returns>
        public static string ToHexString(this byte input, int digits = 2)
        {
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
        public static string ToHexString(this int input, int digits)
        {
            string re;
            re = Convert.ToString(input, 16).ToUpper();
            re = re.PadLeft(digits, '0');
            return re;
        }

        /// <summary>
        /// Inverts the bits of the input value
        /// </summary>
        /// <param name="input">Input value as byte</param>
        /// <returns>Inverted input value</returns>
        public static byte Not(this byte input)
        {
            StringBuilder binary = new StringBuilder();

            binary.Append(Convert.ToString(input, 2).PadLeft(8, '0'));

            for (int i = 0; i < 8; i++)
            {
                if (binary[i] == '0')
                {
                    binary[i] = '1';
                }
                else
                {
                    binary[i] = '0';
                }
            }

            return Convert.ToByte(binary.ToString(), 2);
        }
    }
}
