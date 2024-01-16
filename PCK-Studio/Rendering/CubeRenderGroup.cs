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
                if (!cube.ShouldRender)
                    continue;
                
                vertices.AddRange(cube.GetVertices());
                var indexStorage = cube.GetIndices();
                indices.AddRange(indexStorage.Select(n => n + indicesOffset));
                indicesOffset += CubeData.vertexCountPerCube;
            }
        }

        private void CheckAndFixUVFaveMapping(CubeData cube, CubeData.CubeFace face, Size textureSize)
        {
            if (cube.GetMappedFaceTextureUv(face).All(texVert => texVert.X >= textureSize.Width))
            {
                CubeData.RefAction<Vector2> fixUV = (ref Vector2 uv) =>
                {
                    Debug.Write($"Old: {uv}; ");
                    uv.X %= textureSize.Width;
                    Debug.WriteLine($"Old: {uv};");
                };
                cube.SetFaceUv(face, fixUV);
            }
        }

        internal void AddCube(Vector3 position, Vector3 size, Vector2 uv, float scale = 1f, bool mirrorTexture = false, bool flipZMapping = false)
        {
            var cube = new CubeData(position, size, uv, scale, mirrorTexture, flipZMapping);
            cubes.Add(cube);
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

            cubes[index].ShouldRender = enable;
            Submit();
        }

        internal void Validate(Size textureSize)
        {
            if (!textureSize.IsEmpty)
            {
                foreach (var cube in cubes)
                {
                    CheckAndFixUVFaveMapping(cube, CubeData.CubeFace.Back, textureSize);
                    CheckAndFixUVFaveMapping(cube, CubeData.CubeFace.Front, textureSize);
                    CheckAndFixUVFaveMapping(cube, CubeData.CubeFace.Top, textureSize);
                    CheckAndFixUVFaveMapping(cube, CubeData.CubeFace.Bottom, textureSize);
                    CheckAndFixUVFaveMapping(cube, CubeData.CubeFace.Left, textureSize);
                    CheckAndFixUVFaveMapping(cube, CubeData.CubeFace.Right, textureSize);
                }
            }
        }
    }
}
