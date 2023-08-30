#if !DEBUG
#define _NOT_DEBUG
#endif
using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using PckStudio.Classes.Misc;
using PckStudio.Internal;
using PckStudio.Properties;
using PCKStudio_Updater;


namespace PckStudio
{
    static class Program
    {
        public static readonly Uri ProjectUrl = new Uri("https://github.com/PhoenixARC/-PCK-Studio");
        public static readonly string BaseAPIUrl = "http://api.pckstudio.xyz/api/pck";
        public static readonly string BackUpAPIUrl = "https://raw.githubusercontent.com/PhoenixARC/pckstudio.tk/main/studio/PCK/api/";

        public static readonly string AppData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), Application.ProductName);
        public static readonly string AppDataCache = Path.Combine(AppData, "cache");

        public static readonly GithubParams UpdateParams = new GithubParams(
            Path.GetDirectoryName(ProjectUrl.AbsolutePath).Replace("\\", ""),
            Path.GetFileName(ProjectUrl.AbsolutePath),
            Application.ProductName,
            Settings.Default.UsePrerelease,
            new Regex("(\\*|\\d+(\\.\\d+){0,3}(\\.\\*)?)")
            );
        public static readonly IUpdateDownloader Updater = new GithubUpdateDownloader(UpdateParams);


        public static MainForm MainInstance { get; private set; }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            if (Settings.Default.AutoUpdate)
            {
                UpdateToLatest("Click Ok to continue.", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, DialogResult.OK);
            }

            ApplicationScope.Initialize();
            Trace.TraceInformation("Startup");
            RPC.Initialize();
            MainInstance = new MainForm();
            if (args.Length > 0 && File.Exists(args[0]) && args[0].EndsWith(".pck"))
                MainInstance.LoadPckFromFile(args[0]);
            Application.ApplicationExit += (sender, e) => { RPC.Deinitialize(); };
            Application.Run(MainInstance);
        }

        [Conditional("_NOT_DEBUG")]
        internal static void UpdateToLatest(string message, MessageBoxButtons buttons, MessageBoxIcon icon, DialogResult dialogResult)
        {
            if (Updater.IsUpdateAvailable(Application.ProductVersion) &&
                MessageBox.Show(
                    "New update available.\n" +
                    message,
                    "Update Available",
                    buttons, icon, MessageBoxDefaultButton.Button1) == dialogResult)
            {
                Updater.DownloadTo(new DirectoryInfo(Application.StartupPath));
                Updater.Launch();
                Application.Exit();
            }
        }
    }
}