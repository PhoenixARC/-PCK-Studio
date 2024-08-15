/* Copyright (c) 2022-present miku-666, MattNL
 * This software is provided 'as-is', without any express or implied
 * warranty. In no event will the authors be held liable for any damages
 * arising from the use of this software.
 * 
 * Permission is granted to anyone to use this software for any purpose,
 * including commercial applications, and to alter it and redistribute it
 * freely, subject to the following restrictions:
 * 
 * 1. The origin of this software must not be misrepresented; you must not
 *    claim that you wrote the original software. If you use this software
 *    in a product, an acknowledgment in the product documentation would be
 *    appreciated but is not required.
 * 2. Altered source versions must be plainly marked as such, and must not be
 *    misrepresented as being the original software.
 * 3. This notice may not be removed or altered from any source distribution.
**/
using System;
using System.Collections.Specialized;
using System.Text.RegularExpressions;

namespace PckStudio.Internal
{
    /// <summary>
    /// Represents a Skin Anim value where flags can be set
    /// </summary>
    public class SkinANIM : ICloneable, IEquatable<SkinANIM>, IEquatable<SkinAnimMask>
    {
		public static readonly SkinANIM Empty = new SkinANIM(0);

		private BitVector32 _flags;
		private static readonly Regex _validator = new Regex(@"^0x[0-9a-f]{1,8}\b", RegexOptions.IgnoreCase);

		public SkinANIM(SkinAnimMask mask)
			: this((int)mask)
		{
		}

		private SkinANIM(int mask)
		{
			_flags = new BitVector32(mask);
		}

		public override string ToString() => "0x" + _flags.Data.ToString("x8");

		public static bool IsValidANIM(string anim)
		{
			return !string.IsNullOrWhiteSpace(anim) && _validator.IsMatch(anim);
		}

		public static SkinANIM FromString(string value)
			=> IsValidANIM(value)
				? new SkinANIM(Convert.ToInt32(value.TrimEnd(' ', '\n', '\r'), 16))
                : Empty;

		public static SkinANIM operator |(SkinANIM @this, SkinANIM other) => new SkinANIM(@this._flags.Data | other._flags.Data);

		public static SkinANIM operator |(SkinANIM @this, SkinAnimMask mask) => new SkinANIM(@this._flags.Data | (int)mask);

		public static implicit operator SkinANIM(SkinAnimMask mask) => new SkinANIM(mask);

		public static bool operator ==(SkinANIM @this, SkinAnimMask mask) => @this.Equals(mask);
		public static bool operator !=(SkinANIM @this, SkinAnimMask mask) => !@this.Equals(mask);
		public static bool operator ==(SkinANIM @this, SkinANIM other) => @this.Equals(other);
		public static bool operator !=(SkinANIM @this, SkinANIM other) => !@this.Equals(other);

        public bool Equals(SkinANIM other)
        {
            return _flags.Data == other._flags.Data;
        }

        public bool Equals(SkinAnimMask other)
        {
            return _flags.Data == (int)other;
        }

		public override bool Equals(object obj) => obj is SkinANIM a && Equals(a);

		public override int GetHashCode() => _flags.Data;
		
		/// <summary>
		/// Sets the desired flag in the bitfield
		/// </summary>
		/// <param name="flag">ANIM Flag to set</param>
		/// <param name="state">State of the flag</param>
		public SkinANIM SetFlag(SkinAnimFlag flag, bool state)
		{
			if (!Enum.IsDefined(typeof(SkinAnimFlag), flag))
				throw new ArgumentOutOfRangeException(nameof(flag));
			return new SkinANIM(state ? _flags.Data | 1 << (int)flag : _flags.Data & ~(1 << (int)flag));
		}

		/// <summary>
		/// Gets flag state
		/// </summary>
		/// <param name="flag">Flag to check</param>
		/// <returns>True if flag is set, otherwise false</returns>
		public bool GetFlag(SkinAnimFlag flag)
		{
            if (!Enum.IsDefined(typeof(SkinAnimFlag), flag))
                throw new ArgumentOutOfRangeException(nameof(flag));
            return _flags[1 << (int)flag];
		}

        public object Clone()
        {
            return MemberwiseClone();
        }

        internal SkinANIM SetMask(SkinAnimMask skinAnimMask)
        {
			return new SkinANIM(skinAnimMask);
        }
    }
}
