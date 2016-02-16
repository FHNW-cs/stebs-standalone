using System;
using System.IO;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;
using AvalonDock;
using IOInterfaceLib;
using Stebs.View;
using System.Resources;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Stebs.ViewModel
{
    /// <summary>
    /// Static helper class for load the IO device plugins at startup of the application
    /// </summary>
    static class IOWindowLoader
    {
        /// <summary>
        /// Reads the plugin folder and loads the dll-Files into the application
        /// </summary>
        /// <returns>List of the loaded IO device windows</returns>
        public static List<IOWrapperWindow> LoadPlugins() {
            string[] files = Directory.GetFiles(System.IO.Directory.GetCurrentDirectory() + "\\plugin");
            var libFiles = 
                from file in files
                where Path.GetExtension(file) == ".dll"
                select file;

            // Loop over all .dll-Files in the plugin-folder
            List<IOWrapperWindow> windows = new List<IOWrapperWindow>();
            foreach(string file in libFiles) {

                // Load the IOWindow
                IOWrapperWindow ioW = LoadIOWindow(file);

                if (ioW != null) {
                    // if loading successfull -> add window to the list
                    windows.Add(ioW);
                }
            }

            // return the loaded windows
            return windows;
        }

        /// <summary>
        /// Loads one dll and creates a decorater IOWrapperWindow class for the loaded IO device
        /// </summary>
        /// <param name="dllPath">Path of the dll to load</param>
        /// <returns>The loaded dll as an IOWrapperWindow object</returns>
        public static IOWrapperWindow LoadIOWindow(string dllPath) {
            // Load the DLL
            Assembly ase = Assembly.LoadFile(dllPath);
            foreach(Type t in ase.GetExportedTypes()) {
                // Check if the Type implements IIOWindow
                if (t.GetInterface("IIOWindow") != null) {

                    // If the Class in the DLL inherits already from AvalanDock.DockableContent create the Instance directly
                    IOWrapperWindow ioW;
                    if (t.BaseType.FullName == "AvalonDock.DockableContent") {
                        //ioW =(IIOWindow)ase.CreateInstance(t.FullName); // should never happen
                        throw new Exception("IOWindows which inherits already from AvalonDock.DockableContent aren't supported");

                    // otherwise create a Wrapper around the Instance
                    } else {
                        ioW = new IOWrapperWindow((IIOWindow)ase.CreateInstance(t.FullName));
                    }

                    // Set the title of the DockableContent(important for saving the layout)
                   ((DockableContent)ioW).Name = ioW.GetName().Replace(" ", "");

                    // Get the icon from the Assembly and save it to the WrapperWindow
                    Stream st = ase.GetManifestResourceStream(t.Namespace + ".res.icon.png");                    
                    if (st != null) {
                        ioW.RibbonIcon = new BitmapImage();

                        ioW.RibbonIcon.BeginInit();
                        ioW.RibbonIcon.CacheOption = BitmapCacheOption.OnLoad;
                        ioW.RibbonIcon.UriSource = null;
                        ioW.RibbonIcon.StreamSource = st;
                        ioW.RibbonIcon.EndInit();
                    }
                    
                    return ioW;
                }
            }
            return null;
        }
    }
}
