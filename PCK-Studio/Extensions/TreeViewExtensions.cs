using System;
using System.Linq;
using System.Windows.Forms;

namespace PckStudio.Extensions
{
    internal static class TreeViewExtensions
    {   
        public static TreeNode[] FindPath(this TreeView treeView, string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return Array.Empty<TreeNode>();

            if (!path.Contains(treeView.PathSeparator))
            {
                return treeView.Nodes.Find(path, false);
            }

            string segment = path.Substring(0, path.IndexOf(treeView.PathSeparator));
            if (treeView.Nodes.ContainsKey(segment))
            {
                TreeNode[] res = treeView.Nodes[segment].GetChildNodes().Where(node => node.FullPath == path).ToArray();
                return res;
            }
            return Array.Empty<TreeNode>();
        }
    }
}
