namespace Stebs.Model
{
    using Stebs.View;
    using Stebs.ViewModel;

    using System;
    using System.IO;
    using NLog;

    using assembler;
    using assembler.support;


    /// <summary>
    /// Class for reading the Assembler Code, and generate the machine code
    /// </summary>
    public class AssemblerParser
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Machine code byte array for the whole RAM
        /// </summary>
        private byte[] machineCode = new byte[256];
        public byte[] MachineCode {
            get {
                return machineCode;
            }

            private set {
                machineCode = value;
            }
        }

        /// <summary>
        /// Array to assign every RAM Position to the code line
        /// </summary>
        private int[] ramPosToCodeLine = new int[256];
        public int[] RamPosToCodeLine {
            get {
                return ramPosToCodeLine;
            }

            private set {
                ramPosToCodeLine = value;
            }
        }

        /// <summary>
        /// Processor VM for a reset of the simulator
        /// </summary>
        private ProcessorViewModel processorVM;

        /// <summary>
        /// Output Window for messaging errors
        /// </summary>
        private IOutputWindow output;

        /// <summary>
        /// The constructor of the assembler parser
        /// </summary>
        /// <param name="procVM">The Processor View Model for handling the simulator</param>
        /// <param name="window">The output window to write error or success messages</param>
        public AssemblerParser(ProcessorViewModel procVM, IOutputWindow window) {
            this.processorVM = procVM;
            output = window;

            Reset();
        }


        /// <summary>
        /// Resets the RAM, the Simulator and all arrays
        /// </summary>
        private void Reset() {
            // Reset all Elements
            processorVM.Reset();

            for (int i = 0; i < ramPosToCodeLine.Length; i++) {
                ramPosToCodeLine[i] = -1;
            }

            for (int i = 0; i < machineCode.Length; i++) {
                machineCode[i] = 0;
            }
        }


        /// <summary>
        /// Opens a file stream for reading.
        /// </summary>
        /// <param name="filepath">The file</param>
        /// <returns></returns>
        private static Stream GetFileStream(string filepath) {
            return new FileStream(filepath, FileMode.Open, FileAccess.Read, FileShare.Read);
        }


        /// <summary>
        /// The main function that is called to assemble the given code.
        /// The assembler code is written in Java. Then a jar file was built and converted
        /// into a dll file by means of the IKVM compiler.
        /// </summary>
        /// <param name="asmText">The assembler code</param>
        /// <returns>success or not</returns>
        public bool Assemble(string asmText) {
            // Reset all arrays and lists
            Reset();

            if (string.IsNullOrWhiteSpace(asmText)) {
                WriteError(Common.ERROR_MESSAGE);
                WriteError("Nothing to assemble");
                return false;
            }

            // Get instructions read from file INSTRUCTION.data
            string instructionData = MainWindow.InstructionString;

            // Assemble and set up code list or signal error.
            // assembler.Assembler called from IKVMassembler.dll
            var assembler = new Assembler("");
            if (assembler.execute(asmText, instructionData)) {
                // Copy machine code from Java array to C# array
                int[] ram = Common.getRam();
                for (int i = 0; i < machineCode.Length; ++i) {
                    machineCode[i] = (byte)ram[i];
                }

                // Fill array with line numbers; used for highlighting
                for (int i = 0; i < RamPosToCodeLine.Length; ++i) {
                    RamPosToCodeLine[i] = assembler.getCodeToLineArr()[i];
                }
                // Success
                return true;
            }
            else {
                //
                WriteError(Common.ERROR_MESSAGE);
                return false;
            }
        }

        private void WriteError(string text) {
            output.WriteError(text);
        }
    }
}
