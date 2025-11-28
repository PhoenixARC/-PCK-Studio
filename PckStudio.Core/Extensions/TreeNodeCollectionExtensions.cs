using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PckStudio.Core.Extensions
{
    public static class TreeNodeCollectionExtensions
    {

        /// <summary>
        /// wrapper that allows the use of <paramref name="name"/> in <code>TreeNode.Nodes.Find(<paramref name="name"/>, ...)</code> and <code>TreeNode.Nodes.ContainsKey(<paramref name="name"/>)</code>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="tag"></param>
        /// <returns>new Created TreeNode</returns>
        public static TreeNode CreateNode(this TreeNodeCollection root, string name, object tag = null)
        {
            TreeNode node = new TreeNode(name);
            node.Name = name;
            node.Tag = tag;
            root.Add(node);
            return node;
        }

        public static TreeNode BuildNodeTreeBySeperator(this TreeNodeCollection root, string path, char seperator)
        {
            _ = root ?? throw new ArgumentNullException(nameof(root));
            if (!path.Contains(seperator))
            {
                return root.CreateNode(path);
            }
            string nodeText = path.Substring(0, path.IndexOf(seperator));
            string subPath = path.Substring(path.IndexOf(seperator) + 1);

            if (string.IsNullOrWhiteSpace(nodeText))
            {
                return BuildNodeTreeBySeperator(root, subPath, seperator);
            }

            TreeNode subNode = root.ContainsKey(nodeText) ? root[nodeText] : root.CreateNode(nodeText);
            return BuildNodeTreeBySeperator(subNode.Nodes, subPath, seperator);
        }
    }
}
