using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using OMI.Formats.Languages;
using PckStudio.Core;
using PckStudio.Core.App;
using PckStudio.Core.DLC;
using PckStudio.Forms.Additional_Popups;
using PckStudio.Internal;
using PckStudio.Internal.App;
using PckStudio.Properties;


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
            Updater.Initialize(RawProjectUrl, Settings.Default.AutoUpdate);
            if (Settings.Default.Platform == Core.ConsolePlatform.Unknown)
            {
                MessageBox.Show("Please choose on which console you're playing on.", "Select Platform", MessageBoxButtons.OK, MessageBoxIcon.Question);
                var platformChooser = new ItemSelectionPopUp(Enum.GetNames(typeof(Core.ConsolePlatform)).Skip(1).ToArray());
                if (platformChooser.ShowDialog() == DialogResult.OK && Enum.IsDefined(typeof(ConsolePlatform), platformChooser.SelectedItem))
                    Settings.Default.Platform = (Core.ConsolePlatform)Enum.Parse(typeof(ConsolePlatform), platformChooser.SelectedItem);
            }

            ApplicationScope.Initialize();
            Trace.TraceInformation("Startup");
            RPC.Initialize();
            MainInstance = new MainForm();
            Updater.SetOwner(MainInstance);
            if (args.Length > 0)
            {
                MainInstance.LoadPckFromFile(args.Where(arg => File.Exists(arg) && arg.EndsWith(".pck")));
            }
            Application.ApplicationExit += (sender, e) => { RPC.Deinitialize(); };
            MainInstance.Focus();
            Application.Run(MainInstance);
        }
    }
}