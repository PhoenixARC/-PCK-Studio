using FileTransferProtocolLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Net;
using System.Windows.Forms;
using PckStudio.Classes.ToolboxItems;

namespace PckStudio.Forms
{
    public partial class InstallPS3 : ThemeForm
    {
        string loca = "";
        string dlcPath = "";
        string mod = "";
        bool serverOn = false;
        string currentpath = "";

        public InstallPS3(string mod)
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

        List<pckDir> pcks = new List<pckDir>();

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
            if (JPDig.Checked)
            {
                region = "NPJB00549/";
            }
            else if (EurDisc.Checked)
            {
                region = "BLES01976/";
            }
            else if (EurDig.Checked)
            {
                region = "NPEB01899/";
            }
            else if (USDisc.Checked)
            {
                region = "BLUS31426/";
            }
            else if (USDig.Checked)
            {
                region = "NPUB31419/";
            }

            string device = "/dev_hdd0/";

            if (region != "" && device != "")
            {
                dlcPath = device + "game/" + region;
                buttonServerToggle.Enabled = true;
                if (listViewPCKS.Columns.Count == 0)
                {
                    listViewPCKS.Columns.Add(dlcPath, 395);
                }
            }
        }

        private void buttonServerToggle_Click(object sender, EventArgs e)
        {
            string mode = "";
            if (serverOn == false)
            {
                //Makes sure user typed in their ip
                if (textBoxHost.Text == "")
                {
                    MessageBox.Show("Please enter a valid Playstation®3 IP!");
                    return;
                }

                //Turns Server On
                try
                {
                    buttonMode(mode = "loading");

                    ServicePointManager.Expect100Continue = true;

                    //ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(OnValidateCertificate);
                    FtpWebRequest request = (FtpWebRequest)WebRequest.Create("ftp://" + textBoxHost.Text + "/" + dlcPath);
                    currentpath = textBoxHost.Text + "/" + dlcPath;
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
                        FtpWebRequest request2 = (FtpWebRequest)WebRequest.Create("ftp://" + textBoxHost.Text + "/" + dlcPath + "/");
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
                        if(pck.Text != ".")
                            listViewPCKS.Items.Add(pck);
                    }

                    buttonMode(mode = "stop");
                }
                catch (Exception disc)
                {
                    buttonMode(mode = "start");
                    MessageBox.Show(disc.ToString());
                }
            }
            else if (serverOn == true)
            {
                //Turns Server Off
                listViewPCKS.Items.Clear();
                try
                {
                    buttonMode(mode = "start");
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
            if (listViewPCKS.SelectedItems.Count != 0)
            {
                buttonMode("loading");
                OpenFileDialog openPCK = new OpenFileDialog();

                if (openPCK.ShowDialog() == DialogResult.OK)
                {
                    FTP client = new FTP("ftp://" + textBoxHost.Text, "", "");
                    client.UploadFile(openPCK.FileName, dlcPath + "/" + listViewPCKS.SelectedItems[0].Text + "/" + listViewPCKS.SelectedItems[0].Tag.ToString());
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


        private void replaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listViewPCKS.SelectedItems.Count != 0)
            {
                buttonMode("loading");
                FTP client = new FTP("ftp://" + textBoxHost.Text, "", "");
                client.UploadFile(mod, dlcPath + "/" + listViewPCKS.SelectedItems[0].Text + "/" + listViewPCKS.SelectedItems[0].Tag.ToString());
                MessageBox.Show("PCK Replaced!");
            }
            buttonMode("stop");
            loadPcks();
        }

        private void EurDisc_CheckedChanged(object sender, EventArgs e)
        {
            loadPcks();
        }

        private void EurDig_CheckedChanged(object sender, EventArgs e)
        {
            loadPcks();
        }

        private void USDig_CheckedChanged(object sender, EventArgs e)
        {
            loadPcks();
        }

        private void USDisc_CheckedChanged(object sender, EventArgs e)
        {
            loadPcks();
        }

        private void JPDig_CheckedChanged(object sender, EventArgs e)
        {
            loadPcks();
        }

        private void installPS3_Load(object sender, EventArgs e)
        {
            loadPcks();
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
