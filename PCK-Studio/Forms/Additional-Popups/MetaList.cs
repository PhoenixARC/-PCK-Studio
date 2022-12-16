using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace PckStudio
{
    public partial class MetaList : Form
    {
        public MetaList(List<string> metaTags)
        {
            InitializeComponent();
            MetaTreeView.Nodes.Clear();
            metaTags.ForEach(s => MetaTreeView.Nodes.Add(s));
        }
    }
}