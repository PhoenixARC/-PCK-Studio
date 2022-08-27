using System;
using System.Windows.Forms;

namespace PckStudio
{
    static class Program
    {
        public static string baseurl = "http://api.pckstudio.xyz/api/pck";
        public static string backurl = "https://raw.githubusercontent.com/PhoenixARC/pckstudio.tk/main/studio/PCK/api/";
        public static string Appdata = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/PCK-Studio/";
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            var f = new MainForm();
            if (args.Length > 0 && args[0].EndsWith(".pck"))
                f.LoadFromPath(args[0]);
            Application.Run(f);
        }
    }
}
