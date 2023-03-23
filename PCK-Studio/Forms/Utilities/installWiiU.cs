using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Windows.Forms;
using System.Diagnostics;

using PckStudio.Classes.FileTypes;
using PckStudio.Classes.IO.PCK;
using PckStudio.Classes.Misc;
using OMI.Formats.Archive;
using OMI.Workers.Archive;
using OMI.Workers.Pck;
using OMI.Formats.Pck;

namespace PckStudio.Forms
{
    public partial class installWiiU : MetroFramework.Forms.MetroForm
    {
        string loca = "";
        string dlcPath = "";
        string mod = "";
        bool serverOn = false;
        ConsoleArchive archive = new ConsoleArchive();

        public installWiiU(string mod)
        {
            InitializeComponent();
            
            this.mod = mod;

            if (mod == null)
            {
                replaceToolStripMenuItem.Visible = false;
            }
            else
            {
                replaceToolStripMenuItem.Text = "Replace with " + Path.GetFileName(mod);
            }
        }

        //items class for use in bedrock skin conversion
        public class pckDir
        {
            public string folder { get; set; }
            public string file { get; set; }
        }

        private void buttonSelect_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog sdFind = new FolderBrowserDialog();
            if (sdFind.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    string sdRoot = Directory.GetDirectoryRoot(sdFind.SelectedPath);

                    if (!Directory.Exists(sdRoot + "/wiiu/apps/"))
                    {
                        Directory.CreateDirectory(sdRoot + "/wiiu/apps/");
                    }

                    using (WebClient client = new WebClient())
                    {
                        try
                        {
                            File.WriteAllBytes(sdRoot + "/wiiu/apps/apps.zip", PckStudio.Properties.Resources.apps);
                        }
                        catch
                        {
                            MessageBox.Show("Could not extract resources to:\n" + sdRoot + "/wiiu/apps/apps.zip");
                            return;
                        }
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
        List<pckDir> pcks = new List<pckDir>();
        PckFile currentPCK = null;

        private void updateDatabase()
        {
            pcks.Clear();
            pcks.Add(new pckDir() { folder = "Battle & Beasts", file = "BattleAndBeasts.pck" });
            pcks.Add(new pckDir() { folder = "Battle & Beasts 2", file = "BattleAndBeasts2.pck" });
            pcks.Add(new pckDir() { folder = "Biome Settlers Pack 1", file = "SkinsBiomeSettlers1.pck" });
            pcks.Add(new pckDir() { folder = "Biome Settlers Pack 2", file = "SkinsBiomeSettlers2.pck" });
            //pcks.Add(new pckDir() { folder = "Campfire Tales Skin Pack", file = "" });
            pcks.Add(new pckDir() { folder = "Doctor Who Skins Volume I", file = "SkinPackDrWho.pck" });
            pcks.Add(new pckDir() { folder = "Doctor Who Skins Volume II", file = "SkinPackDrWho.pck" });
            pcks.Add(new pckDir() { folder = "Festive Skin Pack", file = "SkinsFestive.pck" });
            pcks.Add(new pckDir() { folder = "FINAL FANTASY XV Skin Pack", file = "FinalFantasyXV.pck" });
            pcks.Add(new pckDir() { folder = "Magic The Gathering Skin Pack", file = "magicthegathering.pck" });
            pcks.Add(new pckDir() { folder = "Mini Game Heroes Skin Pack", file = "Minigame2.pck" });
            pcks.Add(new pckDir() { folder = "Mini Game Masters Skin Pack", file = "Minigame.pck" });
            pcks.Add(new pckDir() { folder = "Moana Character Pack", file = "Moana.pck" });
            pcks.Add(new pckDir() { folder = "Power Rangers Skin Pack", file = "PowerRangers.pck" });
            pcks.Add(new pckDir() { folder = "Redstone Specialists Skin Pack", file = "SkinsRedstoneSpecialists.pck" });
            pcks.Add(new pckDir() { folder = "Skin Pack 1", file = "Skins1.pck" });
            pcks.Add(new pckDir() { folder = "Star Wars Classic Skin Pack", file = "StarWarsClassicPack.pck" });
            pcks.Add(new pckDir() { folder = "Star Wars Prequel Skin Pack", file = "StarWarsPrequel.pck" });
            pcks.Add(new pckDir() { folder = "Star Wars Rebels Skin Pack", file = "StarWarsRebelsPack.pck" });
            pcks.Add(new pckDir() { folder = "Star Wars Sequel Skin Pack", file = "StarWarsSequel.pck" });
            pcks.Add(new pckDir() { folder = "Story Mode Skin Pack", file = "PackStoryMode.pck" });
            pcks.Add(new pckDir() { folder = "Stranger Things Skin Pack", file = "StrangerThings.pck" });
            pcks.Add(new pckDir() { folder = "Strangers Biome Settlers 3 Skin Pack", file = "BiomeSettlers3_Strangers.pck" });
            pcks.Add(new pckDir() { folder = "The Incredibles Skin Pack", file = "Incredibles.pck" });
            pcks.Add(new pckDir() { folder = "The Simpsons Skin Pack", file = "SkinPackSimpsons.pck" });
            pcks.Add(new pckDir() { folder = "Villains Skin Pack", file = "Villains.pck" });
        }
        
        public void buttonMode(string mode)
        {
            if (mode == "start")
            {
                buttonServerToggle.BackColor = Color.FromArgb(68, 178, 13);
                serverOn = false;
                buttonServerToggle.Text = "Start";
                listViewPCKS.Enabled = false;
            }
            else if (mode == "stop")
            {
                serverOn = true;
                buttonServerToggle.BackColor = Color.Red;
                buttonServerToggle.Text = "Stop";
                listViewPCKS.Enabled = true;
            }
            else if (mode == "loading")
            {
                buttonServerToggle.BackColor = Color.MediumAquamarine;
                buttonServerToggle.Text = "Wait..";
            }
        }

        private void loadPcks()
        {
            string region = "";
            if (radioButtonEur.Checked)
            {
                region = "101d7500";
            }
            else if (radioButtonUs.Checked)
            {
                region = "101d9d00";
            }
            else if (radioButtonJap.Checked)
            {
                region = "101dbe00";
            }

            string device = "";
            if (radioButtonSystem.Checked)
            {
                device = "storage_mlc";
            }
            else if (radioButtonUSB.Checked)
            {
                device = "storage_usb";
            }

            if (region != "" && device != "")
            {
                dlcPath = device + "/usr/title/0005000e/" + region + "/content/WiiU/DLC/";
                buttonServerToggle.Enabled = true;
                if (listViewPCKS.Columns.Count == 0)
                {
                    listViewPCKS.Columns.Add(dlcPath, 395);
                }
            }
        }

        private void buttonServerToggle_Click(object sender, EventArgs e)
        {
            if (serverOn == false)
            {
                //Makes sure user typed in their ip
                if (textBoxHost.Text == "")
                {
                    MessageBox.Show("Please enter a valid Wii U IP!");
                    return;
                }

                //Turns Server On
                try
                {
                    buttonMode("loading");

                    ServicePointManager.Expect100Continue = true;

                    //ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(OnValidateCertificate);
                    FtpWebRequest request = (FtpWebRequest)WebRequest.Create("ftp://" + textBoxHost.Text + "/" + dlcPath);
                    request.Method = WebRequestMethods.Ftp.ListDirectory;
                    request.Credentials = new NetworkCredential("", "a3262443");
                    request.EnableSsl = false;
                    request.Timeout = 1200000;

                    ServicePoint sp = request.ServicePoint;
                    Debug.WriteLine("ServicePoint connections = {0}.", sp.ConnectionLimit);
                    sp.ConnectionLimit = 1;

                    using (var response = (FtpWebResponse)request.GetResponse())
                    {
                        using (var stream = response.GetResponseStream())
                        {
                            using (var reader = new StreamReader(stream, true))
                            {
                                string line = reader.ReadLine();
                                while (line != null)
                                {
                                    listViewPCKS.Items.Add(line);
                                    line = reader.ReadLine();
                                }
                            }
                        }
                    }

                    foreach (ListViewItem pck in listViewPCKS.Items)
                    {
                        int i = 0;
                        FtpWebRequest request2 = (FtpWebRequest)WebRequest.Create("ftp://" + textBoxHost.Text + "/" + dlcPath + "/" + pck.Text + "/");
                        request2.Method = WebRequestMethods.Ftp.ListDirectory;
                        request2.Credentials = new NetworkCredential("", "a3262443");
                        request2.EnableSsl = false;
                        request2.Timeout = 1200000;

                        ServicePoint sp2 = request2.ServicePoint;
                        Console.WriteLine("NOBLEDEZ//PHOENIXARC WAS HERE", sp2.ConnectionLimit);
                        sp2.ConnectionLimit = 1;

                        using (var response = (FtpWebResponse)request2.GetResponse())
                        {
                            using (var stream = response.GetResponseStream())
                            {
                                using (var reader = new StreamReader(stream, true))
                                {
                                    string line = reader.ReadLine();
                                    while (line != null)
                                    {
                                        i += 1;
                                        pck.Tag = line;
                                        line = reader.ReadLine();
                                    }
                                }
                            }
                        }
                        if (i != 1)
                        {
                            pck.Remove();
                        }
                    }

                    buttonMode("stop");
                }
                catch (Exception disc)
                {
                    buttonMode("start");
                    MessageBox.Show(disc.ToString());
                }
            }
            else if (serverOn == true)
            {
                //Turns Server Off
                listViewPCKS.Items.Clear();
                try
                {
                    buttonMode("start");
                }
                catch (Exception disc)
                {
                    MessageBox.Show(disc.ToString());
                }
            }
        }

        private void radioButtonEur_Click(object sender, EventArgs e)
        {
            loadPcks();
        }

        private void radioButtonUs_Click(object sender, EventArgs e)
        {
            loadPcks();
        }

        private void radioButtonJap_Click(object sender, EventArgs e)
        {
            loadPcks();
        }

        private void listViewPCKS_Click(object sender, EventArgs e)
        {

        }

        private void listViewPCKS_MouseDown(object sender, MouseEventArgs e)
        {
            ListViewHitTestInfo hitTestInfo = listViewPCKS.HitTest(e.Location);
            if (e.Button == MouseButtons.Right)
            {
                if (hitTestInfo.Location == ListViewHitTestLocations.None)
                {
                }
                else
                {
                    contextMenuStripCaffiine.Show(Cursor.Position);
                }
            }
        }

        private void replacePCKToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listViewPCKS.SelectedItems.Count != 0)
            {
                buttonMode("loading");
                OpenFileDialog openPCK = new OpenFileDialog();

                openPCK.Filter = "PCK File|*.pck";
                
                if (openPCK.ShowDialog() == DialogResult.OK)
                {
                    using (FTPClient client = new FTPClient("ftp://" + textBoxHost.Text, "", "a3262443"))
                        client.UploadFile(openPCK.FileName, dlcPath + "/" + listViewPCKS.SelectedItems[0].Text + "/" + listViewPCKS.SelectedItems[0].Tag.ToString());
                    if(TextBoxPackImage.Text != "")
                    {
                        string PackID = GetPackID(openPCK.FileName);
                        GetARCFromConsole();
                        ReplacePackImage(PackID);
                        SendARCToConsole();
                    }
                    MessageBox.Show("PCK Replaced!");
                }
            }
            buttonMode("stop");
            loadPcks();
        }

