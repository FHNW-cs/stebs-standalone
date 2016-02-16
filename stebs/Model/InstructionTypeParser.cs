using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Stebs.Model
{
    public class InstructionTypeParser
    {
        /// <summary>
        /// Hidden constructor
        /// </summary>
        private InstructionTypeParser() {}

        /// <summary>
        /// Parses one line of an Instruction type defined in 
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public static InstructionType Parse(String line) {
            line = line.Trim();
            String[] tokens = line.Split(new char[] { ',', ' ' });

            InstructionType instrType = new InstructionType(tokens[0]);
            instrType.SetOperandTypes(ParseParameters(tokens));
            return instrType;
        }

        private static IEnumerable<OperandType> ParseParameters(String[] tokens)
        {
            List<OperandType> list = new List<OperandType>();
            for (int i = 1; i < tokens.Count(); ++i)
            {
                list.Add(OperandTypeFactory.GetTypeForName(tokens[i]));
            }

            return list;
        }

    }
}
