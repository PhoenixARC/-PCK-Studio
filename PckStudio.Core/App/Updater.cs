using System;
using System.IO;
using System.Windows.Forms;
using AutoUpdaterDotNET;
using Newtonsoft.Json;
using PckStudio.Core.Json;

namespace PckStudio.Core.App
{
    public static class Updater
    {
        private static Uri _appCast;
        private static bool _autoUpdate;

        public static void Initialize(Uri appCast, bool autoUpdate = false)
        {
            _appCast = appCast;
            _autoUpdate = autoUpdate;
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
            //AutoUpdater.Icon = Resources.ProjectLogo.ToBitmap();

            if (_autoUpdate)
            {
                UpdateToLatest();
            }
        }

        public static void SetOwner(Form owner) => AutoUpdater.SetOwner(owner);

        public static void UpdateToLatest()
        {
#if NDEBUG
            string url = $"{_appCast}/main/Version.json";
            AutoUpdater.Start(url);
#endif
        }

        private static void AutoUpdaterOnParseUpdateInfoEvent(ParseUpdateInfoEventArgs args)
        {
            UpdateInformation json = JsonConvert.DeserializeObject<UpdateInformation>(args.RemoteData);
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
