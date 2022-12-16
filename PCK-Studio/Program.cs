using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using Dark.Net;

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
        private static void Main()
        {
            #if DEBUG
            Debug.Listeners.Add(new TextWriterTraceListener(Console.Out));
            #endif

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            DarkNet.Instance.SetCurrentProcessTheme(Theme.Auto);

            Form mainForm = new MainForm();
            DarkNet.Instance.SetWindowThemeForms(mainForm, Theme.Auto);

            Application.Run(mainForm);
        }
    }
}
