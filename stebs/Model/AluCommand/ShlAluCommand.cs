namespace Stebs.Model.AluCommand
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    class ShlAluCommand : IAluCommand
    {
        private byte value;
        public ShlAluCommand(byte value)
        {
            this.value = value;
        }

        public byte Execute() 
        {
            return (byte)(value << 1);
        }

        public void SetFlags(Processor proc)
        {
            proc.O = false;
        }
    }
}
