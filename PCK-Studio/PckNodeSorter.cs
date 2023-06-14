using System.Collections.Generic;
using System.Windows.Forms;

using OMI.Formats.Pck;

namespace PckStudio
{
    public class PckNodeSorter : System.Collections.IComparer, IComparer<TreeNode>
	{
		private bool CheckForSkinAndCapeFiles(TreeNode node)
		{
			if (node.Tag is PckFile.FileData file)
			{
				return file.Filetype == PckFile.FileData.FileType.SkinFile ||
					file.Filetype == PckFile.FileData.FileType.CapeFile;
			}
			return false;
		}

		public int Compare(TreeNode first, TreeNode second)
		{
			// ignore these files in order to preserve skin(and cape) files
			if (CheckForSkinAndCapeFiles(first))
			{
				return 0;
			}
			if (CheckForSkinAndCapeFiles(second))
			{
				return 0;
			}

			int result = first.Text.CompareTo(second.Text);
			if (result != 0) return result;
			return first.ImageIndex.CompareTo(second.ImageIndex);
		}

		int System.Collections.IComparer.Compare(object x, object y)
		{
			return x is TreeNode NodeX && y is TreeNode NodeY ? Compare(NodeX, NodeY) : 0;
		}
	}
}