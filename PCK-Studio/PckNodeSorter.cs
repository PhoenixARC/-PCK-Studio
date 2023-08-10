using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;

using OMI.Formats.Pck;

namespace PckStudio
{
    public class PckNodeSorter : IComparer, IComparer<TreeNode>
	{
		private bool CheckForSkinAndCapeFiles(TreeNode node)
		{
			if (IsPckFile(node, out PckFile.FileData file))
				return file.Filetype == PckFile.FileData.FileType.SkinFile || file.Filetype == PckFile.FileData.FileType.CapeFile;
			return false;
		}

		private bool IsPckFile(TreeNode node) => IsPckFile(node, out _);
		private bool IsPckFile(TreeNode node, out PckFile.FileData file)
		{
			if (node.Tag is PckFile.FileData _file)
			{
				file = _file;
				return true;
			}
			file = null;
			return false;
		}

		public int Compare(TreeNode first, TreeNode second)
		{
			if (IsPckFile(first) && !IsPckFile(second))
				return -1;
			if (!IsPckFile(first) && IsPckFile(second))
				return 1;

			if (CheckForSkinAndCapeFiles(first))
				return -1;
			if (CheckForSkinAndCapeFiles(second))
				return 1;

			return first.Text.CompareTo(second.Text);
			// weird fail save
			//return first.ImageIndex.CompareTo(second.ImageIndex);
		}

		int IComparer.Compare(object x, object y)
		{
			if (x is not TreeNode NodeX)
				return -1;
			if (y is not TreeNode NodeY)
				return 1;
            return Compare(NodeX, NodeY);
		}
	}
}