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
using System.Windows.Media.Imaging;
using MetroFramework.Forms;
using RichPresenceClient;
using PckStudio.Classes.Networking;
using PckStudio.Classes.IO;
using API.PCKCenter.model;
using API.PCKCenter;

namespace PckStudio.Forms.Utilities
{
    public partial class PckCenterBeta : MetroForm
    {
        public PckCenterBeta()
        {
            InitializeComponent();
            try
            {
                GetCategories();
                CategoryComboBox.SelectedIndex = 0;
            }
            catch
            {

            }
        }

        public PCKCollections Collections = new PCKCollections();
        public PCKCollectionsLocal LocalCollections = new PCKCollectionsLocal();
        LocalActions LActions = new LocalActions();
        string cache = Program.AppDataCache;

        #region Functions
        public void GetCategories()
        {
            CategoryComboBox.Items.Clear();
            CategoryComboBoxLocal.Items.Clear();
            switch (metroTabControl1.SelectedIndex)
            {
                case 0:
                    string[] Cats = Collections.GetCategories();
                    foreach (string cat in Cats)
                    {
                        CategoryComboBox.Items.Add(cat);
                    }
                    break;
                case 1:
                    string[] CatsL = LocalCollections.GetLocalCategories(VitaCheckBox2.Checked);
                    foreach (string cat in CatsL)
                    {
                        CategoryComboBoxLocal.Items.Add(cat);
                    }
                    break;
            }
        }

        public void LoadPacks()
        {
            OnlineTreeView.Nodes.Clear();
            LocalTreeView.Nodes.Clear();

            DownloadButton.Visible = false;
            pictureBox1.Image = pictureBox2.Image = Properties.Resources.pckCenterHeader;

            switch (metroTabControl1.SelectedIndex)
            {
                case 0:
                    
                    PCKCenterJSON packs = Collections.GetPackDescs(CategoryComboBox.Text, VitaCheckBox.Checked);
                    Collections.CenterPacks = packs;
                    foreach (KeyValuePair<string, EntryInfo> entry in packs.Data)
                    {
                        TreeNode tn = new TreeNode(entry.Value.Name);
                        tn.Tag = entry.Key;
                        OnlineTreeView.Nodes.Add(tn);
                    }

                    break;
                case 1:

                    PCKCenterJSON Localpacks = LocalCollections.GetLocalPackDescs(CategoryComboBoxLocal.Text, VitaCheckBox2.Checked);
                    LocalCollections.CenterPacks = Localpacks;
                    foreach (KeyValuePair<string, EntryInfo> entry in Localpacks.Data)
                    {
                        TreeNode tn = new TreeNode(entry.Value.Name);
                        tn.Tag = entry.Key;
                        LocalTreeView.Nodes.Add(tn);
                    }

                    break;
            }
        }

        public bool IsPackLocal(int packID, bool isVita)
        {
            return File.Exists(cache + $"packs/{(isVita ? "vita" : "normal")}/pcks/" + packID + ".pck");
        }
        #endregion

        #region Online
        private void OnlineTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            try
            {
                EntryInfo EI = Collections.CenterPacks.Data[OnlineTreeView.SelectedNode.Tag.ToString()];
                string nam = string.Format("Pack Name: {0}\npack ID: {1}\nAuthor: {2}\nDescription: {3}",
                    EI.Name, OnlineTreeView.SelectedNode.Tag.ToString(), EI.Author, EI.Description);

                metroLabel1.Text = nam;
                metroLabel1.AutoSize = false;
                metroLabel1.WrapToLine = true;

                pictureBox1.Image = Collections.GetPackImage(int.Parse(OnlineTreeView.SelectedNode.Tag.ToString()), VitaCheckBox.Checked);

                if(!IsPackLocal(int.Parse(OnlineTreeView.SelectedNode.Tag.ToString()), VitaCheckBox.Checked))
                    DownloadButton.Visible = true;
                else
                    DownloadButton.Visible = false;/**/
            }
            catch
            {

            }
        }

        private void CategoryComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                LoadPacks();
            }
            catch
            {

            }
        }

        private void DownloadButton_Click(object sender, EventArgs e)
        {
            try
            {
                Collections.TryDownloadPack(int.Parse(OnlineTreeView.SelectedNode.Tag.ToString()), VitaCheckBox.Checked, CategoryComboBox.Text);
                MessageBox.Show("Download complete");/**/
            }
            catch
            {

            }
        }

        #endregion

        #region Local

        private void LocalTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {

            try
            {
                string nam = "Pack Name: %n\npack ID: %pid\nAuthor: %a\nDescription: %d";
                EntryInfo EI = LocalCollections.CenterPacks.Data[LocalTreeView.SelectedNode.Tag.ToString()];

                metroLabel2.Text = nam.Replace("%n", EI.Name).Replace("%a", EI.Author).Replace("%d", EI.Description).Replace("%pid", LocalTreeView.SelectedNode.Tag.ToString());
                metroLabel2.AutoSize = false;
                metroLabel2.WrapToLine = true;

                pictureBox2.Image = LocalCollections.GetLocalPackImage(int.Parse(LocalTreeView.SelectedNode.Tag.ToString()), VitaCheckBox2.Checked);
                OpenFolderButton.Visible = true;
                DeleteLocalButton.Visible = true;
            }
            catch
            {

            }
        }

        private void CategoryComboBoxLocal_SelectedIndexChanged(object sender, EventArgs e)
        {

            try
            {
                LoadPacks();
            }
            catch
            {

            }
        }

        private void metroTabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                GetCategories();
                CategoryComboBoxLocal.SelectedIndex = 0;
            }
            catch
            {

            }
        }

        private void OpenFolderButton_Click(object sender, EventArgs e)
        {
            switch (VitaCheckBox2.Checked)
            {
                case true:
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo()
                    {
                        FileName = cache + "packs/vita/pcks",
                        UseShellExecute = true,
                        Verb = "open"
                    });
                    break;
                case false:
                    Console.WriteLine(cache + "packs/normal/pcks/");
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo()
                    {
                        FileName = cache + "packs/normal/pcks/",
                        UseShellExecute = true,
                        Verb = "open"
                    });
                    break;
            }
        }

        private void DeleteLocalButton_Click(object sender, EventArgs e)
        {
            EntryInfo EI = LocalCollections.CenterPacks.Data[LocalTreeView.SelectedNode.Tag.ToString()];
            string PackID = LocalTreeView.SelectedNode.Tag.ToString();
            LActions.Removepack(LocalCollections.CenterPacks, int.Parse(PackID));
            metroLabel2.Text = "Pack Name: %n\npack ID: %pid\nAuthor: %a\nDescription: %d";
            pictureBox2.Image.Dispose();
            pictureBox2.Image = Properties.Resources.NoImageFound;
            switch (VitaCheckBox2.Checked)
            {
                case true:
                    File.Delete(cache + "packs/vita/pcks/" + PackID + ".pck");
                    File.Delete(cache + "packs/vita/images/" + PackID + ".png");
                    break;
                case false:
                    File.Delete(cache + "packs/normal/pcks/" + PackID + ".pck");
                    File.Delete(cache + "packs/normal/images/" + PackID + ".png");
                    break;
            }
            LocalTreeView.SelectedNode.Remove();
            switch (LActions.SaveLocalJSON(LocalCollections.CenterPacks, CategoryComboBoxLocal.Text, VitaCheckBox2.Checked))
            {
                case false:
                    MessageBox.Show("Could not save JSON due to unknown error");
                    break;
            }
        }

        #endregion

    }
}
