using PckStudio.Classes.FileTypes;
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
    public partial class meta : MetroFramework.Forms.MetroForm
    {
        List<string> metaList;

        public meta(List<string> metaTags)
        {
            InitializeComponent();
            metaList = metaTags;
        }

        private void meta_Load(object sender, EventArgs e)
        {
            treeView1.Nodes.Clear();
            metaList.ForEach(s => treeView1.Nodes.Add(s));
        }
    }
}