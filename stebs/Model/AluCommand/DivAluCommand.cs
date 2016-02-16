namespace Stebs.Model.AluCommand
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using NLog;

    class DivAluCommand : IAluCommand
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private byte x;
        private byte y;
        public DivAluCommand(byte x, byte y)
        {
            this.x = x;
            this.y = y;
        }

        public byte Execute()
        {
            if (y == 0)
            {
                logger.Error("Division by zero");
                throw new DivideByZeroException();
            }

            return (byte)(x / y);
        }

        public void SetFlags(Processor proc)
        {
            proc.O = false;
        }
    }
}
