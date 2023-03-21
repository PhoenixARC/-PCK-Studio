using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using PckStudio.Classes.FileTypes;
using PckStudio.Classes.IO.ARC;
using PckStudio.Classes.IO.PCK;
using PckStudio.Classes.Misc;

namespace PckStudio.Forms.Additional_Features
{
    public partial class WiiUPanel : UserControl
    {
        string DLCPath = string.Empty;
        string mod = string.Empty;
        bool serverOn = false;
        ConsoleArchive archive = new ConsoleArchive();
        PCKFile currentPCK = null;

        private const string FtpUsername = "PCK_Studio_Client";
        // TODO: randomize per 'session'(instance)
        private const string FtpSessionPassword = "a3262443";
        private ICredentials sessionCredentials = new NetworkCredential(FtpUsername, FtpSessionPassword);

        public WiiUPanel()
        {
            InitializeComponent();
            UpdateDLCPath();
            buttonServerToggle.Enabled = true;
            if (listViewPCKS.Columns.Count == 0)
            {
                listViewPCKS.Columns.Add(DLCPath, listViewPCKS.Width);
            }
        }

        [Obsolete("Prompt user to use Aroma instead!")]
        private void buttonSelect_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Please use Aroma's ftp Plugin!");
            return;
        }

        private enum ButtonState
        {
            Start,
            Stop,
            Wait
        }

        private void SetButtonState(ButtonState state)
        {
            switch(state)
            {
                case ButtonState.Start:
                    buttonServerToggle.BackColor = Color.FromArgb(68, 178, 13);
                    serverOn = false;
                    buttonServerToggle.Text = "Start";
                    listViewPCKS.Enabled = false;
                    break;
                case ButtonState.Stop:
                    serverOn = true;
                    buttonServerToggle.BackColor = Color.Red;
                    buttonServerToggle.Text = "Stop";
                    listViewPCKS.Enabled = true;
                    break;
                case ButtonState.Wait:
                    buttonServerToggle.BackColor = Color.MediumAquamarine;
                    buttonServerToggle.Text = "Wait..";
                    break;
                default:
                    break;
            }
        }

        private string GetConsoleDevice()
        {
            if (radioButtonSystem.Checked)
            {
                return "storage_mlc";
            }
            if (radioButtonUSB.Checked)
            {
                return "storage_usb";
            }
            throw new Exception("how did you get here ?");
        }

        private string GetConsoleRegion()
        {
            if (radioButtonEur.Checked)
            {
                return "101d7500";
            }
            if (radioButtonUs.Checked)
            {
                return "101d9d00";
            }
            if (radioButtonJap.Checked)
            {
                return "101dbe00";
            }
            throw new Exception("how did you get here ?");
        }

        private string GetGameContentPath()
        {
            string device = GetConsoleDevice();
            string region = GetConsoleRegion();
            return $"{device}/usr/title/0005000e/{region}/content";
        }

        private void UpdateDLCPath()
        {
            DLCPath = $"{GetGameContentPath()}/WiiU/DLC/";
        }

