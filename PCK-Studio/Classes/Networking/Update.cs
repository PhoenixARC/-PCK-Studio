using System;
using System.Diagnostics;
using System.Threading;
using System.IO;
using System.Net;
using System.Windows.Forms;

namespace PckStudio.Classes.Networking
{
    [Obsolete]
    public enum UpdateResult
    {
        // Base Failure value
        Failure = -1,
        // Base Success value
        Success,

        UpdateAvailable,

        UpdateFailure,
    }

    [Obsolete]
    class UpdateOptions
    {
        public bool IsBeta { get; set; }
        public bool IsPortable { get; set; }
        public string Domain
        {
            get => _baseDomain?.OriginalString ?? (_betaDomain?.OriginalString ?? throw new NullReferenceException(nameof(_betaDomain)));
            set
            {
                _ = value ?? throw new NullReferenceException(nameof(value));
                _baseDomain = new Uri(value);
            }
        }

        private Uri _baseDomain;
        private Uri _betaDomain;

        public UpdateOptions(bool isBeta, bool isPortable, Uri baseUri, Uri betaUri)
        {
            IsBeta = isBeta;
            IsPortable = isPortable;
            _baseDomain = baseUri;
            _betaDomain = betaUri;
        }
    }

    [Obsolete]
    static class Update
    {
        public static UpdateResult CheckForUpdate(UpdateOptions options)
        {
            // TODO: implement this
            return UpdateResult.Failure;
        }

        public static void UpdateProgram(UpdateOptions options)
        {
            string updateURL = options.Domain;
            if (options.IsPortable)
            {
                updateURL = updateURL.Replace(".msi","Portable.msi");
            }

            string downloadPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Temp\\";
            string destinationURL = options.Domain;
            if (TryDownloadFile(downloadPath + Path.GetFileName(destinationURL), destinationURL))
            {
                Process.Start(downloadPath + Path.GetFileName(destinationURL));
                Application.Exit();
            }
        }

        static bool TryDownloadFile(string filePath, string url)
        {
            try
            {
                using (WebClient client = new WebClient())
                    client.DownloadFile(url, filePath);
                return true;
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
            return false;
        }
    }
}
