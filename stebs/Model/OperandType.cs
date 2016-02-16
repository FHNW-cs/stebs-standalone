using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Stebs.Model
{
   
    public class OperandType
    {

        private static IList<OperandType> operandTypes = new List<OperandType>();

        public static OperandType CONST = new OperandType("const");
        public static OperandType OFFSET = new OperandType("offset");
        public static OperandType ADDRESS = new OperandType("addr");
        public static OperandType REGISTER = new OperandType("reg");
        public static OperandType INDIRECT_REGISTER = new OperandType("|reg|");
        public static OperandType INDIRECT_ADDRESS = new OperandType("|addr|");
        public static OperandType ABSOLUTE = new OperandType("absolute");

        private OperandType(String name)
        {
            Name = name;
            operandTypes.Add(this);
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj) 
               &&  GetType().Equals(obj.GetType())
               &&  this.Name.Equals(((OperandType)obj).Name);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        public String Name
        {
            get;
            private set;
        }

        public override string ToString()
        {
            return Name;
        }


        public static IList<OperandType> OperandTypes()
        {
            return new List<OperandType>(operandTypes);
        }
    }

    
    
}
