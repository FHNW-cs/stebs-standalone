
namespace Stebs.Model
{
    /// <summary>
    /// Interface for the Bus-Classes
    /// </summary>
    interface IBus
    {
        /// <summary>
        /// Writes a value to the bus
        /// </summary>
        /// <param name="value">value to write</param>
        void Write(byte value);

        /// <summary>
        /// Reads a value from the bus
        /// </summary>
        /// <returns>Value read from the bus</returns>
        byte Read();
    }
}
