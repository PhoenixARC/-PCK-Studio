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
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using OpenTK;
using PckStudio.Extensions;
using PckStudio.Internal;

namespace PckStudio.Rendering
{
    internal class CubeMesh : Cube
    {
        internal bool ShouldRender { get; set; } = true;
        
        internal new Vector3 Position
        {
            get => _position;
            set
            {
                if (_position != value)
                {
                    _position = value;
                    UpdateVertices();
                }
            }
        }
        
        internal new Vector3 Size
        {
            get => _size;
            set
            {
                if (_size != value)
                {
                    _size = value;
                    UpdateVertices();
                }
            }
        }
        
        internal new Vector2 Uv
        {
            get => _uv;
            set
            {
                if (_uv != value)
                {
                    _uv = value;
                    UpdateVertices();
                }
            }
        }
        
        internal new float Inflate
        {
            get => _inflate;
            set
            {
                if (_inflate != value)
                {
                    _inflate = value;
                    UpdateVertices();
                }
            }
        }
        
        internal new bool MirrorTexture
        {
            get => _mirrorTexture;
            set
            {
                if (_mirrorTexture != value)
                {
                    _mirrorTexture = value;
                    UpdateVertices();
                }
            }
        }
        
        internal new bool FlipZMapping
        {
            get => _flipZMapping;
            set
            {
                if (_flipZMapping != value)
                {
                    _flipZMapping = value;
                    UpdateVertices();
                }
            }
        }

        private static int[] indicesData = [
                    // Face 1 (Back)
                     0,  1,  2,
                     2,  3,  0,
                    // Face 2 (Front)
                     4,  5,  6,
                     6,  7,  4,
                    // Face 3 (Top)
                     8,  9, 10,
                    10, 11,  8,
                    // Face 4 (Bottom)
                    12, 13, 14,
                    14, 15, 12,
                    // Face 5 (Left)
                    16, 17, 18,
                    18, 19, 16,
                    // Face 6 (Right)
                    20, 21, 22, 
                    22, 23, 20
            ];

        private TextureVertex[] vertices;

