using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;
using System.Windows.Forms;
using PckStudio.Internal.Misc;
using PckStudio.Internal;
using PckStudio.Properties;
using PckStudio.Internal.App;
using AutoUpdaterDotNET;
using Newtonsoft.Json;


namespace PckStudio
{
    static class Program
    {
        internal static readonly Uri ProjectUrl = new Uri("https://github.com/PhoenixARC/-PCK-Studio");
        internal static readonly Uri RawProjectUrl = new Uri("https://raw.githubusercontent.com/PhoenixARC/-PCK-Studio");
        internal static readonly string BaseAPIUrl = "http://api.pckstudio.xyz/api/pck";
        internal static readonly string BackUpAPIUrl = "https://raw.githubusercontent.com/PhoenixARC/pckstudio.tk/main/studio/PCK/api/";

        internal static readonly string AppData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), Application.ProductName);
        internal static readonly string AppDataCache = Path.Combine(AppData, "cache");

        internal static MainForm MainInstance { get; private set; }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            AutoUpdater.SetOwner(MainInstance);
            //AutoUpdater.ClearAppDirectory = true;
#if DEBUG
            AutoUpdater.ReportErrors = true;
#endif
            AutoUpdater.DownloadPath = Application.StartupPath;
            AutoUpdater.ExecutablePath = "./PCK-Studio.exe";
            AutoUpdater.TopMost = true;

            string jsonPath = Path.Combine(Environment.CurrentDirectory, "updates.json");
            AutoUpdater.PersistenceProvider = new JsonFilePersistenceProvider(jsonPath);
            AutoUpdater.ParseUpdateInfoEvent += AutoUpdaterOnParseUpdateInfoEvent;
            AutoUpdater.Icon = Resources.ProjectLogo.ToBitmap();
           
            if (Settings.Default.AutoUpdate)
            {
                UpdateToLatest();
            }

            ApplicationScope.Initialize();
            Trace.TraceInformation("Startup");
            RPC.Initialize();
            MainInstance = new MainForm();
            if (args.Length > 0)
                MainInstance.LoadPckFromFile(args.Where(arg => arg.EndsWith(".pck") && File.Exists(arg)));
            Application.ApplicationExit += (sender, e) => { RPC.Deinitialize(); };
            MainInstance.FocusMe();
            Application.Run(MainInstance);
        }


        internal static void UpdateToLatest()
        {
#if NDEBUG
            string url = $"{RawProjectUrl}/main/Version.json";
            AutoUpdater.Start(url);
#endif
        }

        class UpdateInfo
        {
            [JsonProperty("version")]
            public string Version { get; set; }
            
            [JsonProperty("url")]
            public string Url { get; set; }

            [JsonProperty("changelog")]
            public string Changelog { get; set; }
            
            [JsonProperty("mandatory")]
            public bool Mandatory { get; set; }
        }

        private static void AutoUpdaterOnParseUpdateInfoEvent(ParseUpdateInfoEventArgs args)
        {
            UpdateInfo json = JsonConvert.DeserializeObject<UpdateInfo>(args.RemoteData);
            args.UpdateInfo = new UpdateInfoEventArgs
            {
                CurrentVersion = json.Version,
                DownloadURL = json.Url,
                ChangelogURL = json.Changelog,
                Mandatory = new Mandatory()
                {
                    Value = json.Mandatory,
                }
            };
        }

    }
}