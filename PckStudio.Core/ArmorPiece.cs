using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace PckStudio.Core
{
    public sealed class ArmorPiece(Vector3 position, Vector3 size, Vector2 uv, bool mirrorTexture = default, float infalte = default)
    {
        public static ArmorPiece Helmet { get; } = new ArmorPiece(new(-4, -8, -4), new(8), new(0));
        public static ArmorPiece Chest { get; } = new ArmorPiece(new(-4, 0, -2), new(8, 12, 4), new(16));
        public static ArmorPiece Waist { get; } = new ArmorPiece(new(-4, 0, -2), new(8, 12, 4), new(16, 48));
        public static ArmorPiece RightShoulder { get; } = new ArmorPiece(new(-3, -2, -2), new(4, 12, 4), new(40, 16));
        public static ArmorPiece LeftShoulder { get; } = new ArmorPiece(new(-1, -2, -2), new(4, 12, 4), new(40, 16), mirrorTexture: true);
        public static ArmorPiece RightPants { get; } = new ArmorPiece(new(-2, 0, -2), new(4, 12, 4), new(0, 48));
        public static ArmorPiece LeftPants { get; } = new ArmorPiece(new(-2, 0, -2), new(4, 12, 4), new(0, 48), mirrorTexture: true);
        public static ArmorPiece RightBoot { get; } = new ArmorPiece(new(-2, 0, -2), new(4, 12, 4), new(0, 16));
        public static ArmorPiece LeftBoot { get; } = new ArmorPiece(new(-2, 0, -2), new(4, 12, 4), new(0, 16), mirrorTexture: true);

        public Vector3 Position { get; } = position;
        public Vector3 Size { get; } = size;
        public Vector2 Uv { get; } = uv;
        public bool MirrorTexture { get; } = mirrorTexture;
        public float Infalte { get; } = infalte;
    }
}
