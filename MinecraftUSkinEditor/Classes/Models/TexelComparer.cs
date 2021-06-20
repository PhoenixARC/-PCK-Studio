using System;
using System.Collections.Generic;

namespace MinecraftUSkinEditor.Models
{
	internal class TexelComparer : global::System.Collections.Generic.IComparer<global::MinecraftUSkinEditor.Models.Texel>
	{
		public int Compare(global::MinecraftUSkinEditor.Models.Texel x, global::MinecraftUSkinEditor.Models.Texel y)
		{
			return -x.Z.CompareTo(y.Z);
		}

		public TexelComparer()
		{
		}
	}
}
