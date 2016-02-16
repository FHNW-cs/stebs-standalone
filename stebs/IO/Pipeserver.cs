using Stebs.View;
using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Windows;

namespace Stebs.IO
{
    public class Pipeserver
    {
        public static MainWindow owner;
        public static Invoker ownerInvoker;
        public static string pipeName;
        private static NamedPipeServerStream pipeServer;
        private static readonly int BufferSize = 256;

        private static void SetFile(String path)
        {
            AsmCodeWindow acw = owner.GetAsmCodeWindow();
            acw.NewCommandHandler();
            acw.OpenFile(path);
            owner.ResetExecuted();
            owner.assemble();
            
        }
 

        public static void createPipeServer()
        {
            Decoder decoder = Encoding.Default.GetDecoder();
            Byte[] bytes = new Byte[BufferSize];
            char[] chars = new char[BufferSize];
            int numBytes = 0;
            StringBuilder msg = new StringBuilder();
            ownerInvoker.sDel = SetFile;
            try
            {
                pipeServer = new NamedPipeServerStream(pipeName, PipeDirection.In, 1,
                                                 PipeTransmissionMode.Message,
                                                 PipeOptions.Asynchronous);
               
                while (true)
                {
                   
                    pipeServer.WaitForConnection();

                    do
                    {
                        msg.Length = 0;
                        do
                        {
                            numBytes = pipeServer.Read(bytes, 0, BufferSize);
                            if (numBytes > 0)
                            {
                                int numChars = decoder.GetCharCount(bytes, 0, numBytes);
                                decoder.GetChars(bytes, 0, numBytes, chars, 0, false);
                                msg.Append(chars, 0, numChars);
                            }
                        } while (numBytes > 0 && !pipeServer.IsMessageComplete);
                        decoder.Reset();
                        if (numBytes > 0)
                        {
                           ownerInvoker.Invoke(msg.ToString().Trim());
                        }
                    } while (numBytes != 0);


                    pipeServer.Disconnect();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }

}
