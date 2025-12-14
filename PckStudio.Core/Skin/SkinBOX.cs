/* Copyright (c) 2023-present miku-666, MattNL
 * This software is provided 'as-is', without any express or implied
 * warranty. In no event will the authors be held liable for any damages
 * arising from the use of this software.
 * 
 * Permission is granted to anyone to use this software for any purpose,
 * including commercial applications, and to alter it and redistribute it
 * freely, subject to the following restrictions:
 * 
 * 1.The origin of this software must not be misrepresented; you must not
 *    claim that you wrote the original software. If you use this software
 *    in a product, an acknowledgment in the product documentation would be
 *    appreciated but is not required.
 * 2. Altered source versions must be plainly marked as such, and must not be
 *    misrepresented as being the original software.
 * 3. This notice may not be removed or altered from any source distribution.
**/
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Numerics;
using PckStudio.Core.Extensions;

namespace PckStudio.Core.Skin
{
    public class SkinBOX : Box
    {
        public static readonly SkinBOX DefaultHead = new SkinBOX("HEAD", new Vector3(-4, -8, -4), new Vector3(8), Vector2.Zero);
        public static readonly SkinBOX Empty = new SkinBOX("", Vector3.Zero, Vector3.Zero, Vector2.Zero);

        public static readonly string[] BaseTypes = new string[]
        {
            "HEAD",
            "BODY",
            "ARM0",
            "ARM1",
            "LEG0",
            "LEG1",
        };

        public static readonly string[] OverlayTypes = new string[]
        {
            "HEADWEAR",
            "JACKET",
            "SLEEVE0",
            "SLEEVE1",
            "PANTS0",
            "PANTS1",
        };

        public static Dictionary<int, SkinAnimFlag> KnownHashes = new Dictionary<int, SkinAnimFlag>()
        {
            [unchecked((int)0x9560320c)] = SkinAnimFlag.HEAD_DISABLED,              // HEAD     -4 -8 -4 8  8 8  0  0 0 0 0

            [unchecked((int)0x1f13e4a3)] = SkinAnimFlag.BODY_DISABLED,              // BODY     -4  0 -2 8 12 4 16 16 0 0 0

            [unchecked((int)0x407c9b27)] = SkinAnimFlag.RIGHT_ARM_DISABLED,         // ARM0     -3 -2 -2 4 12 4 40 16 0 0 0 // standard (64x64)
            [unchecked((int)0x867c9b27)] = SkinAnimFlag.RIGHT_ARM_DISABLED,         // ARM0     -2 -2 -2 3 12 4 40 16 0 0 0 // slim

            [unchecked((int)0xca3cf050)] = SkinAnimFlag.LEFT_ARM_DISABLED,          // ARM1     -1 -2 -2 4 12 4 40 16 0 1 0 // classic (64x32)
            [unchecked((int)0x879b27)] = SkinAnimFlag.LEFT_ARM_DISABLED,            // ARM1     -1 -2 -2 4 12 4 32 48 0 0 0 // standard (64x64)
            [unchecked((int)0xe8c79b27)] = SkinAnimFlag.LEFT_ARM_DISABLED,          // ARM1     -1 -2 -2 3 12 4 32 48 0 0 0 // slim

            [unchecked((int)0x1910e24a)] = SkinAnimFlag.LEFT_LEG_DISABLED,          // LEG1     -2  0 -2 4 12 4 16 48 0 0 0 // 64x64
            [unchecked((int)0xce263773)] = SkinAnimFlag.LEFT_LEG_DISABLED,          // LEG1     -2  0 -2 4 12 4  0 16 0 1 0 // 64x32

            [unchecked((int)0x5da5e24a)] = SkinAnimFlag.RIGHT_LEG_DISABLED,         // LEG0     -2  0 -2 4 12 4  0 16 0 0 0

            [unchecked((int)0x4bfe0142)] = SkinAnimFlag.HEAD_OVERLAY_DISABLED,      // HEADWEAR -4 -8 -4 8  8 8 32  0 0 0 0

            // ------------------------------------------------------------------------------------------------------------------------------------

            [unchecked((int)0xe693e4a3)] = SkinAnimFlag.BODY_OVERLAY_DISABLED,      // BODY     -4  0 -2 8 12 4 16 32 0 0 0
            [unchecked((int)0x8e322609)] = SkinAnimFlag.BODY_OVERLAY_DISABLED,      // JACKET   -4  0 -2 8 12 4 16 32 0 0 0

            [unchecked((int)0x860c4433)] = SkinAnimFlag.RIGHT_ARM_OVERLAY_DISABLED, // SLEEVE0  -3 -2 -2 4 12 4 40 32 0 0 0 // classic
            [unchecked((int)0xcc0c4433)] = SkinAnimFlag.RIGHT_ARM_OVERLAY_DISABLED, // SLEEVE0  -2 -2 -2 3 12 4 40 32 0 0 0 // slim

            [unchecked((int)0x91407908)] = SkinAnimFlag.LEFT_ARM_OVERLAY_DISABLED,  // SLEEVE1  -1 -2 -2 4 12 4 48 48 0 0 0 // classic
            [unchecked((int)0x79807908)] = SkinAnimFlag.LEFT_ARM_OVERLAY_DISABLED,  // SLEEVE1  -1 -2 -2 3 12 4 48 48 0 0 0 // slim

            [unchecked((int)0x4de0238a)] = SkinAnimFlag.RIGHT_LEG_OVERLAY_DISABLED, // PANTS0   -2  0 -2 4 12 4  0 32 0 0 0

            [unchecked((int)0x176f238a)] = SkinAnimFlag.LEFT_LEG_OVERLAY_DISABLED,  // PANTS1   -2  0 -2 4 12 4  0 48 0 0 0
        };

