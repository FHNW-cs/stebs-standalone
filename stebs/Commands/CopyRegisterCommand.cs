namespace Stebs.Commands
{
    using Stebs.Model;
    using Stebs.ViewModel;
    using System;
    using System.Text;
    using System.Windows;
    using System.Windows.Input;

    /// <summary>
    /// This command is used to copy the content of the registers
    /// into the clipboard.
    /// </summary>
    internal class CopyRegistersCommand : ICommand {
        /// <summary>
        /// This instance holds the register information.
        /// </summary>
        private ProcessorViewModel viewModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="CopyRegistersCommand"/> class.
        /// </summary>
        /// <param name="vm">The register container.</param>
        public CopyRegistersCommand(ProcessorViewModel vm) {
            viewModel = vm;
        }

        /// <summary>
        /// Event that rises if the execution state has changed.
        /// </summary>
        public event EventHandler CanExecuteChanged {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        /// <summary>
        /// Whether the command can be executed or not.
        /// </summary>
        /// <param name="parameter">Some command arguments - ignored here.</param>
        /// <returns>True is returned in any case.</returns>
        public bool CanExecute(object parameter) {
            return true;
        }

        /// <summary>
        /// Copies the register values into the clipboard.
        /// </summary>
        /// <param name="parameter">This value is ignored.</param>
        public void Execute(object parameter) {
            StringBuilder builder = new StringBuilder();
            Processor proc = viewModel.proc;
            builder.Append("AL: ");
            builder.Append(RegistersViewConverter.Convert(proc.AL));
            builder.Append(Environment.NewLine);
            builder.Append("BL: ");
            builder.Append(RegistersViewConverter.Convert(proc.BL));
            builder.Append(Environment.NewLine);
            builder.Append("CL: ");
            builder.Append(RegistersViewConverter.Convert(proc.CL));
            builder.Append(Environment.NewLine);
            builder.Append("DL: ");
            builder.Append(RegistersViewConverter.Convert(proc.DL));
            builder.Append(Environment.NewLine);
            builder.Append(Environment.NewLine);
            builder.Append("SP: ");
            builder.Append(RegistersViewConverter.Convert(proc.SP));
            builder.Append(Environment.NewLine);
            builder.Append("IP: ");
            builder.Append(RegistersViewConverter.Convert(proc.IP));
            builder.Append(Environment.NewLine);
            builder.Append("SR: ");
            builder.Append(RegistersViewConverter.Convert(proc.SR));
            builder.Append(Environment.NewLine);

            Clipboard.SetText(builder.ToString());
        }
    }
}
