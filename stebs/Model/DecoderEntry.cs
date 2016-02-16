using Stebs.ViewModel;

namespace Stebs.Model
{
    /// <summary>
    /// Represents a assembler-command
    /// </summary>
    public class DecoderEntry
    {
        /// <summary>
        /// The Opcode
        /// </summary>
        public byte OpCode {
            get;
            set;
        }
        
        /// <summary>
        /// The Start MPM(Micro Program Memory) Address of this assembler-command
        /// </summary>
        public int MPMAddress {
            get;
            set;
        }
        
        /// <summary>
        /// The type of this assembler-command
        /// </summary>
        public InstructionType InstructionType {
            get;
            set;
        }
    }
}
