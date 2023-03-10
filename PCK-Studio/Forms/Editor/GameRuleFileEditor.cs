using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using PckStudio.Classes.FileTypes;
using PckStudio.Forms.Additional_Popups.Grf;
using PckStudio.Classes.Misc;
using OMI.Formats.GameRule;
using OMI.Workers.GameRule;
using System.Diagnostics;
using PckStudio.Forms.Additional_Popups.Audio;

namespace PckStudio.Forms.Editor
{
    public partial class GameRuleFileEditor : MetroFramework.Forms.MetroForm
    {
        private PCKFile.FileData _pckfile;
        private GameRuleFile _file;

        public GameRuleFileEditor()
        {
            InitializeComponent();
            PromptForCompressionType();
        }

        private void PromptForCompressionType()
        {
            addCategory dialog = new addCategory(compressionTypeComboBox.Items.Cast<string>().ToArray());
            dialog.label2.Text = "Type";
            dialog.button1.Text = "Ok";
            if (dialog.ShowDialog() == DialogResult.OK)
                compressionTypeComboBox.SelectedIndex = compressionTypeComboBox.Items.IndexOf(dialog.Category);
        }

        public GameRuleFileEditor(PCKFile.FileData file) : this()
        {
            _pckfile = file;
            using (var stream = new MemoryStream(file.Data))
            {
                _file = OpenGameRuleFile(stream, (GameRuleFile.CompressionType)compressionTypeComboBox.SelectedIndex);
            }
        }

        public GameRuleFileEditor(Stream stream) : this()
        {
            _file = OpenGameRuleFile(stream, (GameRuleFile.CompressionType)compressionTypeComboBox.SelectedIndex);
        }

        private GameRuleFile OpenGameRuleFile(Stream stream, GameRuleFile.CompressionType compressionType)
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
                toolStripComboBox1.SelectedIndex = (int)_file.FileHeader.CompressionLevel;
                LoadGameRuleTree(GrfTreeView.Nodes, _file.Root);
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
                        (GameRuleFile.CompressionLevel)toolStripComboBox1.SelectedIndex,
                        (GameRuleFile.CompressionType)compressionTypeComboBox.SelectedIndex);
                    writer.WriteToStream(stream);
                    _pckfile?.SetData(stream.ToArray());
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
                    _file = OpenGameRuleFile(fs, (GameRuleFile.CompressionType)compressionTypeComboBox.SelectedIndex);
                    ReloadGameRuleTree();
                }
            }
        }
    }
}
