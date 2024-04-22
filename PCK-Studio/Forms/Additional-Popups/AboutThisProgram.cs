using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Octokit;
using PckStudio.Extensions;
using PckStudio.ToolboxItems;

namespace PckStudio
{
    public partial class AboutThisProgram : ThemeForm
    {
        public AboutThisProgram()
        {
            InitializeComponent();
            // Subscribe to the KeyDown event
            this.KeyDown += MainForm_KeyDown;
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

                phoenixarcPictureBox.SetPropertyThreadSafe(() => phoenixarcPictureBox.Image, ImageExtensions.ImageFromUrl(devs[0].AvatarUrl));
                phoenixarcPictureBox.SetPropertyThreadSafe(() => phoenixarcPictureBox.Text, devs[0].Name);
                phoenixarcGitHubButton.Click += (sender, e) => Process.Start(devs[0].HtmlUrl);

                mattNLPictureBox.SetPropertyThreadSafe(() => mattNLPictureBox.Image, ImageExtensions.ImageFromUrl(devs[1].AvatarUrl));
                mattNLPictureBox.SetPropertyThreadSafe(() => mattNLPictureBox.Text, devs[1].Name);
                mattNLGitHubButton.Click += (sender, e) => Process.Start(devs[1].HtmlUrl);

                eternalModzPictureBox.SetPropertyThreadSafe(() => eternalModzPictureBox.Image, ImageExtensions.ImageFromUrl(devs[2].AvatarUrl));
                eternalModzPictureBox.SetPropertyThreadSafe(() => eternalModzPictureBox.Text, devs[2].Name);
                eternalModzGitHubButton.Click += (sender, e) => Process.Start(devs[2].HtmlUrl);

                mikuPictureBox.SetPropertyThreadSafe(() => mikuPictureBox.Image, ImageExtensions.ImageFromUrl(devs[3].AvatarUrl));
                mikuPictureBox.SetPropertyThreadSafe(() => mikuLabel.Text, devs[3].Name);
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

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            // Check if Ctrl + W is pressed
            if (e.Control && e.KeyCode == Keys.W)
            {
                // Close the application
                this.Close();
            }
        }
    }
}