        public static readonly string[] ValidBoxTypes = BaseTypes.Concat(OverlayTypes).ToArray();

        public string Type { get; }
        public BoxVisibility Visibility { get; }

        [Flags]
        public enum BoxVisibility : byte
        {
            Always,
            HideWhenWearingHelmet     = 0x1,
            HideWhenWearingChestplate = 0x2,
            HideWhenWearingLeggings   = 0x4,
            HideWhenWearingBoots      = 0x8,
        }

        public SkinBOX(string type, Vector3 pos, Vector3 size, Vector2 uv,
           BoxVisibility visibility = BoxVisibility.Always, bool mirror = false, float scale = 0.0f)
            : base(pos, size, uv, scale, mirror)
        {
            Type = type;
            Visibility = visibility;
        }

        public static SkinBOX FromString(string value) => FromString(value, false);

        public static SkinBOX FromString(string value, bool validateType = default)
        {
            var arguments = value.TrimEnd('\n', '\r', ' ').Split(' ');
            if (arguments.Length < 9)
            {
                Trace.TraceError("Arguments must have at least a length of 9.");
                return Empty;
            }
            var type = arguments[0];
            if (validateType && !IsValidType(type))
            {
                Trace.TraceError($"Invalid skinbox type: {type}.");
                return Empty;
            }

            Vector3 pos = TryGetVector3(arguments, 1);
            Vector3 size = TryGetVector3(arguments, 4);
            Vector2 uv = TryGetVector2(arguments, 7);

            BoxVisibility visibility = arguments.IndexInRange(9) && int.TryParse(arguments[9], out int flags) && flags >= 0 && flags <= 0xf ? (BoxVisibility)flags : default;
            bool mirror = arguments.IndexInRange(10) && arguments[10] == "1";
            float scale = arguments.IndexInRange(11) && float.TryParse(arguments[11], out scale) ? scale : default;
            return new SkinBOX(type, pos, size, uv, visibility, mirror, scale);
        }

        public bool IsValidType() => IsValidType(Type);
        
        public bool IsEmpty() => Size.Length() == 0;

        public static bool IsValidType(string type) => ValidBoxTypes.Contains(type);

        public bool IsBasePart() => IsBasePart(Type);

        public static bool IsBasePart(string type) => BaseTypes.Contains(type);

        public bool IsOverlayPart() => IsOverlayPart(Type);

        public static bool IsOverlayPart(string type) => OverlayTypes.Contains(type);

        public KeyValuePair<string, string> ToProperty()
        {
            return new KeyValuePair<string, string>("BOX", ToString());
        }

        private static string InvariantFormat(params object[] values)
        {
            const string sep = " ";
            string fstr = Enumerable.Range(0, values.Length)
                .Select(i => string.Concat("{", i, "}"))
                .ToString(sep);
            return string.Format(CultureInfo.InvariantCulture, fstr, values);
        }

        public override string ToString()
        {
            return InvariantFormat(Type, Position.X, Position.Y, Position.Z, Size.X, Size.Y, Size.Z, Uv.X, Uv.Y, Visibility.ToString("d"), Convert.ToInt32(Mirror), Inflate);
        }

        private static Vector2 TryGetVector2(string[] arguments, int startIndex)
        {
            float.TryParse(arguments[startIndex], out float x);
            float.TryParse(arguments[startIndex + 1], out float y);
            return new Vector2(x, y);
        }

        private static Vector3 TryGetVector3(string[] arguments, int startIndex)
        {
            Vector2 xy = TryGetVector2(arguments, startIndex);
            float.TryParse(arguments[startIndex + 2], out float z);
            return new Vector3(xy, z);
        }

        public override int GetHashCode()
        {
            int hashCode = -1311939065;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Type);
            hashCode = hashCode * -1521134295 + Position.GetHashCode();
            hashCode = hashCode * -1521134295 + Size.GetHashCode();
            hashCode = hashCode * -1521134295 + Uv.GetHashCode();
            hashCode = hashCode * -1521134295 + Visibility.GetHashCode();
            hashCode = hashCode * -1521134295 + Mirror.GetHashCode();
            hashCode = hashCode * -1521134295 + Inflate.GetHashCode();
            return hashCode;
        }
    }
}
