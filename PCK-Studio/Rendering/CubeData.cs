/* Copyright (c) 2023-present miku-666
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

namespace PckStudio.Rendering
{
    internal class CubeData
    {
        internal delegate void RefAction<T>(ref T val);

        internal const int vertexCountPerCube = 24;

        internal bool ShouldRender { get; set; } = true;
        
        internal Vector3 Position
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
        
        internal Vector3 Size
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
        
        internal Vector2 Uv
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
        
        internal float Scale
        {
            get => _scale;
            set
            {
                if (_scale != value)
                {
                    _scale = value;
                    UpdateVertices();
                }
            }
        }
        
        internal bool MirrorTexture
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
        
        internal bool FlipZMapping
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

        private void UpdateVertices()
        {
            vertices = GetCubeVertexData(Position, Size, Uv, Scale, MirrorTexture, FlipZMapping);
        }

        private Vector3 _position = Vector3.Zero;
        private Vector3 _size = Vector3.One;
        private Vector2 _uv = Vector2.Zero;
        private float _scale = 1f;
        private bool _mirrorTexture = false;
        private bool _flipZMapping = false;

        private static uint[] indicesData = [
                    // Face 1 (Back)
                    0,  1,  2,  3,
                    // Face 2 (Front)
                    4,  5,  6,  7,
                    // Face 3 (Top)
                    8,  9, 10, 11,
                    // Face 4 (Bottom)
                    12, 13, 14, 15,
                    // Face 5 (Left)
                    16, 17, 18, 19,
                    // Face 6 (Right)
                    20, 21, 22, 23,
            ];

        private TextureVertex[] vertices;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="position"></param>
        /// <param name="size"></param>
        /// <param name="uv"></param>
        /// <param name="scale"></param>
        /// <param name="mirrorTexture"></param>
        /// <param name="flipZMapping">Flips the bottom face mapping of the uv mapping</param>
        public CubeData(bool enabled, Vector3 position, Vector3 size, Vector2 uv, float scale, bool mirrorTexture, bool flipZMapping)
        {
            ShouldRender = enabled;
            Position = position;
            Size = size;
            Uv = uv;
            Scale = scale;
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
        /// <param name="scale"></param>
        /// <param name="mirrorTexture"></param>
        /// <param name="flipZMapping">Flips the bottom face mapping of the uv mapping</param>
        public CubeData(Vector3 position, Vector3 size, Vector2 uv, float scale, bool mirrorTexture, bool flipZMapping)
            : this(true, position, size, uv, scale, mirrorTexture, flipZMapping)
        {
        }

        internal enum CubeFace
        {
            Back,
            Front,
            Top,
            Bottom,
            Left,
            Right,
        }

        private static TextureVertex[] GetCubeVertexData(Vector3 position, Vector3 size, Vector2 uv, float scale,
            bool mirrorTexture, bool flipZMapping)
        {
            int mirror = mirrorTexture ? 1 : 0;
            List<TextureVertex> vertices = new List<TextureVertex>();

            var back = new TextureVertex[]
            {
                // Back
                new TextureVertex(new Vector3(position.X, size.Y + position.Y, size.Z + position.Z), new Vector2(uv.X + size.Z * 2 + size.X + size.X * (1 - mirror), uv.Y + size.Z + size.Y), scale),
                new TextureVertex(new Vector3(size.X + position.X, size.Y + position.Y, size.Z + position.Z), new Vector2(uv.X + size.Z * 2 + size.X + size.X * mirror, uv.Y + size.Z + size.Y), scale),
                new TextureVertex(new Vector3(size.X + position.X, position.Y, size.Z + position.Z), new Vector2(uv.X + size.Z * 2 + size.X + size.X * mirror, uv.Y + size.Z), scale),
                new TextureVertex(new Vector3(position.X, position.Y, size.Z + position.Z), new Vector2(uv.X + size.Z * 2 + size.X + size.X * (1 - mirror), uv.Y + size.Z), scale)
            };
            var front = new TextureVertex[]
            {
                // Front
                new TextureVertex(new Vector3(position.X, size.Y + position.Y, position.Z), new Vector2(uv.X + size.Z + size.X * mirror, uv.Y + size.Z + size.Y), scale),
                new TextureVertex(new Vector3(size.X + position.X, size.Y + position.Y, position.Z), new Vector2(uv.X + size.Z + size.X * (1 - mirror), uv.Y + size.Z + size.Y), scale),
                new TextureVertex(new Vector3(size.X + position.X, position.Y, position.Z), new Vector2(uv.X + size.Z + size.X * (1 - mirror), uv.Y + size.Z), scale),
                new TextureVertex(new Vector3(position.X, position.Y, position.Z), new Vector2(uv.X + size.Z + size.X * mirror, uv.Y + size.Z), scale),
            };
            var top = new TextureVertex[]
            {
                // Top
                new TextureVertex(new Vector3(position.X, position.Y, position.Z), new Vector2(uv.X + size.Z + size.X * mirror, uv.Y + size.Z), scale),
                new TextureVertex(new Vector3(position.X, position.Y, size.Z + position.Z), new Vector2(uv.X + size.Z + size.X * mirror, uv.Y), scale),
                new TextureVertex(new Vector3(size.X + position.X, position.Y, size.Z + position.Z), new Vector2(uv.X + size.Z + size.X * (1 - mirror), uv.Y), scale),
                new TextureVertex(new Vector3(size.X + position.X, position.Y, position.Z), new Vector2(uv.X + size.Z + size.X * (1 - mirror), uv.Y + size.Z), scale),
            };
            var bottom = new TextureVertex[]
            {
                // Bottom
                new TextureVertex(new Vector3(position.X + size.X, size.Y + position.Y, position.Z), new Vector2(uv.X + size.Z + size.X + size.X * (1 - mirror), uv.Y + (flipZMapping ? size.Z : 0)), scale),
                new TextureVertex(new Vector3(position.X + size.X, size.Y + position.Y, size.Z + position.Z), new Vector2(uv.X + size.Z + size.X + size.X * (1 - mirror), uv.Y + (!flipZMapping ? size.Z : 0)), scale),
                new TextureVertex(new Vector3(position.X, size.Y + position.Y, size.Z + position.Z), new Vector2(uv.X + size.Z + size.X + size.X * mirror, uv.Y + (!flipZMapping ? size.Z : 0)), scale),
                new TextureVertex(new Vector3(position.X, size.Y + position.Y, position.Z), new Vector2(uv.X + size.Z + size.X + size.X * mirror, uv.Y + (flipZMapping ? size.Z : 0)), scale),
            };
            var left = new TextureVertex[]
            {
                // Left
                new TextureVertex(new Vector3((1 - mirror) * size.X + position.X, position.Y, position.Z), new Vector2(uv.X + size.X + size.Z, uv.Y + size.Z), scale),
                new TextureVertex(new Vector3((1 - mirror) * size.X + position.X, size.Y + position.Y, position.Z), new Vector2(uv.X + size.X + size.Z, uv.Y + size.Z + size.Y), scale),
                new TextureVertex(new Vector3((1 - mirror) * size.X + position.X, size.Y + position.Y, size.Z + position.Z), new Vector2(uv.X + size.X + size.Z * 2, uv.Y + size.Z + size.Y), scale),
                new TextureVertex(new Vector3((1 - mirror) * size.X + position.X, position.Y, size.Z + position.Z), new Vector2(uv.X + size.X + size.Z * 2, uv.Y + size.Z), scale),
            };
            var right = new TextureVertex[]
            {
                // Right
                new TextureVertex(new Vector3(mirror * size.X + position.X, position.Y, position.Z), new Vector2(uv.X + size.Z, uv.Y + size.Z), scale),
                new TextureVertex(new Vector3(mirror * size.X + position.X, size.Y + position.Y, position.Z), new Vector2(uv.X + size.Z, uv.Y + size.Z + size.Y), scale),
                new TextureVertex(new Vector3(mirror * size.X + position.X, size.Y + position.Y, size.Z + position.Z), new Vector2(uv.X, uv.Y + size.Z + size.Y), scale),
                new TextureVertex(new Vector3(mirror * size.X + position.X, position.Y, size.Z + position.Z), new Vector2(uv.X, uv.Y + size.Z), scale),
            };
            
            vertices.AddRange(back);
            vertices.AddRange(front);
            vertices.AddRange(top);
            vertices.AddRange(bottom);
            vertices.AddRange(left);
            vertices.AddRange(right);

            return vertices.ToArray();
        }

        internal TextureVertex[] GetVertices()
        {
            return vertices;
        }

        internal Vector2[] GetMappedFaceTextureUv(CubeFace face)
        {
            var indices = GetIndices();
            var faceInices = indices.Skip((int)face * 4).Take(4).ToArray();
            var vertices = GetVertices();
            var uv0 = vertices[faceInices[0]].TexPosition;
            var uv1 = vertices[faceInices[1]].TexPosition;
            var uv2 = vertices[faceInices[2]].TexPosition;
            var uv3 = vertices[faceInices[3]].TexPosition;
            return [uv0, uv1, uv2, uv3];
        }

        internal uint[] GetIndices()
        {
            return indicesData;
        }

        internal void SetFaceUv(CubeFace face, RefAction<Vector2> refAction)
        {
            var indices = GetIndices();
            var faceInices = indices.Skip((int)face * 4).Take(4).ToArray();
            var vertices = GetVertices();
            var uv0 = vertices[faceInices[0]].TexPosition;
            refAction(ref uv0);
            vertices[faceInices[0]].TexPosition = uv0;
            
            var uv1 = vertices[faceInices[1]].TexPosition;
            refAction(ref uv1);
            vertices[faceInices[1]].TexPosition = uv1;
            
            var uv2 = vertices[faceInices[2]].TexPosition;
            refAction(ref uv2);
            vertices[faceInices[2]].TexPosition = uv2;

            var uv3 = vertices[faceInices[3]].TexPosition;
            refAction(ref uv3);
            vertices[faceInices[3]].TexPosition = uv3;
        }
    }
}