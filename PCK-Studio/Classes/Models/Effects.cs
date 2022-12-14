using System;

namespace PckStudio.Models
{
	[Flags]
	public enum Effects : byte
	{
		None = 0,
		FlipHorizontally = 1,
		FlipVertically = 2
	}
}
