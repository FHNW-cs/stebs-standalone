namespace Stebs.Model
{
    using System;
    using Stebs.Model.AluCommand;

    /// <summary>
    /// Model class for the ALU(arithmetic logic unit).
    /// </summary>
    /// The ALU is the calculating part of a CPU. It can execute mathematical 
    /// operations such adding two values or increment one value. It has
    /// two registers (X and Y) which are required to execute binary or
    /// unairy commands. 
    /// 
    /// Depending on the executed command, both registers
    /// 
    public class Alu
    {
        /// <summary>
        /// 
        /// </summary>
        public byte X { get; set; }
        public byte Y { get; set; }
        public byte Res {get; set; }
        
        /// <summary>
        /// The most significant bit
        /// </summary>
        public const byte MSB = 128;

        /// <summary>
        /// Reference to the processor
        /// </summary>
        private Processor proc;

        /// <summary>
        /// Initializes a new instance of the <see cref="Alu"/> class
        /// </summary>
        /// <param name="proc">the model class of the connected processor</param>
        public Alu(Processor proc)
        {
            this.proc = proc;
        }
        
        /// <summary>
        /// The possible ALU Operations/Commands
        /// </summary>
        public enum Cmd
        {
            /// <summary>
            /// No Operation
            /// </summary>
            NOP = 0,

            /// <summary>
            /// Add two values
            /// </summary>
            ADD = 1,

            /// <summary>
            /// Subtract the second from the first value.
            /// </summary>
            SUB = 2,

            /// <summary>
            /// Multiply two values.
            /// </summary>
            MUL = 3,

            /// <summary>
            /// Divide the first from the second value.
            /// </summary>
            DIV = 4,

            /// <summary>
            /// Calculate the module of the two values.
            /// </summary>
            MOD = 5,

            /// <summary>
            /// Decrement a value.
            /// </summary>
            DEC = 6,

            /// <summary>
            /// Increment a value.
            /// </summary>
            INC = 7,

            /// <summary>
            /// Merge the binary values (or the values).
            /// </summary>
            OR  = 8,

            /// <summary>
            /// Calculate the differences (XOR the values).
            /// </summary>
            XOR = 9,

            /// <summary>
            /// Calculate the inverse of a value.
            /// </summary>
            NOT = 10,

            /// <summary>
            /// Calculate the common bits (and the values)
            /// </summary>
            AND = 11,

            /// <summary>
            /// Right shift a value.
            /// </summary>
            SHR = 12,

            /// <summary>
            /// Left shift values.
            /// </summary>
            SHL = 13,

            /// <summary>
            /// Right shift values and append the Alu.MSB on the left.
            /// </summary>
            ROR = 14,

            /// <summary>
            /// Left shift values and append the LSB on the right.
            /// </summary>
            ROL = 15
        }

        /// <summary>
        /// Executes an operation on the ALU
        /// Reads the input value from the X- and Y-Bus on the processor
        /// </summary>
        /// <param name="op">The operation to execute</param>
        /// <param name="affectFlags">If true, affects the status-register-flags</param>
        public void Execute(Cmd op, bool affectFlags)
        {
            IAluCommand aluCommand = GetAluCommand(op);
            if (aluCommand != null)
            {
                Res = aluCommand.Execute();
                if (affectFlags)
                {
                    SetFlags(aluCommand);
                }
            }
        }

        /// <summary>
        /// Set the flags in the status register of the processor
        /// </summary>
        /// <param name="op">Operation which was executed</param>
        private void SetFlags(IAluCommand aluCommand)
        {
            
            // Zero Flag
            proc.Z = Res == 0;

            // Sign Flag
            proc.S = (Res & MSB) == MSB;

            if (aluCommand != null )
            {
                aluCommand.SetFlags(proc);
            }
            else
            {
                proc.O = false;
            }
        }

        private IAluCommand GetAluCommand(Cmd op)
        {
            switch (op)
            {
                case Cmd.ADD:
                    return new AddAluCommand(X, Y);
                case Cmd.SUB:
                    return new SubAluCommand(X, Y);
                case Cmd.MUL:
                    return new MulAluCommand(X, Y);
                case Cmd.DIV:
                    return new DivAluCommand(X, Y);
                case Cmd.MOD:
                    return new ModAluCommand(X, Y);
                case Cmd.DEC:
                    return new DecAluCommand(X);
                case Cmd.INC:
                    return new IncAluCommand(X);
                case Cmd.OR:
                    return new OrAluCommand(X, Y);
                case Cmd.XOR:
                    return new XOrAluCommand(X, Y);
                case Cmd.NOT:
                    return new NotAluCommand(X);
                case Cmd.AND:
                    return new AndAluCommand(X, Y);
                case Cmd.ROL:
                    return new RolAluCommand(X);
                case Cmd.ROR:
                    return new RorAluCommand(X);
                case Cmd.SHL:
                    return new ShlAluCommand(X);
                case Cmd.SHR:
                    return new ShrAluCommand(X);
            }
            return null;
        }
    }
}
