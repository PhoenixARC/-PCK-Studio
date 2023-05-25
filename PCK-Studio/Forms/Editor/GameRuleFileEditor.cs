using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using PckStudio.Forms.Additional_Popups.Grf;
using PckStudio.Classes.Misc;
using OMI.Formats.GameRule;
using OMI.Workers.GameRule;
using System.Diagnostics;
using OMI.Formats.Pck;
using PckStudio.Forms.Additional_Popups;
using PckStudio.Models;
using PckStudio.Properties;

namespace PckStudio.Forms.Editor
{
    public partial class GameRuleFileEditor : MetroFramework.Forms.MetroForm
    {
        private PckFile.FileData _pckfile;
        private GameRuleFile _file;
        private GameRuleFile.CompressionType compressionType;
        private GameRuleFile.CompressionLevel compressionLevel;
        public GameRuleFileEditor()
        {
            InitializeComponent();
            PromptForCompressionType();
            saveToolStripMenuItem.Visible = !Settings.Default.AutoSaveChanges;
        }

        private void PromptForCompressionType()
        {
            ItemSelectionPopUp dialog = new ItemSelectionPopUp("Wii U, PS Vita", "PS3", "Xbox 360");
            dialog.label2.Text = "Type";
            dialog.okBtn.Text = "Ok";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                switch(dialog.SelectedItem)
                {
                    case "Wii U, PS Vita":
                        wiiUPSVitaToolStripMenuItem.Checked = true;
                        break;
                    case "PS3":
                        pS3ToolStripMenuItem.Checked = true;
                            break;
                    case "Xbox 360":
                        xbox360ToolStripMenuItem.Checked = true;
                        break;
                }
            }
        }

        public GameRuleFileEditor(PckFile.FileData file) : this()
        {
            _pckfile = file;
            if (file.Size > 0)
            {
                using (var stream = new MemoryStream(file.Data))
                {
                    _file = OpenGameRuleFile(stream);
                }
            }
        }

        public GameRuleFileEditor(Stream stream) : this()
        {
            _file = OpenGameRuleFile(stream);
        }

        private GameRuleFile OpenGameRuleFile(Stream stream)
        {
            try
            {
                var reader = new GameRuleFileReader(compressionType);
                return reader.FromStream(stream);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                MessageBox.Show("Faild to open .grf/.grh file");
            }
            return default!;
        }

        private void OnLoad(object sender, EventArgs e)
        {
            RPC.SetPresence("GRF Editor", "Editing a GRF File");
            ReloadGameRuleTree();
        }

        private void LoadGameRuleTree(TreeNodeCollection root, GameRuleFile.GameRule parentRule)
        {
            foreach (var rule in parentRule.ChildRules)
            {
                TreeNode node = new TreeNode(rule.Name);
                node.Tag = rule;
                root.Add(node);
                LoadGameRuleTree(node.Nodes, rule);
            }
        }

        private void ReloadGameRuleTree()
        {
            GrfTreeView.Nodes.Clear();
            if (_file is not null)
            {
                SetCompressionLevel();
                LoadGameRuleTree(GrfTreeView.Nodes, _file.Root);
            }
        }

        private void SetCompressionLevel()
        {
            switch (_file.FileHeader.CompressionLevel)
            {
                case GameRuleFile.CompressionLevel.None:
                    noneToolStripMenuItem.Checked = true;
                    break;
                case GameRuleFile.CompressionLevel.Compressed:
                    compressedToolStripMenuItem.Checked = true;
                    break;
                case GameRuleFile.CompressionLevel.CompressedRle:
                    compressedRLEToolStripMenuItem.Checked = true;
                    break;
                case GameRuleFile.CompressionLevel.CompressedRleCrc:
                    compressedRLECRCToolStripMenuItem.Checked = true;
                    break;
            }
        }

