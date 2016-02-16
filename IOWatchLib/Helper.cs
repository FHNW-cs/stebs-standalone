using System;
using System.Windows.Threading;


namespace Helper {
    public static class Support {
        public static void InvokeIfRequired(this DispatcherObject control, Action operation) {
            if (control.Dispatcher.CheckAccess()) {
                operation();
            }
            else {
                // control.Dispatcher.BeginInvoke (DispatcherPriority.Normal, operation);
                control.Dispatcher.Invoke(DispatcherPriority.Normal, operation);
            }
        }
    }
}
