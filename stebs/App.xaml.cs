using System;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.IO.Pipes;
using System.Text;
using System.IO;


namespace Stebs
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private Mutex _instanceMutex = null;

        public static String[] mArgs = new string[0];

        public static String realAppStartPath = "";


        private void Application_Startup(object sender, StartupEventArgs e) {
            if (e.Args.Length > 0) {
                mArgs = e.Args;
            }
            realAppStartPath = Directory.GetCurrentDirectory();
        }


        protected override void OnStartup(StartupEventArgs e) {
            System.Globalization.CultureInfo.DefaultThreadCurrentCulture =
                Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.CreateSpecificCulture("de-CH");
            
            bool mProc;
            
            if (e.Args.Length > 0) {
                mArgs = e.Args;
            }
            realAppStartPath = Directory.GetCurrentDirectory();
            
            _instanceMutex = new Mutex(true, @"Global\cae245fa-fcc8-4b3f-996c-97c7848cb638", out mProc);

            if (!mProc) {
                if (mArgs.Length > 0) {
                    using (NamedPipeClientStream pipeClient = new NamedPipeClientStream(".", "stebspipe",
                                              PipeDirection.Out,
                                              PipeOptions.Asynchronous))
                    {
                        try {
                            pipeClient.Connect(2000);
                        }
                        catch {
                            MessageBox.Show("Cannot connect to the pipe, please restart stebs!",
                                            "Hey buddy, there is no pipe", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                            return;
                        }
                        using (StreamWriter sw = new StreamWriter(pipeClient)) {
                            String fileName = @"" + mArgs[0];
                            if (!Path.IsPathRooted(fileName)) {
                                fileName = Path.Combine(Environment.CurrentDirectory, fileName);
                                fileName = Path.GetFullPath(fileName);
                            }
                            if (File.Exists(fileName)) {
                                sw.WriteLine(fileName.Trim());
                            }
                            else {
                                MessageBox.Show("File: " + fileName + " not found!",
                                                "Stebs is already running", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                            }
                        }
                        pipeClient.Close();
                    }
                }
                else {
                    MessageBox.Show("Another instance of stebs is already running.",
                                    "Stebs is already running", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }

                Application.Current.Shutdown();
                return;
            }
            else {
                this.StartupUri = new System.Uri("View/MainWindow.xaml", System.UriKind.Relative);
                base.OnStartup(e);
            }

            /* REFACTOR ME */

            /*if (!System.Diagnostics.Debugger.IsAttached) // should check if started from visual studio
            {
                try
                {
                    Directory.SetCurrentDirectory(@"" + Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData).ToString() + "\\Stebs");
                }
                catch (DirectoryNotFoundException)
                {
                    Copy(@"res", @"" + Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData).ToString() + "\\Stebs\\res");
                    Copy(@"plugin", @"" + Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData).ToString() + "\\Stebs\\plugin");
                    Directory.SetCurrentDirectory(@"" + Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData).ToString() + "\\Stebs");
                }
            }*/
        }


        protected override void OnExit(ExitEventArgs e) {
            try {
                if (_instanceMutex != null) {
                    _instanceMutex.ReleaseMutex();
                }
                base.OnExit(e);
            }
            catch { }
        }


        public static void Copy(string sourceDirectory, string targetDirectory) {
            DirectoryInfo diSource = new DirectoryInfo(sourceDirectory);
            DirectoryInfo diTarget = new DirectoryInfo(targetDirectory);
            CopyAll(diSource, diTarget);
        }


        public static void CopyAll(DirectoryInfo source, DirectoryInfo target) {
            // Check if the target directory exists, if not, create it.
            if (Directory.Exists(target.FullName) == false) {
                Directory.CreateDirectory(target.FullName);
            }

            // Copy each file into its new directory.
            foreach (FileInfo fi in source.GetFiles()) {
                Console.WriteLine(@"Copying {0}\{1}", target.FullName, fi.Name);
                fi.CopyTo(Path.Combine(target.ToString(), fi.Name), true);
            }

            // Copy each subdirectory using recursion.
            foreach (DirectoryInfo diSourceSubDir in source.GetDirectories()) {
                DirectoryInfo nextTargetSubDir =
                    target.CreateSubdirectory(diSourceSubDir.Name);
                CopyAll(diSourceSubDir, nextTargetSubDir);
            }
        }
    }
}
