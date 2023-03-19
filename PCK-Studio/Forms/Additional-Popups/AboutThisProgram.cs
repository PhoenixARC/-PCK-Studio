using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using Octokit;
using PckStudio.Classes.Extentions;
using PckStudio.ToolboxItems;

namespace PckStudio
{
    public partial class AboutThisProgram : ThemeForm
    {
        public AboutThisProgram()
        {
            InitializeComponent();
        }

        private async Task<User[]> AcquireDeveloperUserInfoAsync(params string[] usernames)
        {
            var githubClient = new GitHubClient(new ProductHeaderValue(ProductName));
            var result = new User[usernames.Length];
            foreach (var (i, name) in usernames.enumerate())
            {
                result[i] = await githubClient.User.Get(name);
            }
            return result;
        }

        private void AboutThisProgram_Load(object sender, EventArgs e)
        {
            Task.Run(async () =>
            {
                // TODO: check if avatar has changed and only acquire info once
                var devs = await AcquireDeveloperUserInfoAsync("PhoenixARC", "MattN-L", "EternalModz", "NessieHax");

                phoenixarcPictureBox.Image = ImageExtentions.ImageFromUrl(devs[0].AvatarUrl);
                phoenixarcGitHubButton.Click += (sender, e) => Process.Start(devs[0].HtmlUrl);

                mattNLPictureBox.Image = ImageExtentions.ImageFromUrl(devs[1].AvatarUrl);
                mattNLGitHubButton.Click += (sender, e) => Process.Start(devs[1].HtmlUrl);

                eternalModzPictureBox.Image = ImageExtentions.ImageFromUrl(devs[2].AvatarUrl);
                eternalModzGitHubButton.Click += (sender, e) => Process.Start(devs[2].HtmlUrl);

                mikuPictureBox.Image = ImageExtentions.ImageFromUrl(devs[3].AvatarUrl);
                mikuGitHubButton.Click += (sender, e) => Process.Start(devs[3].HtmlUrl);
            });
        }

        private void GitHubPageButton(object sender, EventArgs e)
        {
            Process.Start("https://github.com/PhoenixARC/-PCK-Studio");
        }

        private void WebsiteButton_Click(object sender, EventArgs e)
        {
            Process.Start("https://pckstudio.xyz/");
        }

        private void DiscordDevelopmentServerButton(object sender, EventArgs e)
        {
            Process.Start("https://discord.gg/dAepk3Bhud");
        }
    }
}
