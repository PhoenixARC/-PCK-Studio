using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace PckStudio
{
    static class Program
    {
        public static readonly string BaseAPIUrl = "http://api.pckstudio.xyz/api/pck";
        public static readonly string BackUpAPIUrl = "https://raw.githubusercontent.com/PhoenixARC/pckstudio.tk/main/studio/PCK/api/";
        public static readonly string AppData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "PCK-Studio");
        public static readonly string AppDataCache = Path.Combine(AppData, "cache");
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
#if DEBUG
            Debug.Listeners.Add(new TextWriterTraceListener(Console.Out));
#endif

            System.Globalization.CultureInfo.CurrentCulture.NumberFormat = System.Globalization.NumberFormatInfo.InvariantInfo;

            var mianForm = new MainForm();
            if (args.Length > 0 && File.Exists(args[0]) && args[0].EndsWith(".pck"))
                mianForm.LoadPck(args[0]);
            Application.Run(mianForm);
        }
    }
}
