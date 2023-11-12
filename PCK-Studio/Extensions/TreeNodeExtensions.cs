using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PckStudio.Extensions
{
    internal static class TreeNodeExtensions
    {
        internal static bool IsTagOfType<T>(this TreeNode node) where T : class
        {
            return node.Tag is T;
        }

        internal static bool TryGetTagData<TOut>(this TreeNode node, out TOut tagData) where TOut : class
        {
            if (node?.Tag is TOut _data)
            {
                tagData = _data;
                return true;
            }
            tagData = default;
            return false;
        }

        internal static bool Contains(this TreeNode thisNode, TreeNode childNode)
        {
            if (childNode.Parent == null)
                return false;
            if (thisNode.Equals(childNode.Parent))
                return true;
            // If the parent node is not equal to the first node,
            // call the TreeNode.Contains recursively using the parent of the node.
            return thisNode.Contains(childNode.Parent);
        }

        internal static List<TreeNode> GetChildNodes(this TreeNode thisNode)
        {
            List<TreeNode> nodes = new List<TreeNode>(thisNode.Nodes.Count);
            foreach (TreeNode node in thisNode.Nodes)
            {
                nodes.Add(node);
                nodes.AddRange(node.GetChildNodes());
            }
            return nodes;
        }

    }
}