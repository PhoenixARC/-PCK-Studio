using System;
using System.Net;
using System.Windows.Forms;

namespace PckStudio.Classes
{
    class Network
    {
        public static string Version = "6.4";
        public static bool Beta = true;
        public static bool Portable = false;
        public static bool NeedsUpdate = false;
        public static string MainURL = "https://www.pckstudio.xyz/";
        public static string BackURL = "http://phoenixarc.ddns.net/";
        static string UpdateURL = "studio/PCK/api/updatePCKStudio.txt";
        static string BetaUpdateURL = "studio/PCK/api/updatePCKStudioB.txt";

        public static void CheckUpdate()
        {
            WebClient wc = new WebClient();
            string docuDir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            try
            {
                Console.WriteLine(MainURL + UpdateURL);
                switch (Beta)
                {
                    case false:
                        if (float.Parse(Version) < float.Parse(wc.DownloadString(MainURL + UpdateURL)))
                        {
                            if (MessageBox.Show("An update is available! do you want to update?\nYour Version:" + Version + "\nAvailable version:" + wc.DownloadString(MainURL + UpdateURL), "Update Available", MessageBoxButtons.YesNo) == DialogResult.Yes)
                            {
                                Classes.Update.UpdateProgram(Beta);
                            }
                            else
                            {
                                NeedsUpdate = true;
                            }
                        }
                        break;
                    case true:
                        if (float.Parse(Version) < float.Parse(wc.DownloadString(MainURL + BetaUpdateURL)))
                        {
                            if (MessageBox.Show("An update is available! do you want to update?\nYour Version:" + Version + "\nAvailable version:" + wc.DownloadString(MainURL + BetaUpdateURL), "Update Available", MessageBoxButtons.YesNo) == DialogResult.Yes)
                            {
                                Classes.Update.UpdateProgram(Beta);
                            }
                            else
                            {
                                NeedsUpdate = true;
                            }
                        }
                        break;
                }
            }
            catch
            {
                try
                {
                    switch (Beta)
                    {
                        case false:
                            if (float.Parse(Version) < float.Parse(wc.DownloadString(BackURL + UpdateURL)))
                            {
                                if (MessageBox.Show("An update is available! do you want to update?\nYour Version:" + Version + "\nAvailable version:" + wc.DownloadString(BackURL + UpdateURL), "Update Available", MessageBoxButtons.YesNo) == DialogResult.Yes)
                                {
                                    Classes.Update.UpdateProgram(Beta);
                                }
                                else
                                {
                                    NeedsUpdate = true;
                                }
                            }
                            break;
                        case true:
                            if (float.Parse(Version) < float.Parse(wc.DownloadString(BackURL + BetaUpdateURL)))
                            {
                                if (MessageBox.Show("An update is available! do you want to update?\nYour Version:" + Version + "\nAvailable version:" + wc.DownloadString(BackURL + UpdateURL), "Update Available", MessageBoxButtons.YesNo) == DialogResult.Yes)
                                {
                                    Classes.Update.UpdateProgram(Beta);
                                }
                                else
                                {
                                    NeedsUpdate = true;
                                }
                            }
                            break;
                    }
                }
                catch(Exception ex)
                {
                    MessageBox.Show("Server unavailabe", "Cannot connect to the server!");
                }
            }
        }


    }
}
