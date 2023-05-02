using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace PckStudio
{
    sealed class ProgramInfo
    {
        // this is to specify which build release this is. This is manually updated for now
        // TODO: add different chars for different configurations
        private const string BuildType = "b";
        private static System.Globalization.Calendar _buildCalendar;
        private DateTime date = new FileInfo(Assembly.GetExecutingAssembly().Location).LastWriteTime;

        public string BetaBuildVersion
        {
            get
            {
                // adopted Minecraft Java Edition Snapshot format (YYwWWn)
                // to keep better track of work in progress features and builds
                _buildCalendar ??= new System.Globalization.CultureInfo("en-US").Calendar;
                return string.Format("#{0}w{1}{2}",
                    date.ToString("yy"),
                    _buildCalendar.GetWeekOfYear(date, System.Globalization.CalendarWeekRule.FirstDay, DayOfWeek.Monday),
                    BuildType);
            }
        }
    }

    static class CommitInfo
    {
        private static string _branchName = null;
        private static string _commitHash = null;

        public static string BranchName
        {
            get
            {
                return _branchName ??= Assembly
                        .GetEntryAssembly()
                        .GetCustomAttributes<AssemblyMetadataAttribute>()
                        .FirstOrDefault(attr => attr.Key == "GitBranch")?.Value;
            }
        }

        public static string CommitHash
        {
            get
            {
                return _commitHash ??= Assembly
                        .GetEntryAssembly()
                        .GetCustomAttributes<AssemblyMetadataAttribute>()
                        .FirstOrDefault(attr => attr.Key == "GitHash")?.Value;
            }
        }
    }

    static class Program
    {
        public static readonly string ProjectUrl = "https://github.com/PhoenixARC/-PCK-Studio";
        public static readonly string BaseAPIUrl = "http://api.pckstudio.xyz/api/pck";
        public static readonly string BackUpAPIUrl = "https://raw.githubusercontent.com/PhoenixARC/pckstudio.tk/main/studio/PCK/api/";
        public static readonly string AppData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "PCK-Studio");
        public static readonly string AppDataCache = Path.Combine(AppData, "cache");

        public static readonly ProgramInfo Info = new ProgramInfo();

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            System.Globalization.CultureInfo.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
            
            var mainForm = new MainForm();
            if (args.Length > 0 && File.Exists(args[0]) && args[0].EndsWith(".pck"))
                mainForm.LoadPck(args[0]);
            Application.Run(mainForm);
        }
    }
}
