using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Compression;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Octokit;

namespace PCKStudio_Updater
{
    public sealed class GithubUpdateDownloader : IUpdateDownloader
    {
        private static readonly Assembly updaterAssembly = Assembly.GetAssembly(typeof(GithubUpdateDownloader));

        private readonly GithubParams _updateParams;
        private readonly GitHubClient githubClient;
        private Release latestFetchedRelease;
        private Version latestReleaseVersion;
        private DirectoryInfo downloadDirectory;


        public GithubUpdateDownloader(GithubParams updateParams)
        {
            _updateParams = updateParams;
            var githubClientProductHeader = new ProductHeaderValue(updaterAssembly.GetName().Name);
            githubClient = new GitHubClient(githubClientProductHeader);
        }

        public bool IsUpdateAvailable(FileVersionInfo fileVersionInfo)
        {
            return IsUpdateAvailable(fileVersionInfo.ProductVersion);
        }

        public bool IsUpdateAvailable(Assembly currentAssembly)
        {
            if (!File.Exists(currentAssembly.Location))
                return false;
            FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(currentAssembly.Location);
            return IsUpdateAvailable(fileVersionInfo.ProductVersion);
        }

        public bool IsUpdateAvailable(Version productVersion)
        {
            Debug.WriteLine("Release Product ver.: " + latestReleaseVersion);
            Debug.WriteLine("Current Product ver.: " + productVersion);
            return latestReleaseVersion.CompareTo(productVersion) > 0;
        }

        public bool IsUpdateAvailable(string productVersion)
        {
            GetLatestRelease(_updateParams.UsePreRelease);
            if (Version.TryParse(productVersion, out var currentVersion))
            {
                return IsUpdateAvailable(currentVersion);
            }
            return false;
        }

        private void UnpackZip(string zipFilePath)
        {
            ZipFile.ExtractToDirectory(zipFilePath, Path.GetDirectoryName(zipFilePath));
        }

        private static void DownloadAsset(ReleaseAsset asset, Stream destination)
        {
            string downloadUrl = asset.BrowserDownloadUrl;
            var client = new WebClient();
            using (var serverStream = client.OpenRead(downloadUrl))
            {
                serverStream.CopyTo(destination);
            }
        }

        private void GetLatestRelease(bool prerelease)
        {
            Release release;
            if (prerelease)
            {
                var prereleaseTask = githubClient.Repository.Release.GetAll(_updateParams.RepositoryOwnerName, _updateParams.RepositoryName);
                prereleaseTask.Wait();
                var prereleases = prereleaseTask.Result.OrderByDescending(release => release.PublishedAt ?? release.CreatedAt).Where(release => release.Prerelease).ToArray();
                release = latestFetchedRelease = prereleases[0];
            }
            else
            {
                var latestReleaseTask = githubClient.Repository.Release.GetLatest(_updateParams.RepositoryOwnerName, _updateParams.RepositoryName);
                latestReleaseTask.Wait();
                release = latestFetchedRelease = latestReleaseTask.Result;
            }
            var match = _updateParams.VersionMatcher.Match(release.Name);
            if (match.Success)
            {
                string versionString = match.Value;
                Version.TryParse(versionString, out latestReleaseVersion);
            }
        }

        private void EmptyDirectory(DirectoryInfo directory)
        {
            foreach (FileInfo file in directory.GetFiles())
                if (file.Name != _updateParams.TargetExecutableName + ".exe")
                file.Delete();
            foreach (DirectoryInfo subDirectory in directory.GetDirectories())
                subDirectory.Delete(true);
        }

        public void DownloadTo(DirectoryInfo directory)
        {
            if (latestFetchedRelease is null)
                GetLatestRelease(_updateParams.UsePreRelease);
            if (latestFetchedRelease.Assets?.Count > 0)
            {
                var asset = latestFetchedRelease.Assets[0];
                string zipFilePath = Path.Combine(directory.FullName, "update.zip");
                using(var zipFileStream = File.OpenWrite(zipFilePath))
                {
                    DownloadAsset(asset, zipFileStream);
                }
                Debug.WriteLine("Download Complete", category: nameof(GithubUpdateDownloader));
                EmptyDirectory(directory);
                UnpackZip(zipFilePath);
                File.Delete(zipFilePath);
                downloadDirectory = directory;
            }
        }

        public void Launch()
        {
            if (downloadDirectory is null)
            {
                throw new ArgumentNullException("Download directory not set.");
            }

            var files = downloadDirectory.GetFiles(_updateParams.TargetExecutableName + ".exe", SearchOption.TopDirectoryOnly);
            if (files is not null && files.Length > 0)
            {
                Process.Start(files[0].FullName);
            }
        }
    }
}
