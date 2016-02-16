namespace Stebs.Model.AluCommand
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    class ModAluCommand : IAluCommand
    {
        private byte x;
        private byte y;
        public ModAluCommand(byte x, byte y)
        {
            this.x = x;
            this.y = y;
        }

        public byte Execute()
        {
            if (y == 0)
            {
                throw new DivideByZeroException();
            }
            return (byte)(x % y);
        }

        public void SetFlags(Processor proc)
        {
            proc.O = false;
        }
    }
}
