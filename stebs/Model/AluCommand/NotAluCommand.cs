namespace Stebs.Model.AluCommand
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    class NotAluCommand : IAluCommand
    {
        private byte value;
        public NotAluCommand(byte value)
        {
            this.value = value;
        }

        public byte Execute()
        {
            return (byte)~(int)value;
        }

        public void SetFlags(Processor proc)
        {
            proc.O = false;
        }
    }
}
