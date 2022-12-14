using System;
using System.Diagnostics;
using System.Net;
using System.Windows.Forms;

namespace PckStudio.Classes.Networking
{
    class Network
    {
        public static string Version = "6.51";
        public static bool IsBeta = true;
        public static bool Portable = false;
        public static bool NeedsUpdate = false;
        public static Uri MainURL   = new Uri(Program.BaseAPIUrl);
        public static Uri BackUpURL = new Uri(Program.BackUpAPIUrl);
        static readonly string UpdatePath = "/update/Version";
        static readonly string BetaUpdatePath = "/update/VersionBeta";

        public static void CheckUpdate()
        {
            using WebClient wc = new WebClient();
            try
            {
                Uri versionUri = new Uri(MainURL, IsBeta ? BetaUpdatePath : UpdatePath);
                Console.WriteLine(versionUri);
                string serverVersion = wc.DownloadString(versionUri);
                if (Version != serverVersion)
                {
                    if (MessageBox.Show("An update is available! Do you want to update?" +
                        $"\nYour Version: {Version}" +
                        $"\nAvailable version: {serverVersion}", "Update Available", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        Update.UpdateProgram(new UpdateOptions(
                            isBeta: IsBeta,
                            isPortable: Portable,
                            baseUri: new Uri(MainURL, "/Update/Download/setup/PCKStudio-Setup.msi"),
                            betaUri: new Uri(MainURL, "/Update/Download/setup/beta/PCKStudioBeta-Setup.msi")
                            )
                            );
                    }
                    else
                    {
                        NeedsUpdate = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                MessageBox.Show("Can't connect to the server!", "Server unavailabe");
            }
        }


    }
}
