using Stebs.View;
using System;

namespace Stebs.IO
{
    public class Invoker
    {
        public Action<string> sDel;
        private MainWindow owner;

        public Invoker(MainWindow wOwner)
        {
            owner = wOwner;
        }

        public void Invoke(string sArg)
        {
            owner.Dispatcher.Invoke(sDel, sArg);
        }

        public void BeginInvoke(string sArg)
        {
            owner.Dispatcher.BeginInvoke(sDel, sArg);
        }
    }
}
