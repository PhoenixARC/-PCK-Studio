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

        private static (string, string) Slice(this string s, int i)
            => (s.Substring(0, i), s.Substring(i + 1));

        public static TreeNode BuildNodeTreeBySeperator(this TreeNodeCollection root, string path, char seperator, int maxDepth = -1)
            => root.BuildNodeTreeBySeperator(path, seperator.ToString(), maxDepth);
        public static TreeNode BuildNodeTreeBySeperator(this TreeNodeCollection root, string path, string seperator, int maxDepth = -1)
        {
            _ = root ?? throw new ArgumentNullException(nameof(root));
            if (maxDepth == 0 || !path.Contains(seperator))
                return root.CreateNode(path);

            (string nodeText, string subPath) = path.Slice(path.IndexOf(seperator));

            if (string.IsNullOrWhiteSpace(nodeText))
                return BuildNodeTreeBySeperator(root, subPath, seperator, maxDepth - 1);

            TreeNode subNode = root.ContainsKey(nodeText) ? root.Find(nodeText, searchAllChildren: false).FirstOrDefault(node => node.Tag is null) ?? root.CreateNode(nodeText) : root.CreateNode(nodeText);
            return BuildNodeTreeBySeperator(subNode.Nodes, subPath, seperator, maxDepth - 1);
        }

        public static IEnumerable<TreeNode> GetLeafNodes(this TreeNodeCollection root)
        {
            foreach (TreeNode node in root)
            {
                if (node.Nodes.Count == 0)
                    yield return node;
                foreach (TreeNode ln in node.Nodes.GetLeafNodes())
                    yield return ln;
            }
        }
    }
}
