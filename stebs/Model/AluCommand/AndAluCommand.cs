namespace Stebs.Model.AluCommand
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    class AndAluCommand : IAluCommand
    {
        private byte x;
        private byte y;
        public AndAluCommand(byte x, byte y)
        {
            this.x = x;
            this.y = y;
        }

        public byte Execute()
        {
            return(byte)(x & y);
        }

        public void SetFlags(Processor proc)
        {
            proc.O = false;
        }
    }
}