        private void listViewPCKS_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void buttonInstall_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Replace with " + Path.GetFileNameWithoutExtension(mod) + "?", "Install Mod", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                if (!Directory.Exists(dlcPath + pcks[listViewPCKS.SelectedItems[0].Index].folder + "/"))
                {
                    Directory.CreateDirectory(dlcPath + pcks[listViewPCKS.SelectedItems[0].Index].folder + "/");
                }
                File.Copy(mod, dlcPath + pcks[listViewPCKS.SelectedItems[0].Index].folder + "/" + pcks[listViewPCKS.SelectedItems[0].Index].file);
            }
            loadPcks();
        }

        private void deletePCKModToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Directory.Delete(dlcPath + pcks[listViewPCKS.SelectedItems[0].Index].folder + "/", true);
            loadPcks();
        }

        private void buttonServerToggle_Clic(object sender, EventArgs e)
        {

        }

        private void contextMenuStripCaffiine_Opening(object sender, CancelEventArgs e)
        {

        }

        private void radioButtonSystem_CheckedChanged(object sender, EventArgs e)
        {
            loadPcks();
        }

        private void radioButtonUSB_CheckedChanged(object sender, EventArgs e)
        {
            loadPcks();
        }

        private void replaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listViewPCKS.SelectedItems.Count != 0)
            {
                buttonMode("loading");
                using (FTPClient client = new FTPClient("ftp://" + textBoxHost.Text, "", "a3262443"))
                    client.UploadFile(mod, dlcPath + "/" + listViewPCKS.SelectedItems[0].Text + "/" + listViewPCKS.SelectedItems[0].Tag.ToString());
                if (TextBoxPackImage.Text != "")
                {
                    string PackID = GetPackID(mod);
                    GetARCFromConsole();
                    ReplacePackImage(PackID);
                    SendARCToConsole();
                }
                MessageBox.Show("PCK Replaced!");
            }
            buttonMode("stop");
            loadPcks();
        }

        private string GetPackID(string filename)
        {
            var reader = new PckFileReader();
            currentPCK = reader.FromFile(filename);
            if (currentPCK.TryGetFile("0", PckFile.FileData.FileType.InfoFile, out var file) &&
                file.Properties.HasProperty("PACKID"))
            {
                file.Properties.GetProperty("PACKID");
            }
            throw new KeyNotFoundException();
        }

        private void GetARCFromConsole()
        {
            using (FTPClient client = new FTPClient("ftp://" + textBoxHost.Text, "", "a3262443"))
                client.DownloadFile(dlcPath + "../../Common/Media/MediaWiiU.arc", Program.AppData + "MediaWiiU.arc");
            var reader = new ARCFileReader();
            archive = reader.FromStream(new MemoryStream(File.ReadAllBytes(Program.AppData + "MediaWiiU.arc")));
        }

        private void ReplacePackImage(string PackID)
        {
            if (archive.ContainsKey("Graphics\\PackGraphics\\" + PackID + ".png"))
                archive["Graphics\\PackGraphics\\" + PackID + ".png"] = File.ReadAllBytes(TextBoxPackImage.Text);
            else
                archive.Add("Graphics\\PackGraphics\\" + PackID + ".png", File.ReadAllBytes(TextBoxPackImage.Text));
        }

        private void SendARCToConsole()
        {
            using (FTPClient client = new FTPClient("ftp://" + textBoxHost.Text, "", "a3262443"))
            {
                MemoryStream ms = new MemoryStream();
                var writer = new ARCFileWriter(archive);
                writer.WriteToStream(ms);
                File.WriteAllBytes(Program.AppData + "MediaWiiU.arc", ms.ToArray());
                client.UploadFile(Program.AppData + "MediaWiiU.arc", dlcPath + "../../Common/Media/MediaWiiU.arc");
                archive.Clear();
                currentPCK.Files.Clear();
                currentPCK = null;
                ms.Dispose();
            }
            GC.Collect();
        }

        private void PackImageSelection_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "PNG Image|*.png";
            if(ofd.ShowDialog() == DialogResult.OK)
                TextBoxPackImage.Text = ofd.FileName;
        }
    }
}
