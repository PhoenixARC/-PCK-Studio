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

        #region variables

        public PCKCollections Collections = new PCKCollections();
        public PCKCollectionsLocal LocalCollections = new PCKCollectionsLocal();

        string cache = Program.Appdata + "cache/";

        #endregion

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
                    string[] CatsL = LocalCollections.GetLocalCategories();
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
            pictureBox1.Image = Properties.Resources.pckCenterHeader;
            pictureBox2.Image = Properties.Resources.pckCenterHeader;

            switch (metroTabControl1.SelectedIndex)
            {
                case 0:
                    switch (VitaCheckBox.Checked)
                    {
                        case true:
                            string[] packsVita = Collections.GetPackDescs(CategoryComboBox.Text, true);
                            foreach (string pack in packsVita)
                            {
                                if (!string.IsNullOrWhiteSpace(pack) && !string.IsNullOrEmpty(pack))
                                    OnlineTreeView.Nodes.Add(Collections.GetPackName(pack, true));
                            }
                            break;
                        case false:
                            string[] packs = Collections.GetPackDescs(CategoryComboBox.Text, false);
                            foreach (string pack in packs)
                            {
                                if(!string.IsNullOrWhiteSpace(pack) && !string.IsNullOrEmpty(pack))
                                    OnlineTreeView.Nodes.Add(Collections.GetPackName(pack, false));
                            }
                            break;
                    }
                    break;
                case 1:
                    switch (VitaCheckBox2.Checked)
                    {
                        case true:
                            string[] packsVita = LocalCollections.GetLocalPackDescs(CategoryComboBoxLocal.Text, true);
                            foreach (string pack in packsVita)
                            {
                                if (!string.IsNullOrWhiteSpace(pack) && !string.IsNullOrEmpty(pack))
                                    LocalTreeView.Nodes.Add(LocalCollections.GetLocalPackName(pack, true));
                            }
                            break;
                        case false:
                            string[] packs = LocalCollections.GetLocalPackDescs(CategoryComboBoxLocal.Text, false);
                            foreach (string pack in packs)
                            {
                                if (!string.IsNullOrWhiteSpace(pack) && !string.IsNullOrEmpty(pack))
                                    LocalTreeView.Nodes.Add(LocalCollections.GetLocalPackName(pack, false));
                            }
                            break;
                    }
                    break;
            }
        }

        public bool IsPackLocal(string PackFile, bool IsVita)
        {
            return File.Exists(cache + "packs/files/" + (IsVita ? "Vita/" : "") + PackFile + ".pck");
        }
        #endregion

        #region Online
        private void OnlineTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            try
            {
                string nam = "Pack Name: %n\nAuthor: %a\nDescription: %d";
                string[] packs = Collections.GetPackDescs(CategoryComboBox.Text, VitaCheckBox.Checked);
                string[] Data = Collections.GetPackData(packs[OnlineTreeView.SelectedNode.Index], VitaCheckBox.Checked);

                metroLabel1.Text = nam.Replace("%n", Data[0]).Replace("%a", Data[1]).Replace("%d", Data[2]);
                metroLabel1.AutoSize = false;
                metroLabel1.WrapToLine = true;

                pictureBox1.Image = Collections.GetPackImage(packs[OnlineTreeView.SelectedNode.Index], VitaCheckBox.Checked);

                if(!IsPackLocal(packs[OnlineTreeView.SelectedNode.Index], VitaCheckBox.Checked))
                    DownloadButton.Visible = true;
                else
                    DownloadButton.Visible = false;
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
                string[] packs = Collections.GetPackDescs(CategoryComboBox.Text, VitaCheckBox.Checked);
                Collections.TryDownloadPack(packs[OnlineTreeView.SelectedNode.Index], VitaCheckBox.Checked, CategoryComboBox.Text);
                MessageBox.Show("Download complete");
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
                string nam = "Pack Name: %n\nAuthor: %a\nDescription: %d";
                string[] packs = LocalCollections.GetLocalPackDescs(CategoryComboBoxLocal.Text, VitaCheckBox2.Checked);
                string[] Data = LocalCollections.GetLocalPackData(packs[LocalTreeView.SelectedNode.Index], VitaCheckBox2.Checked);

                metroLabel2.Text = nam.Replace("%n", Data[0]).Replace("%a", Data[1]).Replace("%d", Data[2]);
                metroLabel2.AutoSize = false;
                metroLabel2.WrapToLine = true;

                pictureBox2.Image = LocalCollections.GetLocalPackImage(packs[LocalTreeView.SelectedNode.Index], VitaCheckBox2.Checked);
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
                        FileName = Program.Appdata + "cache/packs/files/vita",
                        UseShellExecute = true,
                        Verb = "open"
                    });
                    break;
                case false:
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo()
                    {
                        FileName = Program.Appdata + "cache/packs/files",
                        UseShellExecute = true,
                        Verb = "open"
                    });
                    break;
            }
        }

        private void DeleteLocalButton_Click(object sender, EventArgs e)
        {
            string[] packs = LocalCollections.GetLocalPackDescs(CategoryComboBoxLocal.Text, VitaCheckBox2.Checked);
            if (MessageBox.Show("Are you sure you would like to remove '" + packs[LocalTreeView.SelectedNode.Index] + "'?", "Confirmation Dialog", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                pictureBox2.Image = Properties.Resources.NoImageFound;
                switch (VitaCheckBox2.Checked)
                {
                    case false:
                        string FileInfo = File.ReadAllText(Program.Appdata + "cache/packs/Category/Category" + CategoryComboBoxLocal.Text + ".txt");
                        File.Delete(Program.Appdata + "cache/packs/files/" + packs[LocalTreeView.SelectedNode.Index] + ".pck");
                        File.Delete(Program.Appdata + "cache/packs/descs/" + packs[LocalTreeView.SelectedNode.Index] + ".desc");
                        try
                        {
                            File.Delete(Program.Appdata + "cache/packs/images/" + packs[LocalTreeView.SelectedNode.Index] + ".png");
                        }
                        catch { }
                        File.WriteAllText(Program.Appdata + "cache/packs/Category/Category" + CategoryComboBoxLocal.Text + ".txt", FileInfo.Replace("\n" + packs[LocalTreeView.SelectedNode.Index], ""));
                        break;
                    case true:
                        string FileInfo2 = File.ReadAllText(Program.Appdata + "cache/packs/Category/VitaCategory" + CategoryComboBoxLocal.Text + ".txt");
                        File.Delete(Program.Appdata + "cache/packs/files/Vita/" + packs[LocalTreeView.SelectedNode.Index] + ".pck");
                        File.Delete(Program.Appdata + "cache/packs/descs/Vita/" + packs[LocalTreeView.SelectedNode.Index] + ".desc");
                        try
                        {
                            File.Delete(Program.Appdata + "cache/packs/images/Vita/" + packs[LocalTreeView.SelectedNode.Index] + ".png");
                        }
                        catch { }
                        File.WriteAllText(Program.Appdata + "cache/packs/Category/VitaCategory" + CategoryComboBoxLocal.Text + ".txt", FileInfo2.Replace("\n" + packs[LocalTreeView.SelectedNode.Index], ""));
                        break;
                }
                LocalTreeView.SelectedNode.Remove();
                metroLabel2.Text = "Pack Name: %n\nAuthor: %a\nDescription: %d";
            }
        }

        #endregion

    }
}
