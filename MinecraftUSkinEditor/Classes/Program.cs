using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;

namespace MinecraftUSkinEditor
{


    static class Program
    {
        public static string baseurl = "http://www.pckstudio.tk/";

        public static FormMain formMain;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            System.Globalization.CultureInfo ci = new System.Globalization.CultureInfo("ja");
            Thread.CurrentThread.CurrentCulture = ci;
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            minekampf.Forms.goodbye gg = new minekampf.Forms.goodbye();
            if(!System.IO.File.Exists(Environment.CurrentDirectory + "\\goodbyemark"))
            gg.ShowDialog();
            Application.Run(new FormMain());
        }
    }
}
