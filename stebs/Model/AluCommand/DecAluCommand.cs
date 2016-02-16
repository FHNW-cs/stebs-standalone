namespace Stebs.Model.AluCommand
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    class DecAluCommand : IAluCommand
    {
        private byte value;
        private byte res;
        public DecAluCommand(byte value)
        {
            this.value = value;
        }

        public byte Execute()
        {
            res =  (byte)(value - 1);
            return res;
        }

        public void SetFlags(Processor proc)
        {
            proc.O = ((value & Alu.MSB) == Alu.MSB && (res & Alu.MSB) != Alu.MSB);
        }
    }
}
