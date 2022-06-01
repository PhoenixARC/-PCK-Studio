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
        public static string Appdata = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/PCK Studio/";
        public static bool IsDev;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            IsDev = args.Length > 0 && args[0] == "-dev";
            Application.Run(new PckStudio.FormMain());
        }
    }
}
