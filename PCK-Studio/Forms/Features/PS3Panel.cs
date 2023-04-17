using PckStudio.Classes.Misc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PckStudio.Forms.Additional_Features
{
    public partial class PS3Panel : UserControl
    {
        private string currentpath = "";
        private bool serverOn = false;

        private string DLCPath = "";
        private string mod = "";

        public PS3Panel()
        {
            InitializeComponent();
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

        private static List<PckDir> pcks = new List<PckDir>()
        {
            new PckDir(folder: "Battle & Beasts", file: "BattleAndBeasts.pck"),
            new PckDir(folder: "Battle & Beasts 2", file: "BattleAndBeasts2.pck"),
            new PckDir(folder: "Biome Settlers Pack 1", file: "SkinsBiomeSettlers1.pck"),
            new PckDir(folder: "Biome Settlers Pack 2", file: "SkinsBiomeSettlers2.pck"),
            //new PckDir() { folder = "Campfire Tales Skin Pack", file = "" },
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

        private void SetupDLCPath()
        {
            string region = string.Empty;
            string device = "dev_hdd0";
            if (JPDig.Checked)
            {
                region = "NPJB00549";
            }
            else if (EurDisc.Checked)
            {
                region = "BLES01976";
            }
            else if (EurDig.Checked)
            {
                region = "NPEB01899";
            }
            else if (USDisc.Checked)
            {
                region = "BLUS31426";
            }
            else if (USDig.Checked)
            {
                region = "NPUB31419";
            }

            if (!string.IsNullOrWhiteSpace(region))
            {
                DLCPath = $"/{device}/game/{region}";
                buttonServerToggle.Enabled = true;
                if (listViewPCKS.Columns.Count == 0)
                {
                    listViewPCKS.Columns.Add(DLCPath, 395);
                }
            }
        }

        private void buttonServerToggle_Click(object sender, EventArgs e)
        {
            //Turns off server
            if (serverOn)
            {
                listViewPCKS.Items.Clear();
                try
                {
                    buttonMode("start");
                }
                catch (Exception disc)
                {
                    MessageBox.Show(disc.ToString());
                }
                return;
            }

            //Makes sure user typed in their ip
            if (string.IsNullOrWhiteSpace(metroTextBox1.Text))
            {
                MessageBox.Show("Please enter a valid Playstation®3 IP!");
                return;
            }

            //Turn on server
            try
            {
                buttonMode("loading");

                ServicePointManager.Expect100Continue = true;

                //ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(OnValidateCertificate);
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create("ftp://" + metroTextBox1.Text + "/" + DLCPath);
                currentpath = metroTextBox1.Text + "/" + DLCPath;
                request.Method = WebRequestMethods.Ftp.ListDirectory;
                request.Credentials = new NetworkCredential("", "");
                request.EnableSsl = false;
                request.Timeout = 1200000;

                ServicePoint sp = request.ServicePoint;
                Console.WriteLine("ServicePoint connections = {0}.", sp.ConnectionLimit);
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
                                Console.WriteLine(line);
                                line = reader.ReadLine();
                            }
                        }
                    }
                }

                foreach (ListViewItem pck in listViewPCKS.Items)
                {
                    int i = 0;
                    FtpWebRequest request2 = (FtpWebRequest)WebRequest.Create("ftp://" + metroTextBox1.Text + "/" + DLCPath + "/");
                    request2.Method = WebRequestMethods.Ftp.ListDirectory;
                    request2.Credentials = new NetworkCredential("", "");
                    request2.EnableSsl = false;
                    request2.Timeout = 1200000;

                    ServicePoint sp2 = request2.ServicePoint;
                    Console.WriteLine("NOBLEDEZ WAS HERE", sp2.ConnectionLimit);
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
                    else
                    {
                    }
                    if (pck.Text != ".")
                        listViewPCKS.Items.Add(pck);
                }

                buttonMode("stop");
            }
            catch (Exception disc)
            {
                buttonMode("start");
                MessageBox.Show(disc.ToString());
            }
        }

        private void radioButton_Click(object sender, EventArgs e)
        {
            SetupDLCPath();
        }

        private void listViewPCKS_MouseDown(object sender, MouseEventArgs e)
        {
            ListViewHitTestInfo HI = listViewPCKS.HitTest(e.Location);
            if (e.Button == MouseButtons.Right)
            {
                if (HI.Location == ListViewHitTestLocations.None)
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

        }

        private void listViewPCKS_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void buttonInstall_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Replace with " + Path.GetFileNameWithoutExtension(mod) + "?", "Install Mod", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                if (!Directory.Exists(DLCPath + pcks[listViewPCKS.SelectedItems[0].Index].FolderName + "/"))
                {
                    Directory.CreateDirectory(DLCPath + pcks[listViewPCKS.SelectedItems[0].Index].FolderName + "/");
                }
                File.Copy(mod, DLCPath + pcks[listViewPCKS.SelectedItems[0].Index].FolderName + "/" + pcks[listViewPCKS.SelectedItems[0].Index].FileName);
            }
            SetupDLCPath();
        }

        private void deletePCKModToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Directory.Delete(DLCPath + pcks[listViewPCKS.SelectedItems[0].Index].FolderName + "/", true);
            SetupDLCPath();
        }

        private void buttonServerToggle_Clic(object sender, EventArgs e)
        {

        }


        private void replaceToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void EurDisc_CheckedChanged(object sender, EventArgs e)
        {
            SetupDLCPath();
        }

        private void PS3Panel_Load(object sender, EventArgs e)
        {
            SetupDLCPath();
        }

        private void listViewPCKS_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                string folname = listViewPCKS.SelectedItems[0].Text;
                if (folname.Contains(".") && folname != "..")
                    return;
                Console.WriteLine("ftp://" + currentpath + listViewPCKS.SelectedItems[0].Text);
                listViewPCKS.Items.Clear();
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create("ftp://" + currentpath.Replace("//", "/") + folname);
                if (folname == "..")
                {
                    string[] tmp = currentpath.Split(new[] { "/" }, StringSplitOptions.None);
                    Console.WriteLine(tmp[(tmp).Length - 2]);
                    string foldr = tmp[(tmp).Length - 2];
                    request = (FtpWebRequest)WebRequest.Create("ftp://" + currentpath.Replace(foldr, "").Replace("//", "/"));
                }
                request.Method = WebRequestMethods.Ftp.ListDirectory;
                request.Credentials = new NetworkCredential("", "");
                request.EnableSsl = false;
                request.Timeout = 1200000;

                currentpath = currentpath + "/" + folname + "/";

                ServicePoint sp = request.ServicePoint;
                Console.WriteLine("ServicePoint connections = {0}.", sp.ConnectionLimit);
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
                                Console.WriteLine(line);
                                line = reader.ReadLine();
                            }
                        }
                    }
                }

                foreach (ListViewItem pck in listViewPCKS.Items)
                {
                    int i = 0;
                    FtpWebRequest request2 = (FtpWebRequest)WebRequest.Create("ftp://" + currentpath);
                    request2.Method = WebRequestMethods.Ftp.ListDirectory;
                    request2.Credentials = new NetworkCredential("", "");
                    request2.EnableSsl = false;
                    request2.Timeout = 1200000;

                    ServicePoint sp2 = request2.ServicePoint;
                    Console.WriteLine("NOBLEDEZ WAS HERE", sp2.ConnectionLimit);
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
                    else
                    {
                    }
                    listViewPCKS.Items.Add(pck);
                }
            }
            catch
            {

            }
        }
    }
}
