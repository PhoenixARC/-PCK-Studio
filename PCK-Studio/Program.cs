using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using Dark.Net;
using PckStudio.Classes.Misc;
using PckStudio.Internal;

namespace PckStudio
{
    static class Program
    {
        public static readonly string ProjectUrl = "https://github.com/PhoenixARC/-PCK-Studio";
        public static readonly string BaseAPIUrl = "http://api.pckstudio.xyz/api/pck";
        public static readonly string BackUpAPIUrl = "https://raw.githubusercontent.com/PhoenixARC/pckstudio.tk/main/studio/PCK/api/";
        public static readonly string AppData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), Application.ProductName);
        public static readonly string AppDataCache = Path.Combine(AppData, "cache");

        public static MainForm MainInstance { get; private set; }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main(string[] args)
        {
            ApplicationScope.Initialize();
            Trace.TraceInformation("Startup");
            RPC.Initialize();
            MainInstance = new MainForm();
            DarkNet.Instance.SetCurrentProcessTheme(Theme.Auto);
            DarkNet.Instance.SetWindowThemeForms(MainInstance, Theme.Auto);
            if (args.Length > 0 && File.Exists(args[0]) && args[0].EndsWith(".pck"))
                MainInstance.LoadPckFromFile(args[0]);
            Application.ApplicationExit += (sender, e) => { RPC.Deinitialize(); };
            Application.Run(MainInstance);
        }
    }
}
