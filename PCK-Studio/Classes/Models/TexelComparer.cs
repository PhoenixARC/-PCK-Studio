using System;
using System.Collections.Generic;

namespace PckStudio.Models
{
	internal class TexelComparer : IComparer<Texel>
	{
		public int Compare(Texel x, Texel y)
		{
			return -x.Z.CompareTo(y.Z);
		}

		public TexelComparer()
		{
		}
	}
}
