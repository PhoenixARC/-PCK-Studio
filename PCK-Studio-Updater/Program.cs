using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Text.RegularExpressions;

namespace PCKStudio_Updater
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Uri projectUrl = new Uri("https://github.com/PhoenixARC/-PCK-Studio");
            if (args.Length > 0)
            {
                projectUrl = new Uri(args[0]);
            }
            
            string executableName = "PCK-Studio";
            if (args.Length > 1)
            {
                executableName = args[1];
            }

            bool prerelease = false;
            if (args.Length > 2)
            {
                prerelease = args[2].ToLower() == "true" || args[2].ToLower() == "1";
            }

            var versionMatcher = new Regex("(\\*|\\d+(\\.\\d+){0,3}(\\.\\*)?)");
            if (args.Length > 3)
            {
                versionMatcher = new Regex(args[3]);
            }

            GithubParams updateParams = new GithubParams(
                Path.GetDirectoryName(projectUrl.AbsolutePath).Replace("\\", ""),
                Path.GetFileName(projectUrl.AbsolutePath),
                executableName,
                prerelease,
                versionMatcher
            );

            IUpdateDownloader updater = new GithubUpdateDownloader(updateParams);

            if (!File.Exists(updateParams.TargetExecutableName + ".exe") || updater.IsUpdateAvailable(FileVersionInfo.GetVersionInfo(updateParams.TargetExecutableName + ".exe").ProductVersion))
            {
                updater.DownloadTo(new DirectoryInfo("."));
                updater.Launch();
                return;
            }
        }
    }
}
