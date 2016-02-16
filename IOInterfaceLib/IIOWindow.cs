namespace IOInterfaceLib
{
    /// <summary>
    /// Delegate which handles interrupts
    /// </summary>
    public delegate void FireInterruptHandler();

    /// <summary>
    /// Interface for all plugin IO-Devices
    /// </summary>
    public interface IIOWindow
    {
        /// <summary>
        /// Event which is called on interrupts
        /// </summary>
        event FireInterruptHandler FireInterruptEvent;

        /// <summary>
        /// Getter for the interface name
        /// </summary>
        /// <returns>The name of the interface</returns>
        string GetName();

        /// <summary>
        /// Write data to the interface. You can set values in serial order.
        /// </summary>
        /// <param name="input">the value to be set</param>
        void Write(byte input);

        /// <summary>
        /// Read a byte from the interface output
        /// </summary>
        /// <returns>the value which the interface can returns</returns>
        byte Read();
    }
}
