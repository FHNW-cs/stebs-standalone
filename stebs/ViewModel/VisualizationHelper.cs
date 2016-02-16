
namespace Stebs.ViewModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Stebs.Model;
    using Stebs.View.CustomControls;

    /// <summary>
    /// Static class which provides some functions for the visualization-process
    /// </summary>
    public static class VisualizationHelper
    {
        
        /// <summary>
        /// Returns the selected FROM Register as a string
        /// </summary>
        /// <param name="entry">The micro instruction</param>
        /// <param name="sel">The current selected general purpouse register</param>
        /// <returns>The selected FROM Regsiter as a string</returns>
        public static string GetFromReg(MPMEntry entry,byte sel) {
            if (SRCRegister.FROM_SEL_REF.Equals(entry.Src))
            {
                return GetSelReg(sel);
            }

            if (SRCRegister.FROM_IP.Equals(entry.Src))
            {
                return "IP";
            }

            if (SRCRegister.FROM_MBR.Equals(entry.Src)) {
                return "MBR";
            }

            if (SRCRegister.FROM_MAR.Equals(entry.Src))
            {
                return "MAR";
            }

            if (SRCRegister.FROM_MDR.Equals(entry.Src))
            {
                return "MDR";
            }

            if (SRCRegister.FROM_RES.Equals(entry.Src))
            { 
                return "RES";
            }

            if (SRCRegister.FROM_SR.Equals(entry.Src))
            {
                return "SR";
            }

            if(SRCRegister.FROM_DATA.Equals(entry.Src))
            {
                return "DATA";
            }

            return String.Empty;
        }
        /// <summary>
        /// Returns the selected TO Register as a string
        /// </summary>
        /// <param name="entry">The micro instruction entry</param>
        /// <param name="sel">The current selected general purpouse register</param>
        /// <returns>The selected TO Regsiter as a string</returns>
        public static string GetToReg(MPMEntry entry, byte sel) {
            if( DESTRegister.EMPTY.Equals(entry.Dst))
            {
                return String.Empty;
            }

            if (DESTRegister.TO_SEL.Equals(entry.Dst))
            {
                return "SEL";
            }

            if (DESTRegister.TO_SEL_REF.Equals(entry.Dst))
            {
                return GetSelReg(sel);
            }

            if (DESTRegister.TO_IP.Equals(entry.Dst))
            {
                return "IP";
            }

            if (DESTRegister.TO_MBR.Equals(entry.Dst))
            { 
                return "MBR";
            } 
            
            if (DESTRegister.TO_MAR.Equals(entry.Dst))
            {
                return "MAR";
            }
            
            if (DESTRegister.TO_MDR.Equals(entry.Dst))
            { 
                return "MDR";
            }
            
            if (DESTRegister.TO_RES.Equals(entry.Dst))
            { 
                return "RES";
            }
            
            if (DESTRegister.TO_SR.Equals(entry.Dst))
            { 
                return "SR";
            }
            
            if( DESTRegister.TO_IR.Equals(entry.Dst))
            {
                return "IR";
            }

            if (DESTRegister.TO_X.Equals(entry.Dst))
            {
                return "ALU_X";
            }

            if (DESTRegister.TO_Y.Equals(entry.Dst))
            {
                return "ALU_Y";
            }

            return String.Empty;
        }

        public static string GetAluCommand(MPMEntry entry)
        {
            return Enum.GetName(typeof(Alu.Cmd), entry.Alu);
        }

        public static string GetAffectedFlag(MPMEntry entry)
        {
            return entry.Af.ToString();
        }



        /// <summary>
        /// Returns the selected general purpouse register as a string
        /// </summary>
        /// <param name="SEL">the selected register-value</param>
        /// <returns>the selected general purpouse register as a string</returns>
        public static string GetSelReg(byte SEL) {
            switch (SEL) {
                case(byte)Processor.REGISTER.AL:
                    return "AL";
                case(byte)Processor.REGISTER.BL:
                    return "BL";
                case(byte)Processor.REGISTER.CL:
                    return "CL";
                case(byte)Processor.REGISTER.DL:
                    return "DL";
                case(byte)Processor.REGISTER.SP:
                    return "SP";
            }
            return "";
        }

        public delegate bool IsJumpSuccessfulDelegate(MPMEntry.CRIT crit);
        public delegate bool IsJumpDelegate(MPMEntry.CRIT crit);
        public delegate bool IsInterruptDelegate();

        /// <summary>
        /// Returns the type of the next MIP as a string
        /// </summary>
        /// <param name="mpmEntry">MPM Entry</param>
        /// <param name="IsJump">Delegate function which checks if there is a jump</param>
        /// <param name="IsJumpSuccessful">Delegate function which checks if the jump was successful</param>
        /// <param name="IsInterrupt">Delegate function which checks if there's an interrupt pending</param>
        /// <returns>Type of the next MIP as a string</returns>
        public static string GetMIP(MPMEntry mpmEntry, IsJumpDelegate IsJump, IsJumpSuccessfulDelegate IsJumpSuccessful, IsInterruptDelegate IsInterrupt) {
            switch (mpmEntry.Na) {
                case MPMEntry.NA.NEXT: // Next Branch Address
                    if (IsJump(mpmEntry.Crit)) {
                        if (IsJumpSuccessful(mpmEntry.Crit)) {
                            return "JUMP";
                        } else {
                            return "NOJMP";
                        }
                    } else {
                        return "NEXT";
                    }

                case MPMEntry.NA.DECODE: // Decoder Address
                    return "DEC";

                case MPMEntry.NA.FETCH: // Fetch Address
                    if (IsInterrupt()) {
                        return "INT";
                    } else {
                        return "FETCH";
                    }
            }
            return "";
        }

        public static string GetEnableValue(MPMEntry entry)
        {
            return entry.Ev.ToString();
        }

        public static string GetCif(MPMEntry entry)
        {
            return entry.Cif.ToString();
        }

        public static string GetRw(MPMEntry entry)
        {
            return entry.Rw ? "R" : "W";
        }

    }

   
}
