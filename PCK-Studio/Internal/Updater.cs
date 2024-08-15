using System;
using System.IO;
using System.Windows.Forms;
using AutoUpdaterDotNET;
using Newtonsoft.Json;
using PckStudio.Internal.Json;
using PckStudio.Properties;

namespace PckStudio.Internal
{
    internal static class Updater
    {
        private static Uri _appCast;

        internal static void Initialize(Uri appCast)
        {
            _appCast = appCast;
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
        }

        internal static void SetOwner(Form owner) => AutoUpdater.SetOwner(owner);

        internal static void UpdateToLatest()
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
