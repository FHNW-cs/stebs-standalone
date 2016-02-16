using System;
using Stebs.ViewModel;
using System.Collections.Generic;

namespace Stebs.Model
{
    /// <summary>
    /// Represents a Micro-Instruction
    /// </summary>
    public class MPMEntry
    {
        /// <summary>
        /// MPM Address of the instruction
        /// </summary>
        public int Addr {
            get;
            set;
        }
        
        /// <summary>
        /// Next MIP Address
        /// </summary>
        public NA Na {
            get;
            set;
        }
        
        /// <summary>
        /// Enable Value Flag
        /// </summary>
        public bool Ev {
            get;
            set;
        }
        
        /// <summary>
        /// Offset, address or value
        /// </summary>
        public int Val {
            get;
            set;
        }

        /// <summary>
        /// Criteria for jumping
        /// </summary>
        public CRIT Crit {
            get;
            set;
        }
       
        /// <summary>
        /// Clear interrupt flag
        /// </summary>
        public bool Cif {
            get;
            set;
        }

        
        /// <summary>
        /// Affect flags
        /// </summary>
        public bool Af {
            get;
            set;
        }

        /// <summary>
        /// ALU command
        /// </summary>
        public Alu.Cmd Alu {
            get;
            set;
        }

        /// <summary>
        /// Source where the data should come from
        /// </summary>
        public SRCRegister Src
        {
            get;
            set;
        }


        /// <summary>
        /// Destination from where the data should come from
        /// </summary>
        public DESTRegister Dst
        {
            get;
            set;
        }

        /// <summary>
        /// Flag the data control. This flags controls data input source.
        /// There is either IO(=true) or Mem = false
        /// </summary>
        public bool IoM
        {
            set;
            get;
        }
        

        /// <summary>
        /// Control
        /// </summary>
        public bool Rw {
            get;
            set;
        }

        /// <summary>
        /// Enum of the next MIP address
        /// </summary>
        public enum NA
        {      
            FETCH  = 1,
            DECODE = 2,
            NEXT   = 3
        }

        /// <summary>
        /// Enum of the three diffrent busses
        /// </summary>
        public enum BUS
        {
            X    = 4,
            DATA = 1
        }

        /// <summary>
        /// Enum of the possible Criteria
        /// </summary>
        public enum CRIT
        {
            EMPTY = 0,
            NZ    = 1,
            NO    = 2,
            NS    = 3,
            Z     = 4,
            O     = 5,
            S     = 6
        }

        public override string ToString()
        {
            return "MPMEntry:\nAddr: " + Addr + "\nNa: " + Na + "\nEV:" + Ev + "\nALU: " + Alu;
        }

        /// <sumtmary>
        /// Retruns the enum output name or an empty string when value is null
        /// </summary>
        /// <param name="instance">Enum instance</param>
        /// <returns>Enum name</returns>
        public string GetName(Enum instance)
        {
            if (instance.ToString() == "EMPTY" || instance.ToString() == "NOP") return "";
            return instance.ToString();
        }
    }
}
