﻿using System;
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
using System.Windows.Media.Imaging;
using RichPresenceClient;

namespace PckStudio.Forms
{
    public partial class pckCenter : MetroFramework.Forms.MetroForm
    {
        string[] mods;
        string hosturl = File.ReadAllText(Environment.CurrentDirectory + "\\settings.ini").Split(new[] { "\r\n", "\n" }, StringSplitOptions.None)[1] + "/studio/PCK/api/";
        string loadDirectory = File.ReadAllText(Environment.CurrentDirectory + "\\settings.ini").Split(new[] { "\r\n", "\n" }, StringSplitOptions.None)[1] + "/studio/PCK/api/pckCenterList.txt";
        string appData = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/PCK Studio/";
        string cacheDir;

        bool nobleLoaded = true;
        bool newLoaded = true;
        bool devPicksLoaded = true;
        bool communityLoaded = true;
        bool TexLoaded = true;
        bool isVita = false;
        

        public pckCenter()
        {
            InitializeComponent();
            //listViewNav.SmallImageList = imgList;

            cacheDir = Environment.CurrentDirectory + "\\cache\\mods\\";

            if (!Directory.Exists(cacheDir))
            {
                Directory.CreateDirectory(cacheDir);
            }
            if(isVita)
                loadDirectory = File.ReadAllText(Environment.CurrentDirectory + "\\settings.ini").Split(new[] { "\r\n", "\n" }, StringSplitOptions.None)[1] + "/studio/PCK/api/pckCenterVitaList.txt";
        }

        private void reload(bool checkNeeded)
        {
            try
            {
                using (WebClient client = new WebClient())
                {
                    try
                    {
                        if ((client.DownloadString(hosturl + "pckCenterAvailable.txt")) == "1")
                        {
                        }
                        else if ((client.DownloadString(hosturl + "pckCenterAvailable.txt")) == "0")
                        {
                            MessageBox.Show("PCK Center is currently down for maintenance, sorry for any inconveniences");
                            radioButtonMine.Checked = true;
                            return;
                        }
                        else
                        {

                        }
                    }
                    catch (Exception connect)
                    {
                        MessageBox.Show(connect.ToString());
                    }
                }

                using (WebClient client = new WebClient())
                {
                    string parseContent = client.DownloadString(loadDirectory);
                    string id = "";
                    mods = parseContent.Split('\n');

                    int controlCount = pckLayout.Controls.Count;
                    for (int i = controlCount - 1; i >= 0; i--)
                    {
                        Control control = pckLayout.Controls[i];

                        pckLayout.Controls.Remove(control);
                        control.Dispose();
                    }

                    foreach (string mod in mods)
                    {
                        try
                        {
                            if (File.Exists(cacheDir + mod + ".png") && checkNeeded == true)
                            {
                                //image cache
                                HttpWebRequest textureFile = (HttpWebRequest)WebRequest.Create(hosturl + "pcks/" + mod + ".png");
                                HttpWebResponse textureFileResponse = (HttpWebResponse)textureFile.GetResponse();

                                DateTime localImageModifiedTime = File.GetLastWriteTime(cacheDir + mod + ".png");
                                DateTime onlineImageModifiedTime = textureFileResponse.LastModified;
                                textureFileResponse.Dispose();
                                if (localImageModifiedTime >= onlineImageModifiedTime)
                                {

                                }
                                else
                                {
                                    client.DownloadFile(hosturl + "pcks/" + mod + ".png", cacheDir + mod + ".png");
                                }
                            }
                            else if (mod.Length == 0) { }
                            else if (File.Exists(cacheDir + mod + ".png") && checkNeeded == false)
                            {

                            }
                            else
                            {
                                // MessageBox.Show(mod + ".png");
                                client.DownloadFile(hosturl + "pcks/" + mod + ".png", cacheDir + mod + ".png");
                            }

                            if (File.Exists(cacheDir + mod + ".desc") && checkNeeded == true)
                            {
                                //desc cache
                                HttpWebRequest descFile = (HttpWebRequest)WebRequest.Create(hosturl + "pcks/" + mod + ".desc");
                                HttpWebResponse descFileResponse = (HttpWebResponse)descFile.GetResponse();

                                DateTime localDescModifiedTime = File.GetLastWriteTime(cacheDir + mod + ".desc");
                                DateTime onlineDescModifiedTime = descFileResponse.LastModified;
                                descFileResponse.Dispose();

                                if (localDescModifiedTime >= onlineDescModifiedTime)
                                {

                                }
                                else
                                {
                                    client.DownloadFile(hosturl + "pcks/" + mod + ".desc", cacheDir + mod + ".desc");
                                }
                            }
                            else if (File.Exists(cacheDir + mod + ".png") && checkNeeded == false)
                            {

                            }
                            else if (mod.Length == 0) { }
                            else
                            {
                                client.DownloadFile(hosturl + "pcks/" + mod + ".desc", cacheDir + mod + ".desc");
                            }
                            if (mod.Length != 0)
                            {
                                string[] parseDesc = File.ReadAllText(cacheDir + mod + ".desc").Split('\n');
                                Bitmap bmp = new Bitmap(Image.FromFile(cacheDir + mod + ".png"));
                                string pckName = parseDesc[0];
                                string author = parseDesc[1];
                                string desc = parseDesc[2];
                                string direct = parseDesc[3];
                                string ad = parseDesc[4];
                                bool IsVita = (parseDesc[5] == "true" || parseDesc[5] == "True");
                                string Packname = parseDesc[6];

                                PckPreview pckPreview = new PckPreview(pckName, author, desc, direct, ad, bmp, 0, mod, null, IsVita, Packname);
                                pckLayout.Controls.Add(pckPreview);
                            }
                        }
                        catch(Exception err) { Console.WriteLine(err.Message); }
                    }
                }
            }
            catch (Exception err)
            {
                MessageBox.Show("Couldn't connect to PCK Center servers.. \n" + err.Message.ToString() + "\n" + err.ToString()) ;
            }
        }
        
