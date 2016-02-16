
namespace Stebs.Model
{
    /// <summary>
    /// Model class of the Data Bus
    /// </summary>
    public class DataBus : IBus
    {
        /// <summary>
        /// The RAM model class
        /// </summary>
        private RAM ram = new RAM();

        /// <summary>
        /// Property for accessing the RAM-Data
        /// </summary>
        public byte[] RamData {
            get {
                return ram.Data;
            }
            set {
                ram.Data = value;
            }
        }

        /// <summary>
        /// Reference to the Processor Model
        /// </summary>
        private Processor proc;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="proc">The Processor Model</param>
        public DataBus(Processor proc) {
            this.proc = proc;
        }

        /// <summary>
        /// Reads a value from the RAM on the address proc.MAR
        /// </summary>
        /// <returns>The value read from the RAM</returns>
        public byte Read() {
            return ram.Read(proc.MAR);
        }

        /// <summary>
        /// Writes a value to the RAM on hte address proc.MAR
        /// </summary>
        /// <param name="value">The value to write to the RAM</param>
        public void Write(byte value) {
            ram.Write(proc.MAR, value);
        }

    }
}
