using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Stebs.Model
{
    static class Constraints
    {

        public static List<string> LabelConstraints()
        {
            List<string> labelConstraints = new List<string>();

            labelConstraints.Add("AL");
            labelConstraints.Add("BL");
            labelConstraints.Add("CL");
            labelConstraints.Add("DL");
            labelConstraints.Add("SP");
            labelConstraints.Add("SP");

            return labelConstraints;

        }



    }
}
