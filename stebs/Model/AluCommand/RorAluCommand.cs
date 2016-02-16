namespace Stebs.Model.AluCommand
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    class RorAluCommand : IAluCommand
    {
        private byte value;
        public RorAluCommand(byte value)
        {
            this.value = value;
        }

        public byte Execute()
        {
            return (byte)((value >> 1) | (value << (8 - 1)));
        }

        public void SetFlags(Processor proc)
        {
            proc.O = false;
        }
    }
}
