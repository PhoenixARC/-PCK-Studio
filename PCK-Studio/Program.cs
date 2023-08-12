using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
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
        static void Main(string[] args)
        {
            ApplicationScope.Initialize();
            RPC.Initialize();
            MainInstance = new MainForm();
            if (args.Length > 0)
                MainInstance.LoadPckFromFile(args.Where(s => s.EndsWith(".pck") && File.Exists(s)));
            Application.ApplicationExit += (sender, e) => { RPC.Deinitialize(); };
            Application.Run(MainInstance);
        }
    }
}
