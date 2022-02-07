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
        public static string baseurl = "https://www.pckstudio.xyz/studio/PCK/api/";
        public static string backurl = "https://raw.githubusercontent.com/PhoenixARC/pckstudio.tk/main/studio/PCK/api/";
        public static string Appdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
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

            PckStudio.Forms.goodbye gg = new PckStudio.Forms.goodbye();
            PckStudio.Forms.Job gj = new PckStudio.Forms.Job();


            if(!System.IO.File.Exists(Appdata + "\\goodbyemark"))
            gg.ShowDialog();
            if(!System.IO.File.Exists(Appdata + "\\discordmark"))
            gj.ShowDialog();
            Application.Run(new PckStudio.FormMain());
        }
    }
}
