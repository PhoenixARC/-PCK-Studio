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
using System.Linq;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using PckStudio.Extensions;
using PckStudio.Internal;

namespace PckStudio.Rendering
{
    internal class CubeGroupMesh : GenericMesh<TextureVertex>
    {
        private List<CubeData> cubes;

        public float Inflate { get; set; } = 0f;
        public Vector3 Translation { get; set; } = Vector3.Zero;
        public Vector3 Pivot { get; set; } = Vector3.Zero;
        public Vector3 Offset { get; set; } = Vector3.Zero;

        internal CubeGroupMesh(string name) : base(name, PrimitiveType.Triangles)
        {
            cubes = new List<CubeData>(5);
        }

        internal CubeGroupMesh(string name, float inflate)
            : this(name)
        {
            Inflate = inflate;
        }

        internal void AddSkinBox(SkinBOX skinBox)
        {
            AddCube(skinBox.Pos.ToOpenTKVector(), skinBox.Size.ToOpenTKVector(), skinBox.UV.ToOpenTKVector(), skinBox.Scale + Inflate, skinBox.Mirror,
                skinBox.Type == "HEAD" ||
                skinBox.Type == "HEADWEAR");
        }

        internal void ClearData()
        {
            cubes.Clear();
            ResetBuffers();
        }

        /// <summary>
        /// Uploads MeshData
        /// </summary>
        internal void UploadData()
        {
            ResetBuffers();
            foreach (var cube in cubes)
            {
                if (!cube.ShouldRender)
                    continue;

                TextureVertex[] cubeVertices = cube.GetVertices();
                vertices.AddRange(cubeVertices);
                var indexStorage = cube.GetIndices();
                indices.AddRange(indexStorage.Select(n => n + indicesOffset));
                indicesOffset += cubeVertices.Length;
            }
            Submit();
        }

        internal void AddCube(Vector3 position, Vector3 size, Vector2 uv, float inflate = 0f, bool mirrorTexture = false, bool flipZMapping = false)
        {
            var cube = new CubeData(position, size, uv, Inflate + inflate, mirrorTexture, flipZMapping);
            cubes.Add(cube);
        }

        internal void ReplaceCube(int index, Vector3 position, Vector3 size, Vector2 uv, float inflate = 0f, bool mirrorTexture = false)
        {
            if (!cubes.IndexInRange(index))
                throw new IndexOutOfRangeException();

            var cube = cubes[index];
            cube.Position = position;
            cube.Size = size;
            cube.Uv = uv;
            cube.Inflate = Inflate + inflate;
            cube.MirrorTexture = mirrorTexture;
        }

        private Vector3 Transform
        {
            get
            {
                Vector3 transform = Translation;
                transform.Xz -= Pivot.Xz / 2f;
                return -transform;
            }
        }


        internal Vector3 GetCenter(int index)
        {
            if (!cubes.IndexInRange(index))
                throw new IndexOutOfRangeException();

            return cubes[index].Center + Transform;
        }

        internal OutlineDefinition GetOutline(int index)
        {
            if (!cubes.IndexInRange(index))
                throw new IndexOutOfRangeException();

            CubeData cube = cubes[index];
            OutlineDefinition outline = cube.GetOutline();
            outline.verticies = outline.verticies.Select(pos => pos + Transform).ToArray();
            return outline;
        }

        internal Vector3 GetFaceCenter(int index, CubeData.CubeFace face)
        {
            if (!cubes.IndexInRange(index))
                throw new IndexOutOfRangeException();
            return cubes[index].GetFaceCenter(face) + Transform;
        }

        internal void SetEnabled(int index, bool enable)
        {
            if (!cubes.IndexInRange(index))
                throw new IndexOutOfRangeException();

            cubes[index].ShouldRender = enable;
        }
    }
}
