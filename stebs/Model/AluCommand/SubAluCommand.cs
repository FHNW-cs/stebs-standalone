namespace Stebs.Model.AluCommand
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    class SubAluCommand : IAluCommand
    {
        private byte x;
        private byte y;
        private byte res;
        public SubAluCommand(byte x, byte y)
        {
            this.x = x;
            this.y = y;
        }

        public byte Execute()
        {
            res = (byte)(x - y);
            return res;
        }

        public void SetFlags(Processor proc)
        {
            proc.O = ((x & Alu.MSB) == Alu.MSB && (y & Alu.MSB) != Alu.MSB && (res & Alu.MSB) != Alu.MSB) ||
                             ((x & Alu.MSB) != Alu.MSB && (y & Alu.MSB) == Alu.MSB && (res & Alu.MSB) == Alu.MSB);
        }
    }
}
