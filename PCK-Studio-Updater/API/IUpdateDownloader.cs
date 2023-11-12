using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCKStudio_Updater
{
    public interface IUpdateDownloader
    {
        public bool IsUpdateAvailable(Version currentVersion);
        public bool IsUpdateAvailable(string currentVersionString);

        public void DownloadTo(DirectoryInfo directory);

        public void Launch();
    }
}
