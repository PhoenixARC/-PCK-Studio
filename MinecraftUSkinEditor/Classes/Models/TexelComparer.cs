using System;
using System.Collections.Generic;

namespace PckStudio.Models
{
	internal class TexelComparer : global::System.Collections.Generic.IComparer<global::PckStudio.Models.Texel>
	{
		public int Compare(global::PckStudio.Models.Texel x, global::PckStudio.Models.Texel y)
		{
			return -x.Z.CompareTo(y.Z);
		}

		public TexelComparer()
		{
		}
	}
}
