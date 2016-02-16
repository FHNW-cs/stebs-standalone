using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Linq;

namespace Stebs.Model
{
    public class OperandTypeFactory
    {
        private OperandTypeFactory()
        {
            // hidden constructor;
        }

        public static OperandType GetTypeForName(String type)
        {
            type = type.Trim();
            type = type.ToLower();
            var query = from opType in  OperandType.OperandTypes()
                        where
                        opType.Name.Equals(type)
                        select opType;

            if (query.Count() == 0)
            {
                throw new ArgumentException("Unknown OperandType");
            }

            return query.First();
            
        }

        
    }
}
