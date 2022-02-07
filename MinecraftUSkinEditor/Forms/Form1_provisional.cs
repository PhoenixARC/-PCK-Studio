using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.IO.Compression;
using System.Net;
using System.Diagnostics;
using PckStudio.Properties;
using Ohana3DS_Rebirth.Ohana;
using PckStudio.Forms;
using System.Drawing.Imaging;
using RichPresenceClient;

namespace PckStudio.Forms
{
    public partial class Form1_provisional : Form
    {


        #region Variables
        string saveLocation;//Save location for pck file
        int fileCount = 0;//variable for number of minefiles
        string Version = "6.2";//template for program version
        string hosturl = "";
        string basurl = "";
        string PCKFile = "";
        string PCKFileBCKUP = "x";
        loadedTexture tex = new loadedTexture(); //3DS feature variable


        PCK.MineFile mf;//Template minefile variable
        PCK currentPCK;//currently opened pck
        LOC l;//Locdata
        PCK.MineFile mfLoc;//LOC minefile
        Dictionary<int, string> types;//Template list for metadata of a individual minefiles metadata
        PCK.MineFile file;//template for a selected minefile
        bool needsUpdate = false;
        bool saved = true;
        string appData = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/PCK Studio/";
        public static bool correct = false;
        bool isdebug = false;

        public class displayId
        {
            public string id;
            public string defaultName;
        }
        #endregion

        private struct loadedTexture
        {
            public bool modified;
            public uint gpuCommandsOffset;
            public uint gpuCommandsWordCount;
            public uint offset;
            public int length;
            public RenderBase.OTexture texture;
        }

        public Form1_provisional()
        {


            Directory.CreateDirectory(Environment.CurrentDirectory + "\\template");
            if (!File.Exists(Environment.CurrentDirectory + "\\template\\UntitledSkinPCK.pck"))
                File.WriteAllBytes(Environment.CurrentDirectory + "\\template\\UntitledSkinPCK.pck", Resources.UntitledSkinPCK);
            if (!File.Exists(Environment.CurrentDirectory + "\\settings.ini"))
                File.WriteAllText(Environment.CurrentDirectory + "\\settings.ini", Resources.settings);
            hosturl = File.ReadAllText(Environment.CurrentDirectory + "\\settings.ini").Split(new[] { "\r\n", "\n" }, StringSplitOptions.None)[0];


            InitializeComponent();

            if (Program.IsDev)
                isdebug = true;

            labelVersion.Text += Version;
            pckOpen.AllowDrop = true;
        }

        private void Form1_provisional_Load(object sender, EventArgs e)
        {

            try 
            {

                new WebClient().DownloadString(Program.baseurl + ChangeURL.Text);
                basurl = Program.baseurl;
                Console.WriteLine(basurl + ChangeURL.Text);
            }
            catch
            {
                try
                {
                    new WebClient().DownloadString(Program.backurl + ChangeURL.Text);
                    basurl = Program.backurl;
                    Console.WriteLine(basurl + ChangeURL.Text);
                }
                catch
                {
                    try
                    {
                        new WebClient().DownloadString("https://google.com");
                        MessageBox.Show("PCK Studio Service is offline, the domain may have changed.\nOpening website");
                        Process.Start("https://phoenixarc.github.io/pckstudio.tk/");
                    }
                    catch
                    {
                        MessageBox.Show("Could not connect to service, internet may be offline");
                    }
                }
            }


            Directory.CreateDirectory(Environment.CurrentDirectory + "\\template");
            if (!File.Exists(Environment.CurrentDirectory + "\\template\\UntitledSkinPCK.pck"))
                File.WriteAllBytes(Environment.CurrentDirectory + "\\template\\UntitledSkinPCK.pck", Resources.UntitledSkinPCK);


            if (isdebug)
                DBGLabel.Visible = true;
            //Makes sure appdata exists
            if (!Directory.Exists(appData))
            {
                Directory.CreateDirectory(appData);
            }

            if (!Directory.Exists(Environment.CurrentDirectory + "\\cache\\mods\\"))
            {
                Directory.CreateDirectory(Environment.CurrentDirectory + "\\cache\\mods\\");
            }


            //Checks to see if program version file exists, and creates one if it doesn't
            //Latest changelog on program start-up
            try
            {
                using (WebClient client = new WebClient())
                {
                    if (isdebug)
                        File.WriteAllText(appData + "pckStudioChangelog.txt", File.ReadAllText("C:\\WEBSITES\\PCKStudio\\studio\\PCK\\api\\" + ChangeURL.Text));
                    else
                        File.WriteAllText(appData + "pckStudioChangelog.txt", client.DownloadString(basurl + ChangeURL.Text));
                    richTextBoxChangelog.Text = File.ReadAllText(appData + "pckStudioChangelog.txt");
                }
            }
            catch
            {
                MessageBox.Show("Could not load changelog");
            }

            if (!File.Exists(Application.StartupPath + @"\ver.txt"))
            {
                File.WriteAllText(Application.StartupPath + @"\ver.txt", Version);
            }
            try
            {
                if (float.Parse(new WebClient().DownloadString(basurl + "updatePCKStudio.txt").Replace("\n", "")) > float.Parse(Version))
                {
                    Console.WriteLine(new WebClient().DownloadString(basurl + "updatePCKStudio.txt").Replace("\n", "") + " != " + Version);
                    if (MessageBox.Show("Update avaliable!\ndo you want to update?", "UPDATE", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        Process.Start(Environment.CurrentDirectory + "\\nobleUpdater.exe");
                    else
                        uPDATEToolStripMenuItem1.Visible = true;
                }
                else
                {
                    uPDATEToolStripMenuItem1.Visible = false;
                }
            }
            catch
            {
                MessageBox.Show("Could not load Version Information");
            }
        }


        #region opens and loads pck file





        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                using (var ofd = new OpenFileDialog())
                {
                    ofd.CheckFileExists = true; //makes sure opened pck exists
                    ofd.Filter = "PCK (Minecraft Console Package)|*.pck";

                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        PCKFile = Path.GetFileName(ofd.FileName);
                        openPck(ofd.FileName);
                    }
                }
            }
            catch (Exception err)
            {
                MessageBox.Show("The PCK you're trying to use currently isn't supported\n" + err.StackTrace + "\n\n" + err.Message);//Error handling for PCKs that give errors when trying to be opened
            }
        }