        private void radioButtonNew_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonNew.Checked == true)
            {
                loadDirectory = hosturl + "pckCenterNew.txt";
                if (isVita)
                    loadDirectory = hosturl + "pckCenterVitaNew.txt";
                if (!string.IsNullOrWhiteSpace(new WebClient().DownloadString(loadDirectory)))
                {
                    reload(newLoaded);
                    newLoaded = false;
                }
                else { MessageBox.Show("No Packs Avaliable!"); }
            }
        }

        private void radioButtonDevPicks_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonDevPicks.Checked == true)
            {
                loadDirectory = hosturl + "pckCenterPicks.txt";
                if (isVita)
                    loadDirectory = hosturl + "pckCenterVitaPicks.txt";
                if (!string.IsNullOrWhiteSpace(new WebClient().DownloadString(loadDirectory)))
                    {
                        reload(devPicksLoaded);
                        devPicksLoaded = false;
                }
                else { MessageBox.Show("No Packs Avaliable!"); }
            }
        }

        private void radioButtonCommunity_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonCommunity.Checked == true)
            {
                loadDirectory = hosturl + "pckCenterCommunity.txt";
                if(isVita)
                    loadDirectory = hosturl + "pckCenterVitaCommunity.txt";
                if (!string.IsNullOrWhiteSpace(new WebClient().DownloadString(loadDirectory)))
                {
                    reload(communityLoaded);
                    communityLoaded = false;
                }
                else { MessageBox.Show("No Packs Avaliable!"); }
            }
        }

        private void radioButtonMine_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonMine.Checked == true)
            {
                loadCollectdion();
            }
        }

        private void loadCollectdion()
        {
            int controlCount = pckLayout.Controls.Count;
            for (int i = controlCount - 1; i >= 0; i--)
            {
                Control control = pckLayout.Controls[i];

                pckLayout.Controls.Remove(control);
                control.Dispose();
            }

            pckLayout.Enabled = false;
            List<string> pckFiles = Directory.GetFiles(appData + "/PCK Center/myPcks/", "*.*", SearchOption.AllDirectories).Where(file => new string[] { ".pck" }.Contains(Path.GetExtension(file))).ToList();
            foreach (string pck in pckFiles)
            {
                int line = 0;
                string pckName = "";
                string author = "";
                string desc = "";
                string direct = "";
                string ad = "";

                string mod = Path.GetFileName(pck);
                mod = Path.GetFileNameWithoutExtension(mod);

                string[] parseDesc = File.ReadAllText(appData + "/PCK Center/myPcks/" + mod + ".desc").Split('\n');
                    pckName += parseDesc[0];
                    author += parseDesc[1];
                    desc += parseDesc[2];
                    direct += parseDesc[3];
                    ad += parseDesc[4];
                
                
                string filename = appData + "/PCK Center/myPcks/" + mod + ".png";

                Bitmap bmp = null;
                using (FileStream memStream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    bmp = (Bitmap)Image.FromStream(memStream);
                }

                PckPreview pckPreview = new PckPreview(pckName, author, desc, direct, ad, bmp, 1, mod, loadCollectdion, PSVitaPCKCheckbox.Checked, "");
                pckLayout.Controls.Add(pckPreview);
            }
            pckLayout.Enabled = true;
        }

        private void radioButtonAll_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonAll.Checked == true)
            {
                loadDirectory = hosturl + "pckCenterList.txt";
                if (isVita)
                    loadDirectory = hosturl + "pckCenterVitaList.txt";
                if (!string.IsNullOrWhiteSpace(new WebClient().DownloadString(loadDirectory)))
                {
                    reload(nobleLoaded);
                    nobleLoaded = false;
                }
                else { MessageBox.Show("No Packs Avaliable!"); }
            }
        }

        private void pckCenter_Load(object sender, EventArgs e)
        {
            Directory.CreateDirectory(appData + "/PCK Center/myPcks/");
            reload(nobleLoaded);
            nobleLoaded = false;


            try
            {
                RPC.SetRPC("825875166574673940", "Viewing the PCK Center", "Program by PhoenixARC", "pcklogo", "PCK Studio", "pcklogo");
            }
            catch
            {
                Console.WriteLine("ERROR WITH RPC");
            }
        }
        
        private void pckLayout_MouseUp(object sender, MouseEventArgs e)
        {
        }

        private void pckLayout_MouseMove_1(object sender, MouseEventArgs e)
        {
        }
        //Down to Collection //Redownload //Yea
        private void pckLayout_MouseClick(object sender, MouseEventArgs e)
        {
        }

        private void pckLayout_MouseDown(object sender, MouseEventArgs e)
        {
        }

        private void pckLayout_ControlRemoved(object sender, ControlEventArgs e)
        {

        }

        private void buttonSubmit_Click(object sender, EventArgs e)
        {
            if(!isVita)
            System.Diagnostics.Process.Start("mailto:phoenixarc.canarynotifs@gmail.com?subject=PCK%20Submission&body=Pack%20name(%E3%83%91%E3%83%83%E3%82%AF%E5%90%8D)%3A%0A%0Aauthor(%E8%91%97%E8%80%85)%3A%0A%0Adescription(%E8%AA%AC%E6%98%8E)%3A%0A%0Aimage(%E7%94%BB%E5%83%8F)%3A");
            if(isVita)
            System.Diagnostics.Process.Start("mailto:phoenixarc.canarynotifs@gmail.com?subject=PCK%20Submission--Vita--&body=Pack%20name(%E3%83%91%E3%83%83%E3%82%AF%E5%90%8D)%3A%0A%0Aauthor(%E8%91%97%E8%80%85)%3A%0A%0Adescription(%E8%AA%AC%E6%98%8E)%3A%0A%0Aimage(%E7%94%BB%E5%83%8F)%3A%3A%0A%0APack%20To%20Replace%3A%0A%0A");
        }

        private void radioButtonTex_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonTex.Checked == true)
            {
                loadDirectory = hosturl + "pckCenterTex.txt";
                if (isVita)
                    loadDirectory = hosturl + "pckCenterVitaTex.txt";
                if (!string.IsNullOrWhiteSpace(new WebClient().DownloadString(loadDirectory)))
                {
                    reload(TexLoaded);
                    TexLoaded = false;
                }
                else { MessageBox.Show("No Packs Avaliable!"); }
            }
        }

        private void PSVitaPCKCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            isVita = PSVitaPCKCheckbox.Checked;


            nobleLoaded = true;
            newLoaded = true;
            devPicksLoaded = true;
            communityLoaded = true;
            TexLoaded = true;

            radioButtonAll.Checked = true;
            loadDirectory = hosturl + "pckCenterList.txt";

            if (isVita)
                loadDirectory = hosturl + "pckCenterVitaList.txt";
            if (!string.IsNullOrWhiteSpace(new WebClient().DownloadString(loadDirectory)))
            {
                reload(nobleLoaded);
                nobleLoaded = false;
            }
            else { MessageBox.Show("No Packs Avaliable!"); }
        }
    }

    public class PckPreview : UserControl
    {
        string name;
        string author;
        string desc;
        string direct;
        string ad;
        int mode;
        string mod;
        bool IsVita;
        string Pack;

        Bitmap icon;

        PictureBox iconBox = new PictureBox();
        public MyNameLabel nameLabel = new MyNameLabel();
        MyTablePanel layout = new MyTablePanel();
        MethodInvoker reloader;

        public PckPreview(string name, string author, string desc, string direct, string ad, Bitmap icon, int mode, string mod, MethodInvoker Reloader, bool vita, string packName) : base()
        {
            this.reloader = Reloader;
            nameLabel.parentPreview = this;
            layout.parentPreview = this;
            this.name = name;
            this.author = author;
            this.desc = desc;
            this.direct = direct;
            this.ad = ad;
            this.mode = mode;
            this.mod = mod;
            this.icon = icon;
            this.IsVita = vita;
            this.Pack = packName;
            layout.BackColor = Color.White;
            this.Size = new Size(250, 280);
            nameLabel.Dock = DockStyle.Fill;
            nameLabel.Location = new Point(0,0);
            nameLabel.Size = new Size(230, 30);
            iconBox.Image = icon;
            //iconBox.Dock = DockStyle.Fill;
            iconBox.Anchor = AnchorStyles.None;
            nameLabel.Text = name;
            iconBox.SizeMode = PictureBoxSizeMode.StretchImage;
            iconBox.Size = new Size(230, 230);
            layout.Margin = new Padding(0, 0, 0, 0);
            this.Margin = new Padding(20, 15, 20, 15);
            nameLabel.ForeColor = Color.Black;
            nameLabel.TextAlign = ContentAlignment.MiddleCenter;
            nameLabel.Font = new Font(nameLabel.Font.FontFamily, 14);
            layout.Controls.Add(iconBox, 0, 1);
            layout.Controls.Add(nameLabel, 0, 0);
            layout.Parent = this;
            layout.Dock = DockStyle.Fill;
            iconBox.Enabled = false;

        }

        public void setHover(bool hover)
        {
            if (hover)
            {
                layout.BackColor = Color.LightGray;
            }
            else
            {
                layout.BackColor = Color.White;
            }
            layout.Refresh();
        }
        public void onClick()
        {

            layout.BackColor = Color.Gray;
            layout.Refresh();
            pckCenterOpen openPck = new pckCenterOpen(name, author, desc, direct, ad, icon, mode, mod, reloader, IsVita, Pack);
            openPck.ShowDialog();
        }
        public void onDoubleClick()
        {

        }
    }

    public class MyTablePanel : TableLayoutPanel
    {
        public PckPreview parentPreview;

        protected override void OnMouseEnter(EventArgs e)
        {
            try
            {
                //parentPreview.setHover(true);
                //base.OnMouseEnter(e);
            }
            catch (Exception)
            {

            }
        }
        protected override void OnMouseLeave(EventArgs e)
        {
            try
            {
                //parentPreview.setHover(false);
                //base.OnMouseLeave(e);
            }
            catch (Exception)
            {

            }
        }
        protected override void OnMouseClick(MouseEventArgs e)
        {
            try
            {
                parentPreview.onClick();
                base.OnMouseClick(e);
            }
            catch (Exception)
            {

            }
        }
        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            try
            {
                parentPreview.onDoubleClick();
                base.OnMouseDoubleClick(e);
            }
            catch (Exception)
            {

            }
        }
    }

    public class MyNameLabel : Label
    {
        public PckPreview parentPreview;

        protected override void OnMouseEnter(EventArgs e)
        {
            parentPreview.setHover(true);
            base.OnMouseEnter(e);
        }
        protected override void OnMouseLeave(EventArgs e)
        {
            parentPreview.setHover(false);
            base.OnMouseLeave(e);
        }
        protected override void OnMouseClick(MouseEventArgs e)
        {
            parentPreview.onClick();
            base.OnMouseClick(e);
        }
    }
}


