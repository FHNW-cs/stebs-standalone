
namespace Stebs.Model
{
    /// <summary>
    /// Model class of a bus
    /// </summary>
    public class Bus : IBus
    {
        /// <summary>
        /// The value which is stored on the bus
        /// </summary>
 
        private byte value;

        /// <summary>
        /// Writes a value to the bus
        /// </summary>
        /// <param name="value">The value to write</param>
        virtual public void Write(byte value) {
            this.value = value;
        }

        /// <summary>
        /// Reads a value from the bus
        /// </summary>
        /// <returns>The value read from the bus</returns>
        virtual public byte Read() {
            return this.value;
        }
    }
}