        private void GrfTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node is TreeNode t && t.Tag is GameRuleFile.GameRule)
                ReloadParameterTreeView();
        }

        private void ReloadParameterTreeView()
        {
            GrfParametersTreeView.Nodes.Clear();
            if (GrfTreeView.SelectedNode is TreeNode t && t.Tag is GameRuleFile.GameRule rule)
            foreach (var param in rule.Parameters)
            {
                GrfParametersTreeView.Nodes.Add(new TreeNode($"{param.Key}: {param.Value}") { Tag = param});
            }
        }

        private void addDetailContextMenuItem_Click(object sender, EventArgs e)
        {
            if (GrfTreeView.SelectedNode == null || !(GrfTreeView.SelectedNode.Tag is GameRuleFile.GameRule)) return;
            var grfTag = GrfTreeView.SelectedNode.Tag as GameRuleFile.GameRule;
            AddParameter prompt = new AddParameter();
            if (prompt.ShowDialog() == DialogResult.OK)
            {
                if (grfTag.Parameters.ContainsKey(prompt.ParameterName))
                {
                    MessageBox.Show("Can't add detail that already exists.", "Error");
                    return;
                }
                grfTag.Parameters.Add(prompt.ParameterName, prompt.ParameterValue);
                ReloadParameterTreeView();
            }
        }

        private void removeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (GrfTreeView.SelectedNode is TreeNode t && t.Tag is GameRuleFile.GameRule rule &&
                GrfParametersTreeView.SelectedNode is TreeNode paramNode && paramNode.Tag is KeyValuePair<string, string> pair &&
                rule.Parameters.ContainsKey(pair.Key) && rule.Parameters.Remove(pair.Key))
            {
                ReloadParameterTreeView(); 
                return;
            }
            MessageBox.Show("No Rule selected");
        }

        private void GrfDetailsTreeView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
                removeToolStripMenuItem_Click(sender, e);
        }

        private void GrfDetailsTreeView_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (GrfTreeView.SelectedNode is TreeNode t && t.Tag is GameRuleFile.GameRule rule &&
                GrfParametersTreeView.SelectedNode is TreeNode paramNode && paramNode.Tag is KeyValuePair<string, string> param)
            {
                AddParameter prompt = new AddParameter(param.Key, param.Value, false);
                if (prompt.ShowDialog() == DialogResult.OK)
                {
                    rule.Parameters[prompt.ParameterName] = prompt.ParameterValue;
                    ReloadParameterTreeView();
                }
            }
        }

        private void addGameRuleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool isValidNode = GrfTreeView.SelectedNode is TreeNode t && t.Tag is GameRuleFile.GameRule;
            var parentRule = isValidNode
               ? GrfTreeView.SelectedNode.Tag as GameRuleFile.GameRule
               : _file.Root;

            TreeNodeCollection root = isValidNode
                ? GrfTreeView.SelectedNode.Nodes
                : GrfTreeView.Nodes;

            using (RenamePrompt prompt = new RenamePrompt(""))
            {
                prompt.OKButton.Text = "Add";
                if (MessageBox.Show($"Add Game Rule to {parentRule.Name}", "Attention",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes &&
                    prompt.ShowDialog() == DialogResult.OK &&
                    !string.IsNullOrWhiteSpace(prompt.NewText))
                {
                    var tag = parentRule.AddRule(prompt.NewText);
                    TreeNode node = new TreeNode(tag.Name);
                    node.Tag = tag;
                    root.Add(node);
                }
            }
        }

        private void removeGameRuleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (GrfTreeView.SelectedNode is TreeNode t && t.Tag is GameRuleFile.GameRule tag && removeTag(tag))
                t.Remove();
        }

        private bool removeTag(GameRuleFile.GameRule rule)
        {
            _ = rule.Parent ?? throw new ArgumentNullException(nameof(rule.Parent));
            foreach (var subTag in rule.ChildRules.ToList())
                return removeTag(subTag);
            return rule.Parent.ChildRules.Remove(rule);
        }

        private void GrfTreeView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
                removeGameRuleToolStripMenuItem_Click(sender, e);
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_file.FileHeader.unknownData[3] != 0)
            {
                MessageBox.Show("World grf saving is currently unsupported");
                return;
            }
            using (var stream = new MemoryStream())
            {
                try
                {
                    var writer = new GameRuleFileWriter(
                        _file,
                        compressionLevel,
                        compressionType);
                    writer.WriteToStream(stream);
                    _pckfile?.SetData(stream.ToArray());
                    DialogResult = DialogResult.OK;
                    MessageBox.Show("Saved!");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    MessageBox.Show($"Failed to save grf file\n{ex.Message}", "Save Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void metroPanel1_Resize(object sender, EventArgs e)
        {
            int padding = 2;
            GrfTreeView.Size = new Size(metroPanel1.Size.Width / 2 - padding, metroPanel1.Size.Height);
            GrfParametersTreeView.Size = new Size(metroPanel1.Size.Width / 2 - padding, metroPanel1.Size.Height);
            // good enough
            metroLabel2.Location = new Point(metroPanel1.Size.Width / 2 + 25, metroLabel2.Location.Y);
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Game Rule File|*.grf";
            PromptForCompressionType();
            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                using (var fs = File.OpenRead(dialog.FileName))
                {
                    _file = OpenGameRuleFile(fs);
                    ReloadGameRuleTree();
                }
            }
        }

        private void noneToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            if (sender is ToolStripRadioButtonMenuItem radioButton && radioButton.Checked)
                compressionLevel = GameRuleFile.CompressionLevel.None;
        }

        private void compressedToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            if (sender is ToolStripRadioButtonMenuItem radioButton && radioButton.Checked)
                compressionLevel = GameRuleFile.CompressionLevel.Compressed;
        }

        private void compressedRLEToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            if (sender is ToolStripRadioButtonMenuItem radioButton && radioButton.Checked)
                compressionLevel = GameRuleFile.CompressionLevel.CompressedRle;
        }

        private void compressedRLECRCToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            if (sender is ToolStripRadioButtonMenuItem radioButton && radioButton.Checked)
                compressionLevel = GameRuleFile.CompressionLevel.CompressedRleCrc;
        }

        private void wiiUPSVitaToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            if (sender is ToolStripRadioButtonMenuItem radioButton && radioButton.Checked)
                compressionType = GameRuleFile.CompressionType.Zlib;
        }

        private void pS3ToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            if (sender is ToolStripRadioButtonMenuItem radioButton && radioButton.Checked)
                compressionType = GameRuleFile.CompressionType.Deflate;
        }

        private void xbox360ToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            if (sender is ToolStripRadioButtonMenuItem radioButton && radioButton.Checked)
                compressionType = GameRuleFile.CompressionType.XMem;
        }

        private void GameRuleFileEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Settings.Default.AutoSaveChanges)
            {
                saveToolStripMenuItem_Click(sender, EventArgs.Empty);
            }
        }
    }
}
