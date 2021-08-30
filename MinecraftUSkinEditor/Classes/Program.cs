using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using PckStudio.Classes;

namespace PckStudio
{


    static class Program
    {
        public static string baseurl = "http://www.pckstudio.tk/studio/PCK/api/";
        public static string backurl = "https://phoenixarc.github.io/pckstudio.tk/studio/PCK/api/";
        public static bool IsDev = false;
        public static FormMain formMain;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            try
            {
                if (args[0] == "-dev")
                    IsDev = true;
            }
            catch { }
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            PckStudio.Forms.goodbye gg = new PckStudio.Forms.goodbye();
            PckStudio.Forms.Job gj = new PckStudio.Forms.Job();


            if(!System.IO.File.Exists(Environment.CurrentDirectory + "\\goodbyemark"))
            gg.ShowDialog();
            if(!System.IO.File.Exists(Environment.CurrentDirectory + "\\discordmark"))
            gj.ShowDialog();
            Application.Run(new FormMain());
        }
    }
}
