using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Octokit;

namespace PCK_Studio_Publisher
{
    public partial class MainForm : Form
    {
        private const string AppName = "PCK-Studio.exe";
        private GitHubClient client;
        public MainForm()
        {
            InitializeComponent();
            
        }

        private async void publishReleaseButton_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
                return;

            if (string.IsNullOrWhiteSpace(ownerTextBox.Text))
            {
                MessageBox.Show("Owner name can not be empty.");
                return;
            }
            try
            {
                var user = await client.User.Get(ownerTextBox.Text);
            }
            catch(NotFoundException ex)
            {
                MessageBox.Show(ownerTextBox.Text + " not found.");
                Debug.WriteLine(ex.Message);
                return;
            }
            
            if (string.IsNullOrWhiteSpace(repoTextBox.Text))
            {
                MessageBox.Show("Repository name can not be empty.");
                return;
            }

            if (string.IsNullOrWhiteSpace(tagTextBox.Text))
            {
                MessageBox.Show("Tag name can not be empty.");
                return;
            }

            if (string.IsNullOrWhiteSpace(titleTextBox.Text))
            {
                MessageBox.Show("Title neeeded.");
                return;
            }

            if (string.IsNullOrWhiteSpace(assetOpenFileDialog.FileName) || !File.Exists(assetOpenFileDialog.FileName))
            {
                MessageBox.Show("Please attach an asset.");
                return;
            }

            var newRelease = new NewRelease(tagTextBox.Text)
            {
                Name = titleTextBox.Text,
                Body = bodyTextBox.Text,
                Prerelease = prereleaseRadioButton.Checked,
                GenerateReleaseNotes = false,
                TargetCommitish = null,
                Draft = false,
            };

            var release = await client.Repository.Release.Create(ownerTextBox.Text, repoTextBox.Text, newRelease);
            Debug.WriteLine("Release created.");

            using (var fs = File.OpenRead(assetOpenFileDialog.FileName))
            {
                ReleaseAssetUpload releaseAsset = new ReleaseAssetUpload()
                {
                    FileName = Path.GetFileName(assetOpenFileDialog.FileName),
                    ContentType = "application/zip",
                    RawData = fs
                };
                await client.Repository.Release.UploadAsset(release, releaseAsset);
                Debug.WriteLine("Asset uploaded.");
            }
        }

        private void brosweAssetButton_Click(object sender, EventArgs e)
        {
            assetOpenFileDialog.Reset();
            assetOpenFileDialog.Filter = "Zip|*.zip";
            assetOpenFileDialog.Title = "Selected package asset";
            if (assetOpenFileDialog.ShowDialog() == DialogResult.OK)
            {
                var zipArchive = ZipFile.OpenRead(assetOpenFileDialog.FileName);
                var appEntry = zipArchive.GetEntry(AppName);
                if (appEntry is not null)
                {
                    string path = Path.Combine(System.Windows.Forms.Application.StartupPath, AppName);
                    appEntry.ExtractToFile(path, true);
                    var fileVersionInfo = FileVersionInfo.GetVersionInfo(path);
                    tagTextBox.Text = "v" + fileVersionInfo.ProductVersion;
                    titleTextBox.Text = Path.GetFileNameWithoutExtension(AppName) + " - v" + fileVersionInfo.ProductVersion;
                    assetLabel.Text = Path.GetFileName(assetOpenFileDialog.FileName);
                    releaseInfoGroupBox.Enabled = true;
                    File.Delete(path);
                }
            }
        }

        private async void accessTokenTextBox_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                client = new GitHubClient(new ProductHeaderValue("PCK-Studio-Publisher"));
                client.Credentials = new Credentials(accessTokenTextBox.Text);
                accessTokenTextBox.Enabled = false;
                var currentUser = await client.User.Current();
                currentUserLoginLabel.Text = currentUser.Login;
                currentUserNameLabel.Text = currentUser.Name;
                repositoryGroupBox.Enabled = true;
                assetGroupBox.Enabled = true;
            }
        }
    }
}