        private void openPck(string filePath)
        {
            new TabPage();
            treeViewMain.Nodes.Clear();
            PCK pCK = (currentPCK = new PCK(filePath));
            foreach (PCK.MineFile mineFile in pCK.mineFiles)
            {
                Console.WriteLine(mineFile.name);
                if (!(mineFile.name == "0"))
                {
                    continue;
                }
                foreach (object[] entry in mineFile.entries)
                {
                    if (entry[0].ToString() == "LOCK")
                    {
                        if ((new pckLocked(entry[1].ToString(), correct).ShowDialog() != DialogResult.OK || !correct))
                        {
                            return;
                        }
                    }
                }
            }
            addPasswordToolStripMenuItem.Enabled = true;
            openedPCKS.SelectedTab.Text = Path.GetFileName(filePath);
            saveLocation = filePath;
            _ = treeViewMain;
            _ = pictureBoxImagePreview;
            _ = treeMeta;
            _ = textBox1;
            _ = label1;
            _ = label2;
            _ = tabDataDisplay;
            ImageList imageList = new ImageList();
            imageList.ColorDepth = ColorDepth.Depth32Bit;
            imageList.ImageSize = new Size(20, 20);
            imageList.Images.Add(Resources.ZZFolder);
            imageList.Images.Add(Resources.BINKA_ICON);
            imageList.Images.Add(Resources.IMAGE_ICON);
            imageList.Images.Add(Resources.LOC_ICON);
            imageList.Images.Add(Resources.PCK_ICON);
            imageList.Images.Add(Resources.ZUnknown);
            treeViewMain.ImageList = imageList;
            foreach (PCK.MineFile mineFile2 in pCK.mineFiles)
            {
                TreeNode treeNode = new TreeNode();
                treeNode.Text = mineFile2.name;
                treeNode.Tag = mineFile2;
                string text = "";
                int num = 0;
                new List<string>();
                TreeNodeCollection nodes = treeViewMain.Nodes;
                do
                {
                    text = "";
                    string name = mineFile2.name;
                    for (int i = 0; i < name.Length; i++)
                    {
                        char c = name[i];
                        bool flag = false;
                        if (c == '/')
                        {
                            foreach (TreeNode item in nodes)
                            {
                                if (item.Text == text)
                                {
                                    nodes = nodes[item.Index].Nodes;
                                    flag = true;
                                }
                            }
                            if (!flag)
                            {
                                nodes.Add(text);
                                nodes = nodes[nodes.Count - 1].Nodes;
                            }
                            flag = false;
                            text = "";
                        }
                        else
                        {
                            text += c;
                        }
                        num++;
                    }
                }
                while (num != mineFile2.name.Length);
                if (Path.GetExtension(text) == ".binka")
                {
                    treeNode.ImageIndex = 1;
                    treeNode.SelectedImageIndex = 1;
                }
                else if (Path.GetExtension(text) == ".png")
                {
                    treeNode.ImageIndex = 2;
                    treeNode.SelectedImageIndex = 2;
                }
                else if (Path.GetExtension(text) == ".loc")
                {
                    treeNode.ImageIndex = 3;
                    treeNode.SelectedImageIndex = 3;
                }
                else if (Path.GetExtension(text) == ".pck")
                {
                    treeNode.ImageIndex = 4;
                    treeNode.SelectedImageIndex = 4;
                }
                else
                {
                    treeNode.ImageIndex = 5;
                    treeNode.SelectedImageIndex = 5;
                }
                treeNode.Text = text;
                nodes.Add(treeNode);
                saved = false;
            }
            myTablePanelStartScreen.Visible = false;
            pckOpen.Visible = false;
            label5.Visible = false;
            richTextBoxChangelog.Visible = false;
            openedPCKS.Visible = true;
            openedPCKS.AllowDrop = true;
            foreach (ToolStripMenuItem dropDownItem in fileToolStripMenuItem.DropDownItems)
            {
                dropDownItem.Enabled = true;
            }
            foreach (ToolStripMenuItem dropDownItem2 in editToolStripMenuItem.DropDownItems)
            {
                dropDownItem2.Enabled = true;
            }
            foreach (TreeNode node in treeViewMain.Nodes)
            {
                if (node.Text == "languages.loc")
                {
                    mfLoc = (PCK.MineFile)treeViewMain.Nodes[node.Index].Tag;
                }
                if (node.Text == "localisation.loc")
                {
                    mfLoc = (PCK.MineFile)treeViewMain.Nodes[node.Index].Tag;
                }
            }
            fileCount = 0;
            foreach (PCK.MineFile mineFile3 in currentPCK.mineFiles)
            {
                _ = mineFile3;
                fileCount++;
            }
            saved = false;
            LittleEndianCheckBox.Visible = true;
            LittleEndianCheckBox.Checked = currentPCK.IsLittleEndian;
        }
        #endregion
    }
}
