using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PckStudio.Internal.App;

namespace PckStudio.Extensions
{
    internal static class TreeViewExtensions
    {   
        public static TreeNode[] FindPath(this TreeView treeView, string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return Array.Empty<TreeNode>();
            string segment = path.Substring(0, path.IndexOf(treeView.PathSeparator));
            if (treeView.Nodes.ContainsKey(segment))
            {
                var res = treeView.Nodes[segment].GetChildNodes().Where(node => node.FullPath == path).ToArray();
                return res;
            }
            return Array.Empty<TreeNode>();
        }
    }
}
