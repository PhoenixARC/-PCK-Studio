using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;

using OMI.Formats.Pck;
using PckStudio.Extensions;

namespace PckStudio.Internal
{
    public class PckNodeSorter : IComparer, IComparer<TreeNode>
	{
		public object SortingOptions { get; set; } = null;
		public bool Descending { get; set; } = false;

		private bool CheckForSkinAndCapeFiles(TreeNode node)
		{
			return node.TryGetTagData(out PckFile.FileData file) &&
				(file.Filetype == PckFile.FileData.FileType.SkinFile || file.Filetype == PckFile.FileData.FileType.CapeFile);
		}

        public int Compare(object x, object y)
        {
            TreeNode tx = x as TreeNode;
            TreeNode ty = y as TreeNode;
			return Compare(tx, ty);
        }

        public int Compare(TreeNode x, TreeNode y)
		{
			int result = InternalCompare(x, y);
			Debug.WriteLine(result);
			if (Descending && result != 0)
            {
                result = 2 % result + 1;
            }
            return result;
		}

		private int InternalCompare(TreeNode first, TreeNode second)
		{
			if (first.IsTagOfType<PckFile.FileData>() && !second.IsTagOfType<PckFile.FileData>())
				return -1;
			if (!first.IsTagOfType<PckFile.FileData>() && second.IsTagOfType<PckFile.FileData>())
				return 1;

			if (CheckForSkinAndCapeFiles(first))
				return 1;
			if (CheckForSkinAndCapeFiles(second))
				return 1;

			return first.Text.CompareTo(second.Text);
		}
	}

}