using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Stebs.Model
{
    public class InstructionType
    {
        private IList<OperandType> operandTypes;

        public InstructionType(String mnemonic) {
            this.Mnemonic = mnemonic;
        }

        public String Mnemonic
        {
            get;
            private set;
        }

        public void AddOperandType(String type) {
            operandTypes. Add(OperandTypeFactory.GetTypeForName(type));
        }

        public void SetOperandTypes(IEnumerable<OperandType> operands)
        {
            operandTypes = new List<OperandType>(operands);
        }

        public int GetOperandCount()
        {
            return operandTypes.Count();
        }

        public OperandType GetOperandType(int pos)
        {
            return operandTypes[pos];
        }

        public String Name
        {
            get {
                StringBuilder builder = new StringBuilder();
                builder.Append(Mnemonic);
                builder.Append(" ");
                foreach(OperandType t in operandTypes) {
                    builder.Append(t);
                    builder.Append(",");
                }
                builder.Remove(builder.Length - 1, 1);

                return builder.ToString();
            }
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj)
            && this.GetType().Equals(obj.GetType())
            && operandTypes.Equals(((InstructionType)obj).operandTypes);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode() * 13
                + operandTypes.GetHashCode() * 17;
        }

    }
}
