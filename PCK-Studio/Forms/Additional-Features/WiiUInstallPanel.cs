using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Compression;
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
    public partial class WiiUInstallPanel : UserControl
    {
        string DLCPath = "";
        string mod = "";
        bool serverOn = false;
        ConsoleArchive archive = new ConsoleArchive();
        PCKFile currentPCK = null;

        private const string FtpUsername = "PCK_Studio_Client";
        // TODO: randomize per 'session'(instance)
        private const string FtpSessionPassword = "a3262443";
        private ICredentials sessionCredentials = new NetworkCredential(FtpUsername, FtpSessionPassword);

        public WiiUInstallPanel()
        {
            InitializeComponent();
            UpdateDLCPath();
        }

        private readonly struct PckDir
        {
            public PckDir(string folder, string file)
            {
                FolderName = folder;
                FileName = file;
            }

            public string FolderName { get; }
            public string FileName { get; }
        }

        // TODO: find better name   
        private static List<PckDir> pcks = new List<PckDir>()
        {
            new PckDir(folder: "Battle & Beasts", file: "BattleAndBeasts.pck"),
            new PckDir(folder: "Battle & Beasts 2", file: "BattleAndBeasts2.pck"),
            new PckDir(folder: "Biome Settlers Pack 1", file: "SkinsBiomeSettlers1.pck"),
            new PckDir(folder: "Biome Settlers Pack 2", file: "SkinsBiomeSettlers2.pck"),
            //new pckDir() { folder = "Campfire Tales Skin Pack", file = "" },
            new PckDir(folder: "Doctor Who Skins Volume I", file: "SkinPackDrWho.pck"),
            new PckDir(folder: "Doctor Who Skins Volume II", file: "SkinPackDrWho.pck"),
            new PckDir(folder: "Festive Skin Pack", file: "SkinsFestive.pck"),
            new PckDir(folder: "FINAL FANTASY XV Skin Pack", file: "FinalFantasyXV.pck"),
            new PckDir(folder: "Magic The Gathering Skin Pack", file: "magicthegathering.pck"),
            new PckDir(folder: "Mini Game Heroes Skin Pack", file: "Minigame2.pck"),
            new PckDir(folder: "Mini Game Masters Skin Pack", file: "Minigame.pck"),
            new PckDir(folder: "Moana Character Pack", file: "Moana.pck"),
            new PckDir(folder: "Power Rangers Skin Pack", file: "PowerRangers.pck"),
            new PckDir(folder: "Redstone Specialists Skin Pack", file: "SkinsRedstoneSpecialists.pck"),
            new PckDir(folder: "Skin Pack 1", file: "Skins1.pck"),
            new PckDir(folder: "Star Wars Classic Skin Pack", file: "StarWarsClassicPack.pck"),
            new PckDir(folder: "Star Wars Prequel Skin Pack", file: "StarWarsPrequel.pck"),
            new PckDir(folder: "Star Wars Rebels Skin Pack", file: "StarWarsRebelsPack.pck"),
            new PckDir(folder: "Star Wars Sequel Skin Pack", file: "StarWarsSequel.pck"),
            new PckDir(folder: "Story Mode Skin Pack", file: "PackStoryMode.pck"),
            new PckDir(folder: "Stranger Things Skin Pack", file: "StrangerThings.pck"),
            new PckDir(folder: "Strangers Biome Settlers 3 Skin Pack", file: "BiomeSettlers3_Strangers.pck"),
            new PckDir(folder: "The Incredibles Skin Pack", file: "Incredibles.pck"),
            new PckDir(folder: "The Simpsons Skin Pack", file: "SkinPackSimpsons.pck"),
            new PckDir(folder: "Villains Skin Pack", file: "Villains.pck"),
        };

        [Obsolete("Prompt user to use Aroma instead!")]
        private void buttonSelect_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Please use Aroma's ftp Plugin!");
            return;
            FolderBrowserDialog sdFind = new FolderBrowserDialog();
            if (sdFind.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    string sdRoot = Directory.GetDirectoryRoot(sdFind.SelectedPath);
                    Directory.CreateDirectory(sdRoot + "/wiiu/apps/");

                    try
                    {
                        File.WriteAllBytes(sdRoot + "/wiiu/apps/apps.zip", Properties.Resources.apps);
                    }
                    catch
                    {
                        MessageBox.Show($"Could not extract resources to:\n{sdRoot}/wiiu/apps/apps.zip");
                        return;
                    }

                    string zipPath = sdRoot + "/wiiu/apps/apps.zip";
                    string extractPath = sdRoot + "/wiiu/apps/temp";
                    ZipFile.ExtractToDirectory(zipPath, extractPath);

                    if (!Directory.Exists(sdRoot + "/wiiu/apps/ftpiiu_everywhere"))
                    {
                        Directory.Move(sdRoot + "/wiiu/apps/temp/ftpiiu_everywhere", sdRoot + "/wiiu/apps/ftpiiu_everywhere");
                    }

                    if (!Directory.Exists(sdRoot + "/wiiu/apps/homebrew_launcher"))
                    {
                        Directory.Move(sdRoot + "/wiiu/apps/temp/homebrew_launcher", sdRoot + "/wiiu/apps/homebrew_launcher");
                    }

                    if (!Directory.Exists(sdRoot + "/wiiu/apps/mocha_fshax"))
                    {
                        Directory.Move(sdRoot + "/wiiu/apps/temp/mocha_fshax", sdRoot + "/wiiu/apps/mocha_fshax");
                    }

                    if (!File.Exists(sdRoot + "/wiiu/apps/sign_c2w_patcher.elf"))
                    {
                        File.Move(sdRoot + "/wiiu/apps/temp/sign_c2w_patcher.elf", sdRoot + "/wiiu/apps/sign_c2w_patcher.elf");
                    }

                    File.Delete(sdRoot + "/wiiu/apps/apps.zip");
                    Directory.Delete(sdRoot + "/wiiu/apps/temp/", true);
                }
                catch (Exception er)
                {
                    MessageBox.Show(er.ToString());
                }
                MessageBox.Show("Done");
            }
        }

        private enum ButtonState
        {
            Start,
            Stop,
            Loading
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
                case ButtonState.Loading:
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
            buttonServerToggle.Enabled = true;
            if (listViewPCKS.Columns.Count == 0)
            {
                listViewPCKS.Columns.Add(DLCPath, listViewPCKS.Width);
            }
            else
            {
                listViewPCKS.Columns[0].Text = DLCPath;
            }
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

            //Makes sure user typed in their ip
            if (!Regex.IsMatch(textBoxHost.Text, @"^[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}$"))
            {
                MessageBox.Show("Please enter a valid Wii U IP!");
                return;
            }

            // Turn on server
            try
            {
                SetButtonState(ButtonState.Loading);

                ServicePointManager.Expect100Continue = true;

                using (var client = new FTPClient($"ftp://{textBoxHost.Text}", sessionCredentials))
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
                SetButtonState(ButtonState.Loading);
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
                SetButtonState(ButtonState.Loading);
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
            using (FTPClient client = new FTPClient($"ftp://{textBoxHost.Text}", sessionCredentials))
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
                ? file.properties.GetProperty("PACKID").Item2
                : string.Empty;
        }

        private void GetARCFromConsole()
        {
            using var ms = new MemoryStream();
            using (FTPClient client = new FTPClient($"ftp://{textBoxHost.Text}", sessionCredentials))
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
            using (FTPClient client = new FTPClient($"ftp://{textBoxHost.Text}", sessionCredentials))
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
            ofd.Filter = "PNG Image|*.png";
            if (ofd.ShowDialog() == DialogResult.OK)
                TextBoxPackImage.Text = ofd.FileName;
        }
    }
}
