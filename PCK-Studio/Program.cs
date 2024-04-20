using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;
using System.Windows.Forms;
using PckStudio.Classes.Misc;
using PckStudio.Internal;
using PckStudio.Properties;
using PCKStudio_Updater;


namespace PckStudio
{
    static class Program
    {
        internal static readonly Uri ProjectUrl = new Uri("https://github.com/PhoenixARC/-PCK-Studio");
        internal static readonly string BaseAPIUrl = "http://api.pckstudio.xyz/api/pck";
        internal static readonly string BackUpAPIUrl = "https://raw.githubusercontent.com/PhoenixARC/pckstudio.tk/main/studio/PCK/api/";

        internal static readonly string AppData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), Application.ProductName);
        internal static readonly string AppDataCache = Path.Combine(AppData, "cache");

        private static readonly GithubParams UpdateParams = new GithubParams(
            Path.GetDirectoryName(ProjectUrl.AbsolutePath).Replace("\\", ""),
            Path.GetFileName(ProjectUrl.AbsolutePath),
            Application.ProductName,
            Settings.Default.UsePrerelease,
            new Regex("(\\*|\\d+(\\.\\d+){0,3}(\\.\\*)?)")
            );
        internal static readonly IUpdateDownloader Updater = new GithubUpdateDownloader(UpdateParams);


        internal static MainForm MainInstance { get; private set; }

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
            if (args.Length > 0)
                MainInstance.LoadPckFromFile(args.Where(arg => arg.EndsWith(".pck") && File.Exists(arg)));
            Application.ApplicationExit += (sender, e) => { RPC.Deinitialize(); };
            Application.Run(MainInstance);
        }

        [Conditional("NDEBUG")]
        internal static void UpdateToLatest(string message, MessageBoxButtons buttons, MessageBoxIcon icon, DialogResult dialogResult)
        {
            bool updateAvailable = Updater.IsUpdateAvailable(Application.ProductVersion);
            if (updateAvailable && MessageBox.Show(
                    MainInstance ?? null,
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