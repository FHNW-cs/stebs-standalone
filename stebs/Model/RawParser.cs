using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Stebs.Model;
using Stebs.ViewModel;

namespace Stebs.Model
{
    public class RawParser : IProcessorParser
    {
        private TextReader rom1Reader;
        private TextReader rom2Reader;
        private TextReader instructionReader;

        private static Dictionary<int, DESTRegister> destMap = new Dictionary<int, DESTRegister>()
        {
            {11, DESTRegister.TO_IP},
            {10, DESTRegister.TO_IR},
            {9,  DESTRegister.TO_MAR},
            {8,  DESTRegister.TO_MBR},
            {7,  DESTRegister.TO_MDR}, 
            {6,  DESTRegister.TO_RES},
            {5,  DESTRegister.TO_SEL},
            {4,  DESTRegister.TO_SEL_REF},
            {3,  DESTRegister.TO_SR},
            {2,  DESTRegister.TO_X},
            {1,  DESTRegister.TO_Y},
            {0,  DESTRegister.EMPTY}
        };

        private static Dictionary<int,SRCRegister> srcMap = new Dictionary<int, SRCRegister>()
        {
            
            {12, SRCRegister.FROM_DATA},
            {11, SRCRegister.FROM_IP},
            {9,  SRCRegister.FROM_MAR},
            {8,  SRCRegister.FROM_MBR},
            {7,  SRCRegister.FROM_MDR}, 
            {6,  SRCRegister.FROM_RES},
            {4,  SRCRegister.FROM_SEL_REF},
            {3,  SRCRegister.FROM_SR},
            {0,  SRCRegister.EMPTY}
        };


        public RawParser(StreamReader rom1, StreamReader rom2, StreamReader instruction)
        {
            rom1Reader = rom1;
            rom2Reader = rom2;
            instructionReader = instruction;
        }

        public Processor Parse()
        {
            Processor proc = new Processor();
            ReadDecoderEntries(proc);
            ReadMPMEntries(proc);
            
            return proc;
        }
      
        private void ReadDecoderEntries(Processor proc)
        {
            string[] decoderTableEntries = ReadText(instructionReader);
            
            foreach(string entry in decoderTableEntries)
            {
                if (string.IsNullOrEmpty(entry)) continue;

                string[] parts = entry.Split(';');
                DecoderEntry decodeEntry = new DecoderEntry();

                decodeEntry.MPMAddress =("0" + parts[0]).HexToInt();
                decodeEntry.OpCode = parts[1].HexToByte();
                decodeEntry.InstructionType = InstructionTypeParser.Parse(parts[2]);
                proc.DecoderEntries.Add(decodeEntry);
            }
        }

        /// <summary>
        /// Read a given stream into a string array. 
        /// The string array contains the rows.
        /// </summary>
        /// <param name="reader">The text reader which contains
        /// the text to be parsed
        /// </param>
        /// <returns>A string array with each line separed</returns>
        private string[] ReadText(TextReader reader)
        {
            return reader.ReadToEnd().Split('\n');
        }


        /// <summary>
        /// Parse Rom-Files, with MPM Entries
        /// </summary>
        /// <returns>a list with the MPMEntries of this command</returns>
        public void ReadMPMEntries(Processor proc)
        {
            ParseRom1(proc);
            ParseRom2(proc);
            
        }

        private void ParseRom1(Processor proc)
        {
            string[] romDataEntries = ReadText(rom1Reader);
            Boolean isFirstLine = true;
            
            foreach(string line in romDataEntries)
            {
                if (isFirstLine)
                {
                    isFirstLine = false;
                    continue;
                }

                string[] microcodes = line.Split((char[])null, StringSplitOptions.RemoveEmptyEntries); // split by whitespace
                ParseRom1Line(proc,microcodes);
            }
        }

        private void ParseRom1Line(Processor proc,string[] microcodes)
        {
            foreach(string microcode in microcodes)
            {
                MPMEntry entry = ParseRom1Block(microcode);

                if (proc.mpmEntries.ContainsKey(entry.Addr)) {
                    throw new ProcessorParserException(String.Format("Duplicate MPM entry : {0}", entry.Addr));
                }
                proc.mpmEntries.Add(entry.Addr,entry);
            }
        }

        private MPMEntry ParseRom1Block(String block)
        {
            MPMEntry entry = null;
            int microcodeBits = Convert.ToInt32(block, 16);
            
            entry = new MPMEntry();
            entry.Addr = ( microcodeBits >> 20) & 0xFFF;      // MPMA
            entry.Na   = ParseNa((microcodeBits >> 18) & 0x3); // Next MIP Address
            entry.Cif  = ((microcodeBits >> 17) & 0x1) == 1;  // Clear Interrupt Flag(old IC)
            entry.Ev   = ((microcodeBits >> 16) & 0x1) == 1;  // Enable Value Flag(X Path Selector)
            entry.Val  = ( microcodeBits >> 4) & 0xFFF;       // Offset Address or Value
            entry.Af   = ((microcodeBits >> 3) & 1) == 1;     // Affected Flag(olf Fl)
            entry.Crit = ParseCrit((microcodeBits >> 0) & 7); // Flag Criterion

            return entry;
        }

        

        private void ParseRom2(Processor proc)
        {
            string[] romDataEntries = ReadText(rom2Reader);
            Boolean isFirstLine = true;

            foreach(string line in romDataEntries)
            {
                if (isFirstLine)
                {
                    isFirstLine = false;
                    continue;
                }

                string[] microcodes = line.Split((char[])null, StringSplitOptions.RemoveEmptyEntries); // split by whitespace

                ParseRom2Line(proc,microcodes);
            }
            return;
        }

        private void ParseRom2Line(Processor proc, string[] microcodes)
        {
            foreach(string microcode in microcodes)
            {
                int microcodeBits = Convert.ToInt32(microcode, 16);

                MPMEntry entry = proc.mpmEntries[(microcodeBits >> 16) & 0xFFF]; // MPMA
                entry.Alu = ParseAluCommand((microcodeBits >> 12) & 0x0F);       // Alu
                entry.Dst = ParseDesination(((microcodeBits >> 8) & 0x0F));      // Destination
                entry.Src = ParseSource((microcodeBits >> 4) & 0x0F);            // Source
                entry.Rw  = (((microcodeBits >> 3) & 0x01) == 1 ? true : false); // Rw
                entry.IoM =(((microcodeBits >> 2) & 0x01) == 1 ? true : false);  // IO/Mem
                // Last bit 'Nu' - not used
            }
            return;
        }

        private MPMEntry.CRIT ParseCrit(int value)
        {
            return( MPMEntry.CRIT) Enum.Parse(typeof(MPMEntry.CRIT), value.ToString(),true);
        }

        private MPMEntry.NA ParseNa(int value)
        {
            return (MPMEntry.NA) Enum.Parse(typeof(MPMEntry.NA), value.ToString(), true);
        }

        private Alu.Cmd ParseAluCommand(int value)
        {
            return (Alu.Cmd)Enum.Parse(typeof(Alu.Cmd), value.ToString(), true);
        }

        private SRCRegister ParseSource(int value)
        {
            if (!srcMap.ContainsKey(value))
            {
                throw new ProcessorParserException(String.Format("Unknown source register: {0}",value));
            }

            return srcMap[value];
        }

        private DESTRegister ParseDesination(int value)
        {
            if (!destMap.ContainsKey(value))
            {
                throw new ProcessorParserException(String.Format("Unknown destination register {0}", value));
            }

            return destMap[value];
        }

    }
}