        private void buttonServerToggle_Click(object sender, EventArgs e)
        {
            //Turn off server
            if (serverOn)
            {
                listViewPCKS.Items.Clear();
                try
                {
                    SetButtonState(ButtonState.Start);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
                return;
            }

            if (!Regex.IsMatch(IPv4TextBox.Text, @"^[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}$"))
            {
                MessageBox.Show("Please enter a valid Wii U IP!");
                return;
            }

            // Turn on server
            try
            {
                SetButtonState(ButtonState.Wait);

                ServicePointManager.Expect100Continue = true;

                using (var client = new FTPClient($"ftp://{IPv4TextBox.Text}", sessionCredentials))
                {
                    client.SetTimeoutLimit(TimeSpan.FromSeconds(10));
                    string[] dirList = client.ListDirectory(DLCPath);
                    listViewPCKS.Items.AddRange(dirList.Select(s => new ListViewItem(s)).ToArray());
                    foreach (ListViewItem pck in listViewPCKS.Items)
                    {
                        string[] res = client.ListDirectory($"{DLCPath}/{pck.Text}");
                        if (res.Length != 1)    
                        {
                            pck.Remove();
                        }
                    }
                }

                SetButtonState(ButtonState.Stop);
            }
            catch (Exception ex)
            {
                SetButtonState(ButtonState.Start);
                MessageBox.Show(ex.ToString());
            }   
        }

        private void radioButton_Click(object sender, EventArgs e)
        {
            UpdateDLCPath();
            listViewPCKS.Columns[0].Text = DLCPath;
        }

        private void listViewPCKS_MouseDown(object sender, MouseEventArgs e)
        {
            ListViewHitTestInfo hitTestInfo = listViewPCKS.HitTest(e.Location);
            if (e.Button == MouseButtons.Right && hitTestInfo.Location != ListViewHitTestLocations.None)
            {
                contextMenuStripCaffiine.Show(Cursor.Position);
            }
        }

        private void replaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listViewPCKS.SelectedItems.Count != 0)
            {
                SetButtonState(ButtonState.Wait);
                ReplacePck(mod);
                MessageBox.Show("PCK Replaced!");
            }
            SetButtonState(ButtonState.Stop);
            UpdateDLCPath();
        }

        private void replacePCKToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listViewPCKS.SelectedItems.Count != 0)
            {
                SetButtonState(ButtonState.Wait);
                OpenFileDialog openPCK = new OpenFileDialog();
                openPCK.Filter = "PCK File|*.pck";

                if (openPCK.ShowDialog() == DialogResult.OK)
                {
                    ReplacePck(openPCK.FileName);
                    MessageBox.Show("PCK Replaced!");
                }
            }
            SetButtonState(ButtonState.Stop);
            UpdateDLCPath();
        }

        private void ReplacePck(string filename)
        {
            using (FTPClient client = new FTPClient($"ftp://{IPv4TextBox.Text}", sessionCredentials))
                client.UploadFile(filename, DLCPath + "/" + listViewPCKS.SelectedItems[0].Text + "/" + listViewPCKS.SelectedItems[0].Tag.ToString());
            if (!string.IsNullOrWhiteSpace(TextBoxPackImage.Text))
            {
                string PackID = GetPackId(filename);
                GetARCFromConsole();
                AddOrReplacePackImage(PackID);
                SendARCToConsole();
            }
        }

        private string GetPackId(string filepath)
        {
            using (var fs = File.OpenRead(filepath))
            {
                currentPCK = PCKFileReader.Read(fs, false);
            }
            if (currentPCK is null) return string.Empty;
            return currentPCK.TryGetFile("0", PCKFile.FileData.FileType.InfoFile, out var file)
                ? file.Properties.GetProperty("PACKID").Item2
                : string.Empty;
        }

        private void GetARCFromConsole()
        {
            using var ms = new MemoryStream();
            using (FTPClient client = new FTPClient($"ftp://{IPv4TextBox.Text}", sessionCredentials))
            {
                client.DownloadFile(ms, GetGameContentPath() + "/Common/Media/MediaWiiU.arc");
                ms.Position = 0;
                archive = ARCFileReader.Read(ms);
            }
        }

        private void AddOrReplacePackImage(string packId)
        {
            string arcPath = $"Graphics\\PackGraphics\\{packId}.png";
            byte[] data = File.ReadAllBytes(TextBoxPackImage.Text);
            if (archive.ContainsKey(arcPath)) archive[arcPath] = data;
            else archive.Add(arcPath, data);
        }

        private void SendARCToConsole()
        {
            using (FTPClient client = new FTPClient($"ftp://{IPv4TextBox.Text}", sessionCredentials))
            {
                using (var ms = new MemoryStream())
                {
                    ARCFileWriter.Write(ms, archive);
                    ms.Position = 0;
                    client.UploadFile(ms, GetGameContentPath() + "/Common/Media/MediaWiiU.arc");
                }
                archive.Clear();
                currentPCK?.Files.Clear();
                currentPCK = null;
            }
            GC.Collect();
        }

        private void PackImageSelection_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Pack Image|*.png";
            if (ofd.ShowDialog() == DialogResult.OK)
                TextBoxPackImage.Text = ofd.FileName;
        }
    }
}
