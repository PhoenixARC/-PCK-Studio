using PckStudio.ToolboxItems;
using System.Collections.Generic;

namespace PckStudio
{
    public partial class MetaList : ThemeForm
    {
        public MetaList(List<string> metaTags)
        {
            InitializeComponent();
            MetaTreeView.Nodes.Clear();
            metaTags.ForEach(s => MetaTreeView.Nodes.Add(s));
        }
    }
}