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
using OpenTK.Graphics.OpenGL;
using PckStudio.Extensions;
using PckStudio.Internal;

namespace PckStudio.Rendering
{
    internal class CubeRenderGroup : RenderGroup<TextureVertex>
    {

        private class CubeData
        {
            internal const int vertexCountPerCube = 24;
            internal bool Enabled { get; set; } = true;
            internal Vector3 Position { get; set; } = Vector3.Zero;
            internal Vector3 Size { get; set; } = Vector3.One;
            internal Vector2 Uv { get; set; } = Vector2.Zero;
            internal float Scale { get; set; } = 1f;
            internal bool MirrorTexture { get; set; } = false;
            internal bool FlipZMapping { get; set; } = false;

            private static uint[] indicesData = new uint[] {
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
            };

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
                Enabled = enabled;
                Position = position;
                Size = size;
                Uv = uv;
                Scale = scale;
                MirrorTexture = mirrorTexture;
                FlipZMapping = flipZMapping;
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

            internal static TextureVertex[] GetCubeVertexData(Vector3 position, Vector3 size, Vector2 uv, float scale = 1f, bool mirrorTexture = false, bool flipZMapping = false)
            {
                int mirror = mirrorTexture ? 1 : 0;
                return
                [
                    // Back
                    new TextureVertex(new Vector3(position.X         , size.Y + position.Y, size.Z + position.Z), new Vector2(uv.X + size.Z * 2 + size.X + size.X * (1 - mirror), uv.Y + size.Z + size.Y), scale),
                    new TextureVertex(new Vector3(size.X + position.X, size.Y + position.Y, size.Z + position.Z), new Vector2(uv.X + size.Z * 2 + size.X + size.X *      mirror , uv.Y + size.Z + size.Y), scale),
                    new TextureVertex(new Vector3(size.X + position.X, position.Y         , size.Z + position.Z), new Vector2(uv.X + size.Z * 2 + size.X + size.X *      mirror , uv.Y + size.Z         ), scale),
                    new TextureVertex(new Vector3(position.X         , position.Y         , size.Z + position.Z), new Vector2(uv.X + size.Z * 2 + size.X + size.X * (1 - mirror), uv.Y + size.Z         ), scale),

                    // Front
                    new TextureVertex(new Vector3(position.X         , size.Y + position.Y, position.Z), new Vector2(uv.X + size.Z + size.X *      mirror , uv.Y + size.Z + size.Y), scale),
                    new TextureVertex(new Vector3(size.X + position.X, size.Y + position.Y, position.Z), new Vector2(uv.X + size.Z + size.X * (1 - mirror), uv.Y + size.Z + size.Y), scale),
                    new TextureVertex(new Vector3(size.X + position.X, position.Y         , position.Z), new Vector2(uv.X + size.Z + size.X * (1 - mirror), uv.Y + size.Z         ), scale),
                    new TextureVertex(new Vector3(position.X         , position.Y         , position.Z), new Vector2(uv.X + size.Z + size.X *      mirror , uv.Y + size.Z         ), scale),

                    // Top
                    new TextureVertex(new Vector3(position.X         , position.Y,          position.Z), new Vector2(uv.X + size.Z + size.X *      mirror , uv.Y + size.Z), scale),
                    new TextureVertex(new Vector3(position.X         , position.Y, size.Z + position.Z), new Vector2(uv.X + size.Z + size.X *      mirror , uv.Y         ), scale),
                    new TextureVertex(new Vector3(size.X + position.X, position.Y, size.Z + position.Z), new Vector2(uv.X + size.Z + size.X * (1 - mirror), uv.Y         ), scale),
                    new TextureVertex(new Vector3(size.X + position.X, position.Y,          position.Z), new Vector2(uv.X + size.Z + size.X * (1 - mirror), uv.Y + size.Z), scale),

                    // Bottom
                    new TextureVertex(new Vector3(position.X + size.X, size.Y + position.Y, position.Z         ), new Vector2(uv.X + size.Z + size.X + size.X * (1 - mirror), uv.Y + ( flipZMapping ? size.Z : 0)), scale),
                    new TextureVertex(new Vector3(position.X + size.X, size.Y + position.Y, size.Z + position.Z), new Vector2(uv.X + size.Z + size.X + size.X * (1 - mirror), uv.Y + (!flipZMapping ? size.Z : 0)), scale),
                    new TextureVertex(new Vector3(position.X         , size.Y + position.Y, size.Z + position.Z), new Vector2(uv.X + size.Z + size.X + size.X *      mirror , uv.Y + (!flipZMapping ? size.Z : 0)), scale),
                    new TextureVertex(new Vector3(position.X         , size.Y + position.Y, position.Z         ), new Vector2(uv.X + size.Z + size.X + size.X *      mirror , uv.Y + ( flipZMapping ? size.Z : 0)), scale),

                    // Left
                    new TextureVertex(new Vector3((1 - mirror) * size.X + position.X, position.Y         , position.Z         ), new Vector2(uv.X + size.X + size.Z    , uv.Y + size.Z         ), scale),
                    new TextureVertex(new Vector3((1 - mirror) * size.X + position.X, size.Y + position.Y, position.Z         ), new Vector2(uv.X + size.X + size.Z    , uv.Y + size.Z + size.Y), scale),
                    new TextureVertex(new Vector3((1 - mirror) * size.X + position.X, size.Y + position.Y, size.Z + position.Z), new Vector2(uv.X + size.X + size.Z * 2, uv.Y + size.Z + size.Y), scale),
                    new TextureVertex(new Vector3((1 - mirror) * size.X + position.X, position.Y         , size.Z + position.Z), new Vector2(uv.X + size.X + size.Z * 2, uv.Y + size.Z         ), scale),

                    // Right
                    new TextureVertex(new Vector3(mirror * size.X + position.X, position.Y         , position.Z         ), new Vector2(uv.X + size.Z, uv.Y + size.Z         ), scale),
                    new TextureVertex(new Vector3(mirror * size.X + position.X, size.Y + position.Y, position.Z         ), new Vector2(uv.X + size.Z, uv.Y + size.Z + size.Y), scale),
                    new TextureVertex(new Vector3(mirror * size.X + position.X, size.Y + position.Y, size.Z + position.Z), new Vector2(uv.X         , uv.Y + size.Z + size.Y), scale),
                    new TextureVertex(new Vector3(mirror * size.X + position.X, position.Y         , size.Z + position.Z), new Vector2(uv.X         , uv.Y + size.Z         ), scale),
                ];
            }

