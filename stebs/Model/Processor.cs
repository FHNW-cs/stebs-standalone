using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;

namespace Stebs.Model
{
    /// <summary>
    /// Model class for the Processor
    /// </summary>
    public class Processor
    {
        public List<DecoderEntry> DecoderEntries
        {
            get;
            private set;
        }

        /// <summary>
        /// Dictionary of the MPM Entries with the MPM-Address as key
        /// </summary>
        public Dictionary<int, MPMEntry> mpmEntries = new Dictionary<int, MPMEntry>();


        public enum REGISTER
        {
            AL = 0,
            BL = 1,
            CL = 2,
            DL = 3,
            SP = 4,
            SEL = 5,
            IP = 6,
            MBR = 7,
            MAR = 9,
            MDR = 10,
            IR = 11,
            RES = 12,
            S = 13,
            O = 14,
            Z = 15,
            I = 16
        }

        // =====================================================================
        // Properties
        // =====================================================================
        #region
        /// <summary>
        /// The SEL-Register
        /// contains a value for selecting one of the general purpose register(AL,BL,CL,DL,SP)
        /// </summary>
        public byte SEL {
            get;
            set;
        }

        /// <summary>
        /// The general purpose register AL
        /// </summary>
        public byte AL {
            get;
            set;
        }

        /// <summary>
        /// The general purpose register BL
        /// </summary>
        public byte BL {
            get;
            set;
        }

        /// <summary>
        /// The general purpose register CL
        /// </summary>
        public byte CL {
            get;
            set;
        }

        /// <summary>
        /// The general purpose register DL
        /// </summary>
        public byte DL {
            get;
            set;
        }

        /// <summary>
        /// The Stack Pointer
        /// </summary>
        public byte SP {
            get;
            set;
        }

        /// <summary>
        /// The instruction pointer
        /// </summary>
        public byte IP {
            get;
            set;
        }
            
        /// <summary>
        /// Memory Buffer Register
        /// </summary>
        public byte MBR {
            get;
            set;
        }

        /// <summary>
        /// Memory Address Register
        /// </summary>
        public byte MAR {
            get;
            set;
        }

        /// <summary>
        /// Memory Data Register
        /// </summary>
        public byte MDR {
            get;
            set;
        }

        /// <summary>
        /// Sign Flag
        /// </summary>
        public bool S {
            get;
            set;
        }

        /// <summary>
        /// Overflow Flag
        /// </summary>
        public bool O {
            get;
            set;
        }

        /// <summary>
        /// Zero Flag
        /// </summary>
        public bool Z {
            get;
            set;
        }

        /// <summary>
        /// Enable/Disable Interrupt flag
        /// </summary>
        public bool I {
            get;
            set;
        }

        /// <summary>
        /// Interrupt Flag
        /// Is set if there is a pending interrupt
        /// </summary>
        public bool IRF {
            get;
            set;
        }
        
        /// <summary>
        /// Status register, combines the flags I,O,S and Z
        /// </summary>
        public byte SR {
            get {
                return(byte)((I ? 16 : 0) |(S ? 8 : 0) |(O ? 4 : 0) |(Z ? 2 : 0));
            }
            set {
                I =(value & 16) == 16;
                S =(value & 8) == 8;
                O =(value & 4) == 4;
                Z =(value & 2) == 2;
            }
        }
        
        /// <summary>
        /// Micro Instruction Pointer
        /// Contains the MPM(Micro Program Memory) Address of the current micro instruction
        /// </summary>
        public int MIP {
            get;
            set;
        }

        /// <summary>
        /// Instruction Register
        /// Contains the Opcode of the current Assembler Command
        /// </summary>
        public byte IR {
            get;
            set;
        }

        /// <summary>
        /// The Data-Bus
        /// </summary>
        public Bus Data {
            get;
            private set;
        }

        /// <summary>
        /// The DATA-Bus which contains the RAM
        /// </summary>
        public DataBus ExternalData {
            get;
            private set;
        }
        #endregion

        /// <summary>
        /// The ALU(arithmetic logic unit) which executes simple arithemtic operations
        /// </summary>
        public Alu alu;

        /// <summary>
        /// Constructor
        /// Loads the Decoder- and MPM Table and initialises the Busses and the ALU
        /// </summary>
        /// <param name="LoadData">if False, don't parse the Excel-Files(needed for testing the ALU class)</param>
        /// FIXME Retext
        public Processor() {
            DecoderEntries = new List<DecoderEntry>();
            alu =  new Alu(this);
            Data = new Bus();
            ExternalData = new DataBus(this);
        }

    }
}
