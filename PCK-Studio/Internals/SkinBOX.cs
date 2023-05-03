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
using System.Numerics;

namespace PckStudio.Internal
{
    public class SkinBOX : ICloneable, IEquatable<SkinBOX>
    {
        public string Type;
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
            var arguments = value.Split(' ');
            if (arguments.Length < 9)
            {
                throw new ArgumentException("Arguments must have at least a length of 9");
            }
            var type = arguments[0];
            var pos = TryGetVector3(arguments, 1);
            var size = TryGetVector3(arguments, 4);
            var uv = TryGetVector2(arguments, 7);
            var skinBox = new SkinBOX(type, pos, size, uv);
            if (arguments.Length >= 10)
                skinBox.HideWithArmor = arguments[9] == "1";
            if (arguments.Length >= 11)
                skinBox.Mirror = arguments[10] == "1";
            if (arguments.Length >= 12)
                float.TryParse(arguments[11], out skinBox.Scale);
            return skinBox;
        }

        public ValueTuple<string, string> ToProperty()
		{
			return new ValueTuple<string, string>("BOX", ToString().Replace(',', '.'));
		}

        public override string ToString()
        {
			return $"{Type} {Pos.X} {Pos.Y} {Pos.Z} {Size.X} {Size.Y} {Size.Z} {UV.X} {UV.Y} {Convert.ToInt32(HideWithArmor)} {Convert.ToInt32(Mirror)} {Scale}";
        }

        private static Vector2 TryGetVector2(string[] arguments, int startIndex)
        {
            float.TryParse(arguments[startIndex], out float x);
            float.TryParse(arguments[startIndex + 1], out float y);
            return new Vector2(x, y);
        }

        private static Vector3 TryGetVector3(string[] arguments, int startIndex)
        {
            var vec2 = TryGetVector2(arguments, startIndex);
            float.TryParse(arguments[startIndex + 2], out float z);
            return new Vector3(vec2, z);
        }

        public override int GetHashCode()
        {
            return Type.GetHashCode() % Pos.GetHashCode() * UV.GetHashCode() % Size.GetHashCode();
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