            internal TextureVertex[] GetVertices()
            {
                return GetCubeVertexData(Position, Size, Uv, Scale, MirrorTexture, FlipZMapping);
            }
            
            [Conditional("DEBUG")]
            internal void GetMappedTextureUv(CubeFace face)
            {
                var indices = GetIndices();
                var faceInices = indices.Skip((int)face * 4).Take(4).ToArray();
                var vertices = GetVertices();
                var uv0 = vertices[faceInices[0]].TexPosition;
                var uv1 = vertices[faceInices[1]].TexPosition;
                var uv2 = vertices[faceInices[2]].TexPosition;
                var uv3 = vertices[faceInices[3]].TexPosition;
                Debug.WriteLine("----------");
                Debug.WriteLine(uv0);
                Debug.WriteLine(uv1);
                Debug.WriteLine(uv2);
                Debug.WriteLine(uv3);
            }

            internal uint[] GetIndices()
            {
                return indicesData;
            }
        }

        private List<CubeData> cubes;

        internal CubeRenderGroup(string name) : base(name, PrimitiveType.Quads)
        {
            cubes = new List<CubeData>(5);
        }

        internal void AddSkinBox(SkinBOX skinBox)
        {
            AddCube(skinBox.Pos.ToOpenTKVector(), skinBox.Size.ToOpenTKVector(), skinBox.UV.ToOpenTKVector(), skinBox.Scale + 1f, skinBox.Mirror, skinBox.Type == "HEAD");
        }

        internal void Clear()
        {
            cubes.Clear();
            ResetBuffers();
        }

        /// <summary>
        /// Submits buffered data to the underlying graphics buffer
        /// </summary>
        internal void Submit()
        {
            ResetBuffers();
            foreach (var cube in cubes)
            {
                if (!cube.Enabled)
                    continue;
                
                vertices.AddRange(cube.GetVertices());
                var indexStorage = cube.GetIndices();
                indices.AddRange(indexStorage.Select(n => n + indicesOffset));
                indicesOffset += CubeData.vertexCountPerCube;
            }
        }

        internal void AddCube(Vector3 position, Vector3 size, Vector2 uv, float scale = 1f, bool mirrorTexture = false, bool flipZMapping = false)
        {
            var cube = new CubeData(position, size, uv, scale, mirrorTexture, flipZMapping);
            cubes.Add(cube);
            //cube.GetMappedTextureUv(CubeData.CubeFace.Right);
        }

        internal void ReplaceCube(int index, Vector3 position, Vector3 size, Vector2 uv, float scale = 1f, bool mirrorTexture = false)
        {
            if (!cubes.IndexInRange(index))
                throw new IndexOutOfRangeException();

            var cube = cubes[index];
            cube.Position = position;
            cube.Size = size;
            cube.Uv = uv;
            cube.Scale = scale;
            cube.MirrorTexture = mirrorTexture;
            Submit();
        }

        internal void SetEnabled(int index, bool enable)
        {
            if (!cubes.IndexInRange(index))
                throw new IndexOutOfRangeException();

            cubes[index].Enabled = enable;
            Submit();
        }
    }
}
