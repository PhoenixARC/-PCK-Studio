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
using PckStudio.Core;

namespace PckStudio.Rendering
{
    public sealed class Cube
    {
        public Vector3 Position { get; }

        public Vector3 Size { get; }

        public Vector3 Rotation { get; }

        public Vector2 Uv { get; }

        public float Inflate { get; }

        public bool MirrorTexture { get; }

        public bool FlipZMapping { get; }

        public Vector3 Center => Position + Size / 2f;

        public enum Face
        {
            Back,
            Front,
            Top,
            Bottom,
            Left,
            Right
        }

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
            return new BoundingBox(start, end);
        }
    }
}
