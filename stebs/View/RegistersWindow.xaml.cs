using Stebs.ViewModel;
using System.Windows.Input;

namespace Stebs.View
{
    /// <summary>
    /// Interaction logic for MacroWindow.xaml
    /// </summary>
    public partial class RegistersWindow : AvalonDock.DockableContent
    {

        public ProcessorViewModel procVM;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="procVM">The Processor ViewModel</param>
        public RegistersWindow(ProcessorViewModel procVM) {
            InitializeComponent();

            this.procVM = procVM;

            DataContext = procVM;
        }

        private void CopyCommandBinding(object sender, ExecutedRoutedEventArgs e)
        {
            procVM.CopyRegistersCommand.Execute(e.Parameter);    
        }

        private void CanCopyCommandBinding(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = procVM.CopyRegistersCommand.CanExecute(e.Parameter);
            e.Handled = procVM.CopyRegistersCommand.CanExecute(e.Parameter);
        }
        
    }
}
