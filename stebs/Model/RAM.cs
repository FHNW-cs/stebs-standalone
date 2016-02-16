using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Stebs.Model
{
    /// <summary>
    /// The RAM class in the Model
    /// </summary>
    public class RAM
    {
        /// <summary>
        /// The Size of the RAM
        /// </summary>
        public static int RAM_SIZE = 256;

        private byte[] data = new byte[RAM_SIZE];
        /// <summary>
        /// The Byte-Array of the RAM
        /// </summary>
        public byte[] Data {
            get {
                return data;
            }
            set {
                data = value;
            }
        }

        /// <summary>
        /// Reads a value from the RAM on the position MAR
        /// </summary>
        /// <param name="MAR">The position on the RAM to read from</param>
        /// <returns>The value read from the RAM</returns>
        public byte Read(byte MAR) {
            return data[MAR];
        }

        /// <summary>
        /// Writes a value to the RAM on the positon MAR
        /// </summary>
        /// <param name="MAR">The position to write to the RAM</param>
        /// <param name="value"></param>
        public void Write(byte MAR, byte value) {
            data[MAR] = value;
        }

        /// <summary>
        /// Reset the RAM Data to 0
        /// </summary>
        public void ResetRAM() {
            for (int i = 0; i < RAM_SIZE; i++) {
                data[i] = 0;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public RAM() {
            // Initialize the RAM Data
            ResetRAM();
        }
    }
}
