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
using System.Linq;
using System.Numerics;

namespace PckStudio.Internal.Skin
{
    public class SkinBOX : ICloneable, IEquatable<SkinBOX>
    {
        public static readonly SkinBOX Empty = new SkinBOX("HEAD", new Vector3(-4, -8, -4), new Vector3(8), Vector2.Zero);

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

        public string Type { get; set; }
        public Vector3 Pos;
        public Vector3 Size;
        public Vector2 UV;
        public bool HideWithArmor;
        public bool Mirror;
        public float Scale;

        public SkinBOX(string type, Vector3 pos, Vector3 size, Vector2 uv,
            bool hideWithArmor = false, bool mirror = false, float scale = 0.0f)
        {
            Type = type;
            Pos = pos;
            Size = size;
            UV = uv;
            HideWithArmor = hideWithArmor;
            Mirror = mirror;
            Scale = scale;
        }

        public static SkinBOX FromString(string value)
         {
            var arguments = value.TrimEnd('\n', '\r', ' ').Split(' ');
            if (arguments.Length < 9)
            {
                throw new ArgumentException("Arguments must have at least a length of 9");
            }
            var type = arguments[0];
            Vector3 pos = TryGetVector3(arguments, 1);
            Vector3 size = TryGetVector3(arguments, 4);
            Vector2 uv = TryGetVector2(arguments, 7);
            var skinBox = new SkinBOX(type, pos, size, uv);
            if (arguments.Length >= 10)
                skinBox.HideWithArmor = arguments[9] == "1";
            if (arguments.Length >= 11)
                skinBox.Mirror = arguments[10] == "1";
            if (arguments.Length >= 12)
                float.TryParse(arguments[11], out skinBox.Scale);
            return skinBox;
        }

        public bool IsValidType() => IsValidType(Type);

        public static bool IsValidType(string type) => ValidBoxTypes.Contains(type);

        public bool IsBasePart() => IsBasePart(Type);
        
        public static bool IsBasePart(string type) => BaseTypes.Contains(type);

        public bool IsOverlayPart() => IsOverlayPart(Type);

        public static bool IsOverlayPart(string type) => OverlayTypes.Contains(type);

        public KeyValuePair<string, string> ToProperty()
		{
			return new KeyValuePair<string, string>("BOX", ToString());
		}

        public override string ToString()
        {
			return
                $"{Type} {Pos.X} {Pos.Y} {Pos.Z} {Size.X} {Size.Y} {Size.Z} {UV.X} {UV.Y} {Convert.ToInt32(HideWithArmor)} {Convert.ToInt32(Mirror)} {Scale}"
                .Replace(',', '.');
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
            hashCode = hashCode * -1521134295 + Pos.GetHashCode();
            hashCode = hashCode * -1521134295 + Size.GetHashCode();
            hashCode = hashCode * -1521134295 + UV.GetHashCode();
            hashCode = hashCode * -1521134295 + HideWithArmor.GetHashCode();
            hashCode = hashCode * -1521134295 + Mirror.GetHashCode();
            hashCode = hashCode * -1521134295 + Scale.GetHashCode();
            return hashCode;
        }

        public override bool Equals(object obj)
        {
            return obj is SkinBOX box && Equals(box);
        }

        public bool Equals(SkinBOX other)
        {
            return Type.Equals(other.Type) &&
                Pos.Equals(other.Pos) &&
                Size.Equals(other.Size) &&
                UV.Equals(other.UV);
        }

        public object Clone()
        {
            return new SkinBOX((string)Type.Clone(), Pos, Size, UV, HideWithArmor, Mirror, Scale);
        }
    }
}
