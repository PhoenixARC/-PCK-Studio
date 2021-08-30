using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PckStudio
{
    public partial class LOCEditor : MetroFramework.Forms.MetroForm
    {
        #region Variables
        DataTable tbl;
        LOC currentLoc;

        public class displayId
        {
            public string id;
            public string defaultName;
        }
        #endregion


        public LOCEditor(LOC loc)
        {
            InitializeComponent();

            tbl = new DataTable();
            currentLoc = loc;
            tbl.Columns.Add(new DataColumn("Language") { ReadOnly = true });
            tbl.Columns.Add("Display Name");
            dataGridViewLocEntryData.DataSource = tbl;
            DataGridViewColumn column = dataGridViewLocEntryData.Columns[1];
            column.Width = 600;

        }

        private void LOCEditor_Load(object sender, EventArgs e)
        {
            foreach(string id in currentLoc.ids.names)
                treeViewLocEntries.Nodes.Add(id);
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            tbl.Rows.Clear();

            foreach (LOC.Language l in currentLoc.langs)
                tbl.Rows.Add(l.name, l.names[e.Node.Index]);

            try
            {
                if (treeViewLocEntries.SelectedNode != null)
                {
                    buttonReplaceAll.Enabled = true;
                }
                else
                {
                    buttonReplaceAll.Enabled = false;
                }
            }catch (Exception)
            {
                buttonReplaceAll.Enabled = false;
            }
        }
        
        private void deleteDisplayIDToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(treeViewLocEntries.SelectedNode != null)
            {
                int index = treeViewLocEntries.SelectedNode.Index;

                currentLoc.ids.names.RemoveAt(index);

                foreach (LOC.Language l in currentLoc.langs)
                    l.names.RemoveAt(index);

                treeViewLocEntries.Nodes.RemoveAt(index);
            }
        }

        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            for(int i = 0; i < tbl.Rows.Count; i++)
            {
                currentLoc.langs[i].names[treeViewLocEntries.SelectedNode.Index] = (string)tbl.Rows[i][1];
            }             
        }

        private void treeView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Delete) //checks to make sure pressed key was del
            {
                if (treeViewLocEntries.SelectedNode != null)
                {
                    int index = treeViewLocEntries.SelectedNode.Index;

                    currentLoc.ids.names.RemoveAt(index);

                    foreach (LOC.Language l in currentLoc.langs)
                        l.names.RemoveAt(index);

                    treeViewLocEntries.Nodes.RemoveAt(index);
                }
            }
        }

        private void buttonReplaceAll_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < tbl.Rows.Count; i++)
            {
                tbl.Rows[i][1] = textBoxReplaceAll.Text;
            }
            for (int i = 0; i < tbl.Rows.Count; i++)
            {
                currentLoc.langs[i].names[treeViewLocEntries.SelectedNode.Index] = (string)tbl.Rows[i][1];
            }
        }
    }
}
