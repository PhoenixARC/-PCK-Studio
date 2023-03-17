using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Dark.Net;

namespace PckStudio
{
    sealed class ProgramInfo
    {
        // this is to specify which build release this is. This is manually updated for now
        // TODO: add different chars for different configurations
        private const string BuildType = "b";
        private System.Globalization.Calendar BuildCalendar = new System.Globalization.CultureInfo("en-US").Calendar;
        private DateTime date = new FileInfo(Assembly.GetExecutingAssembly().Location).LastWriteTime;

        public string BuildVersion
        {
            get
            {
                // adopted Minecraft Java Edition Snapshot format (YYwWWn)
                // to keep better track of work in progress features and builds
                return string.Format("(Debug build #{0}w{1}{2})",
                    date.ToString("yy"),
                    BuildCalendar.GetWeekOfYear(date, System.Globalization.CalendarWeekRule.FirstDay, DayOfWeek.Monday),
                    BuildType);
            }
        }

        public string LastCommitHash =>
            Assembly
                .GetEntryAssembly()
                .GetCustomAttributes<AssemblyMetadataAttribute>()
                .FirstOrDefault(attr => attr.Key == "GitHash")?.Value;
    }

    static class Program
    {
        public static readonly string BaseAPIUrl = "http://api.pckstudio.xyz/api/pck";
        public static readonly string BackUpAPIUrl = "https://raw.githubusercontent.com/PhoenixARC/pckstudio.tk/main/studio/PCK/api/";
        public static readonly string AppData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "PCK-Studio");
        public static readonly string AppDataCache = Path.Combine(AppData, "cache");

        public static readonly ProgramInfo Info = new ProgramInfo();

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main(string[] args)
        {
            System.Globalization.CultureInfo.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;

            MainForm mainForm = new MainForm();
            DarkNet.Instance.SetWindowThemeForms(mainForm, Theme.Auto);
            if (args.Length > 0 && File.Exists(args[0]) && args[0].EndsWith(".pck"))
                mainForm.LoadPck(args[0]);
            Application.Run(mainForm);
        }
    }
}
