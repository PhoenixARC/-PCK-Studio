using System;
using System.IO;
using System.Windows.Forms;

namespace PckStudio
{
    static class Program
    {
        public static string BaseAPIUrl = "http://api.pckstudio.xyz/api/pck";
        public static string BackUpAPIUrl = "https://raw.githubusercontent.com/PhoenixARC/pckstudio.tk/main/studio/PCK/api/";
        public static string AppData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "PCK-Studio");
        public static string AppDataCache = Path.Combine(AppData, "cache");
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            var f = new MainForm();
            if (args.Length > 0 && File.Exists(args[0]) && args[0].EndsWith(".pck"))
                f.LoadFromPath(args[0]);
            Application.Run(f);
        }
    }
}
