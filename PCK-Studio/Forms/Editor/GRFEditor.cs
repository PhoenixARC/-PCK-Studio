using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using PckStudio.Classes.FileTypes;
using PckStudio.Classes.IO.GRF;
using PckStudio.Forms.Additional_Popups.Grf;
using PckStudio.Classes.Misc;

namespace PckStudio.Forms.Editor
{
    public partial class GRFEditor : MetroFramework.Forms.MetroForm
    {
        private PCKFile.FileData _pckfile;
        private GRFFile _file;


        private GRFEditor()
        {
            InitializeComponent();
        }

        public GRFEditor(PCKFile.FileData file) : this()
        {
            _pckfile = file;
            using(var stream = new MemoryStream(file.Data))
            {
                try
                {
                    _file = GRFFileReader.Read(stream);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    MessageBox.Show("Faild to open .grf/.grh file");
                }
            }
        }

        public GRFEditor(Stream stream) : this()
        {
            try
            {
                _file = GRFFileReader.Read(stream);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                MessageBox.Show("Faild to open .grf/.grh file");
            }
        }

        private void OnLoad(object sender, EventArgs e)
        {
            RPC.SetPresence("GRF Editor", "Editing a GRF File");
            loadGRFTreeView(GrfTreeView.Nodes, _file.Root);
        }

        private void loadGRFTreeView(TreeNodeCollection root, GRFFile.GameRule parentRule)
        {
            foreach (var rule in parentRule.SubRules)
            {
                TreeNode node = new TreeNode(rule.Name);
                node.Tag = rule;
                root.Add(node);
                loadGRFTreeView(node.Nodes, rule);
            }
        }

        private void GrfTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node is TreeNode t && t.Tag is GRFFile.GameRule)
                ReloadParameterTreeView();
        }

        private void ReloadParameterTreeView()
        {
            GrfParametersTreeView.Nodes.Clear();
            if (GrfTreeView.SelectedNode is TreeNode t && t.Tag is GRFFile.GameRule rule)
            foreach (var param in rule.Parameters)
            {
                GrfParametersTreeView.Nodes.Add(new TreeNode($"{param.Key}: {param.Value}") { Tag = param});
            }
        }

        private void addDetailContextMenuItem_Click(object sender, EventArgs e)
        {
            if (GrfTreeView.SelectedNode == null || !(GrfTreeView.SelectedNode.Tag is GRFFile.GameRule)) return;
            var grfTag = GrfTreeView.SelectedNode.Tag as GRFFile.GameRule;
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
            if (GrfTreeView.SelectedNode is TreeNode t && t.Tag is GRFFile.GameRule rule &&
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
            if (GrfTreeView.SelectedNode is TreeNode t && t.Tag is GRFFile.GameRule rule &&
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
            bool isValidNode = GrfTreeView.SelectedNode is TreeNode t && t.Tag is GRFFile.GameRule;
            GRFFile.GameRule parentRule = isValidNode
               ? GrfTreeView.SelectedNode.Tag as GRFFile.GameRule
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
            if (GrfTreeView.SelectedNode is TreeNode t && t.Tag is GRFFile.GameRule tag && removeTag(tag))
                t.Remove();
        }

        private bool removeTag(GRFFile.GameRule rule)
        {
            _ = rule.Parent ?? throw new ArgumentNullException(nameof(rule.Parent));
            foreach (var subTag in rule.SubRules.ToList())
                return removeTag(subTag);
            return rule.Parent.SubRules.Remove(rule);
        }

        private void GrfTreeView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
                removeGameRuleToolStripMenuItem_Click(sender, e);
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_file.IsWorld)
            {
                MessageBox.Show("World grf saving is currently unsupported");
                return;
            }
            using (var stream = new MemoryStream())
            {
                try
                {
                    GRFFileWriter.Write(stream, _file, GRFFile.eCompressionType.ZlibRleCrc);
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
    }
}
