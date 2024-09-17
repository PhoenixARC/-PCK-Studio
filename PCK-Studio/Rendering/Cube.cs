/* Copyright (c) 2024-present miku-666
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
using OpenTK;
using PckStudio.Extensions;
using PckStudio.Internal.Skin;

namespace PckStudio.Rendering
{
    internal sealed class Cube
    {
        internal Vector3 Position { get; }

        internal Vector3 Size { get; }
        
        internal Vector3 Rotation { get; }

        internal Vector2 Uv { get; }

        internal float Inflate { get; }

        internal bool MirrorTexture { get; }

        internal bool FlipZMapping { get; }
        
        internal Vector3 Center => Position + Size / 2f;
        
        internal enum Face
        {
            Back,
            Front,
            Top,
            Bottom,
            Left,
            Right
        }

        internal static Cube FromSkinBox(SkinBOX skinBOX) => FromSkinBox(skinBOX, 0f);

        internal static Cube FromSkinBox(SkinBOX skinBOX, float inflate, bool flipZMapping = false)
            => new Cube(skinBOX.Pos.ToOpenTKVector(), skinBOX.Size.ToOpenTKVector(), skinBOX.UV.ToOpenTKVector(), skinBOX.Scale + inflate, skinBOX.Mirror, flipZMapping);

        public Cube(Vector3 position, Vector3 size, Vector2 uv, float inflate, bool mirrorTexture, bool flipZMapping)
        {
            Position = position;
            Size = size;
            Uv = uv;
            Inflate = inflate;
            MirrorTexture = mirrorTexture;
            FlipZMapping = flipZMapping;
        }

        public Vector3 GetFaceCenter(Face face)
        {
            Vector3 result = Center;
            switch (face)
            {
                case Face.Top:
                    result.Y -= Size.Y / 2f;
                    return result;
                case Face.Bottom:
                    result.Y += Size.Y / 2f;
                    return result;
                case Face.Back:
                    result.Z -= Size.Z / 2f;
                    return result;
                case Face.Front:
                    result.Z += Size.Z / 2f;
                    return result;
                case Face.Left:
                    result.X -= Size.X / 2f;
                    return result;
                case Face.Right:
                    result.X += Size.X / 2f;
                    return result;
                default:
                    return result;
            }
        }

        public BoundingBox GetBoundingBox() => GetBoundingBox(Matrix4.Identity);

        public BoundingBox GetBoundingBox(Matrix4 transform)
        {
            Vector3 halfSize = Size / 2f;
            Vector3 halfSizeInflated = halfSize + new Vector3(Inflate);
            Vector3 transformedCenter = Vector3.TransformPosition(Center, transform);
            Vector3 start = transformedCenter - halfSizeInflated;
            Vector3 end   = transformedCenter + halfSizeInflated;
            return new BoundingBox(start.ToNumericsVector(), end.ToNumericsVector());
        }
    }
}
