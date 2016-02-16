namespace Stebs.ViewModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Stebs.Model;
    using System.Collections;

    public class MPMEntryViewModel :  ViewModelBase
    {
        private static Dictionary<SRCRegister, string> srcMapper = new Dictionary<SRCRegister, string>()
        {
            {SRCRegister.EMPTY,String.Empty},
            {SRCRegister.FROM_DATA,"From Data"},
            {SRCRegister.FROM_IP, "From IP"},
            {SRCRegister.FROM_MAR, "From MAR"},
            {SRCRegister.FROM_MBR, "From MBR"},
            {SRCRegister.FROM_MDR, "From MDR"},
            {SRCRegister.FROM_RES, "From RES"},
            {SRCRegister.FROM_SEL_REF,"From |SEL|"},
            {SRCRegister.FROM_SR, "From SR"}
        };

        private static Dictionary<DESTRegister, string> dstMapper = new Dictionary<DESTRegister, string>()
        {
            {DESTRegister.EMPTY,String.Empty},
            {DESTRegister.TO_IP,"To IP"},
            {DESTRegister.TO_IR, "To IR"},
            {DESTRegister.TO_MAR, "To MAR"},
            {DESTRegister.TO_MBR, "To MBR"},
            {DESTRegister.TO_MDR, "To MDR"},
            {DESTRegister.TO_RES, "To RES"},
            {DESTRegister.TO_SEL,"To SEL"},
            {DESTRegister.TO_SEL_REF, "To |SEL|"},
            {DESTRegister.TO_SR, "To SR"},
            {DESTRegister.TO_X, "To X"},
            {DESTRegister.TO_Y, "To Y"}
        };

        public MPMEntryViewModel(MPMEntry model) : base(){
            this.Model = model;
        }

        /// <summary>
        /// The underlying model which is wrapped by the viewmodel
        /// </summary>
        public MPMEntry Model
        {
            get;
            private set;
        }

        /// <summary>
        /// MPM Address as a HEX-string
        /// </summary>
        public string Addr {
            get
            {
                return Model.Addr.ToHexString(3);
            }
        }

        /// <summary>
        /// Next MIP Address as HEX-string
        /// </summary>
        public string Na {
            get {

                return Model.GetName(Model.Na);
            }
        }

        /// <summary>
        /// Enable Value Flag as string(0 -> "" or 1 -> "Enable")
        /// </summary>
        public string Ev {
            get {
                return Model.Ev ? "Enable" : "";
            }
        }

        /// <summary>
        /// Value as HEX-String
        /// </summary>
        public string Val {
            get {
                return Model.Val.ToHexString(3);
            }
        }

        /// <summary>
        /// Criteria as HEX-String
        /// </summary>
        public string Crit
        {
            get
            {
                return Model.GetName(Model.Crit);
            }
        }

        /// <summary>
        /// Clear interrupt flag as string (0 -> "" or 1 -> "Clear")
        /// </summary>
        public string Cif {
            get {
                return Model.Cif ? "Clear" : "";
            }
        }

        /// <summary>
        /// Affect flags as string(0 -> "" or 1 -> "Affect")
        /// </summary>
        public string Af {
            get {
                return Model.Af ? "Affect" : "";
            }
        }

        /// <summary>
        /// ALU operation as HEX-string
        /// </summary>
        public string Alu {
            get {
                return Model.GetName(Model.Alu);
            }
        }

        /// <summary>
        /// Displayable string of the source register
        /// </summary>
        public string Src
        {
            get
            {
                return srcMapper[Model.Src];
            }
        }

        /// <summary>
        /// Displayable information of the destination register
        /// </summary>
        public string Dst
        {
            get
            {
                return dstMapper[Model.Dst];
            }
        }

        /// <summary>
        /// String representation of IO/Memory control
        /// </summary>
        public string IoM
        {
            get { return Model.IoM ? "IO" : "MEM";  }
        }


        /// <summary>
        /// Read/Write Control
        /// </summary>
        public string Rw {
            get {
                return(Model.Rw ? "R" : "W");
            }
        }

    }
}