        /// <param name="position"></param>
        /// <param name="size"></param>
        /// <param name="uv"></param>
        /// <param name="inflate"></param>
        /// <param name="mirrorTexture"></param>
        /// <param name="flipZMapping">Flips the bottom face mapping of the uv mapping</param>
        public CubeMesh(bool enabled, Vector3 position, Vector3 size, Vector2 uv, float inflate, bool mirrorTexture, bool flipZMapping)
            : base()
        {
            ShouldRender = enabled;
            Position = position;
            Size = size;
            Uv = uv;
            Inflate = inflate;
            MirrorTexture = mirrorTexture;
            FlipZMapping = flipZMapping;
            UpdateVertices();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="position"></param>
        /// <param name="size"></param>
        /// <param name="uv"></param>
        /// <param name="inflate"></param>
        /// <param name="mirrorTexture"></param>
        /// <param name="flipZMapping">Flips the bottom face mapping of the uv mapping</param>
        public CubeMesh(Vector3 position, Vector3 size, Vector2 uv, float inflate, bool mirrorTexture, bool flipZMapping)
            : this(true, position, size, uv, inflate, mirrorTexture, flipZMapping)
        {
        }

        public CubeMesh(Cube cube)
            : this(cube.Position, cube.Size, cube.Uv, cube.Inflate, cube.MirrorTexture, cube.FlipZMapping)
        {
        }

        private TextureVertex[] GetCubeVertexData()
        {
            int mirror = MirrorTexture ? 1 : 0;
            List<TextureVertex> vertices = new List<TextureVertex>();

            Vector2 uv = Uv;

            BoundingBox boundingBox = GetBoundingBox();
            Vector3 from = boundingBox.Start;
            Vector3 to   = boundingBox.End;

            var back = new TextureVertex[]
            {
                // Back
                new TextureVertex(new Vector3(from.X,   to.Y, to.Z), new Vector2(uv.X + Size.Z * 2 + Size.X + Size.X * (1 - mirror), uv.Y + Size.Z + Size.Y)),
                new TextureVertex(new Vector3(  to.X,   to.Y, to.Z), new Vector2(uv.X + Size.Z * 2 + Size.X + Size.X * mirror, uv.Y + Size.Z + Size.Y)),
                new TextureVertex(new Vector3(  to.X, from.Y, to.Z), new Vector2(uv.X + Size.Z * 2 + Size.X + Size.X * mirror, uv.Y + Size.Z)),
                new TextureVertex(new Vector3(from.X, from.Y, to.Z), new Vector2(uv.X + Size.Z * 2 + Size.X + Size.X * (1 - mirror), uv.Y + Size.Z))
            };
            var front = new TextureVertex[]
            {
                // Front
                new TextureVertex(new Vector3(from.X,   to.Y, from.Z), new Vector2(uv.X + Size.Z + Size.X * mirror, uv.Y + Size.Z + Size.Y)),
                new TextureVertex(new Vector3(  to.X,   to.Y, from.Z), new Vector2(uv.X + Size.Z + Size.X * (1 - mirror), uv.Y + Size.Z + Size.Y)),
                new TextureVertex(new Vector3(  to.X, from.Y, from.Z), new Vector2(uv.X + Size.Z + Size.X * (1 - mirror), uv.Y + Size.Z)),
                new TextureVertex(new Vector3(from.X, from.Y, from.Z), new Vector2(uv.X + Size.Z + Size.X * mirror, uv.Y + Size.Z)),
            };
            var top = new TextureVertex[]
            {
                // Top
                new TextureVertex(new Vector3(from.X, from.Y, from.Z), new Vector2(uv.X + Size.Z + Size.X * mirror, uv.Y + Size.Z)),
                new TextureVertex(new Vector3(from.X, from.Y,   to.Z), new Vector2(uv.X + Size.Z + Size.X * mirror, uv.Y)),
                new TextureVertex(new Vector3(  to.X, from.Y,   to.Z), new Vector2(uv.X + Size.Z + Size.X * (1 - mirror), uv.Y)),
                new TextureVertex(new Vector3(  to.X, from.Y, from.Z), new Vector2(uv.X + Size.Z + Size.X * (1 - mirror), uv.Y + Size.Z)),
            };
            var bottom = new TextureVertex[]
            {
                // Bottom
                new TextureVertex(new Vector3(  to.X, to.Y, from.Z), new Vector2(uv.X + Size.Z + Size.X + Size.X * (1 - mirror), uv.Y + (FlipZMapping ? Size.Z : 0))),
                new TextureVertex(new Vector3(  to.X, to.Y,   to.Z), new Vector2(uv.X + Size.Z + Size.X + Size.X * (1 - mirror), uv.Y + (!FlipZMapping ? Size.Z : 0))),
                new TextureVertex(new Vector3(from.X, to.Y,   to.Z), new Vector2(uv.X + Size.Z + Size.X + Size.X * mirror, uv.Y + (!FlipZMapping ? Size.Z : 0))),
                new TextureVertex(new Vector3(from.X, to.Y, from.Z), new Vector2(uv.X + Size.Z + Size.X + Size.X * mirror, uv.Y + (FlipZMapping ? Size.Z : 0))),
            };
            var left = new TextureVertex[]
            {
                // Left
                new TextureVertex(new Vector3(MirrorTexture ? from.X : to.X, from.Y, from.Z), new Vector2(uv.X + Size.X + Size.Z, uv.Y + Size.Z)),
                new TextureVertex(new Vector3(MirrorTexture ? from.X : to.X, to.Y  , from.Z), new Vector2(uv.X + Size.X + Size.Z, uv.Y + Size.Z + Size.Y)),
                new TextureVertex(new Vector3(MirrorTexture ? from.X : to.X, to.Y  , to.Z), new Vector2(uv.X + Size.X + Size.Z * 2, uv.Y + Size.Z + Size.Y)),
                new TextureVertex(new Vector3(MirrorTexture ? from.X : to.X, from.Y, to.Z), new Vector2(uv.X + Size.X + Size.Z * 2, uv.Y + Size.Z)),
            };
            var right = new TextureVertex[]
            {
                // Right
                new TextureVertex(new Vector3(MirrorTexture ? to.X : from.X, from.Y, from.Z), new Vector2(uv.X + Size.Z, uv.Y + Size.Z)),
                new TextureVertex(new Vector3(MirrorTexture ? to.X : from.X,   to.Y, from.Z), new Vector2(uv.X + Size.Z, uv.Y + Size.Z + Size.Y)),
                new TextureVertex(new Vector3(MirrorTexture ? to.X : from.X,   to.Y, to.Z), new Vector2(uv.X, uv.Y + Size.Z + Size.Y)),
                new TextureVertex(new Vector3(MirrorTexture ? to.X : from.X, from.Y, to.Z), new Vector2(uv.X, uv.Y + Size.Z)),
            };
            
            vertices.AddRange(back);
            vertices.AddRange(front);
            vertices.AddRange(top);
            vertices.AddRange(bottom);
            vertices.AddRange(left);
            vertices.AddRange(right);

            return vertices.ToArray();
        }

        private void UpdateVertices()
        {
            vertices = GetCubeVertexData();
        }

        internal TextureVertex[] GetVertices()
        {
            return vertices;
        }

        internal int[] GetIndices()
        {
            return indicesData;
        }

        internal static CubeMesh Create(SkinBOX skinBox)
        {
            return new CubeMesh(FromSkinBox(skinBox));
        }
    }
}