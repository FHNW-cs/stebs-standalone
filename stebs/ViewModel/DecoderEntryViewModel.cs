namespace Stebs.ViewModel
{
    using Stebs.Model;

    public class DecoderEntryViewModel : ViewModelBase
    {
        public DecoderEntryViewModel(DecoderEntry model)
        {
           Model = model;
        }

        public DecoderEntry Model
        {
            get;
            private set;
        }

        /// <summary>
        /// The Opcode
        /// </summary>
        public string OpCode
        {
            get  { return Model.OpCode.ToHexString(); }
        }

        /// <summary>
        /// The MPMAddress as a HEX-String with 3 digits
        /// </summary>
        public string MPMAddress
        {
            get
            {
                return Model.MPMAddress.ToHexString(3);
            }
        }


        /// <summary>
        /// The type of this assembler-command
        /// </summary>
        public string InstructionType
        {
            get
            {
                return Model.InstructionType.Name;
            }
        }
    }
}
