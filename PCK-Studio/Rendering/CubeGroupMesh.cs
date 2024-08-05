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
using PckStudio.Internal.Skin;

namespace PckStudio.Rendering
{
    internal class CubeGroupMesh : GenericMesh<TextureVertex>
    {
        private List<CubeMesh> cubes;
        
        public bool FlipZMapping
        {
            get => _flipZMapping;
            set
            {
                _flipZMapping = value;
                UpdateCollection();
            }
        }

        public Vector3 Translation { get; } 
        public Vector3 Pivot { get; } 
        public Vector3 Offset { get; set; } = Vector3.Zero;

        private bool _flipZMapping = false;

        internal CubeGroupMesh(string name) : base(name, PrimitiveType.Triangles)
        {
            cubes = new List<CubeMesh>(5);
        }

        internal CubeGroupMesh(string name, Vector3 translation, Vector3 pivot)
            : this(name)
        {
            Translation = translation;
            Pivot = pivot;
        }

        public static VertexBufferLayout GetLayout()
        {
            var layout = new VertexBufferLayout();
            layout.Add(ShaderDataType.Float3);
            layout.Add(ShaderDataType.Float2);
            return layout;
        }

        internal void AddSkinBox(SkinBOX skinBox, float inflate = 0f)
        {
            var cube = CubeMesh.Create(skinBox);
            cube.FlipZMapping = FlipZMapping;
            cube.Inflate += inflate;
            cubes.Add(cube);
        }

        internal void ClearData()
        {
            cubes.Clear();
            ResetBuffers();
        }

        /// <summary>
        /// Uploads vertex data
        /// </summary>
        // TODO: rename function
        internal void UploadData()
        {
            ResetBuffers();
            foreach (CubeMesh cube in cubes)
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

        internal void AddCube(Vector3 position, Vector3 size, Vector2 uv, float inflate = 0f, bool mirrorTexture = false)
        {
            var cube = new CubeMesh(position, size, uv, inflate, mirrorTexture, FlipZMapping);
            cubes.Add(cube);
        }

        internal void RemoveCube(int index)
        {
            if (!cubes.IndexInRange(index))
                throw new IndexOutOfRangeException();
            
            cubes.RemoveAt(index);
            UploadData();
        }

        internal void ReplaceCube(int index, Vector3 position, Vector3 size, Vector2 uv, float inflate = 0f, bool mirrorTexture = false)
        {
            if (!cubes.IndexInRange(index))
                throw new IndexOutOfRangeException();

            CubeMesh cube = cubes[index];
            cube.Position = position;
            cube.Size = size;
            cube.Uv = uv;
            cube.Inflate = inflate;
            cube.MirrorTexture = mirrorTexture;
            cube.Update();
        }

        private void UpdateCollection()
        {
            foreach (CubeMesh cube in cubes)
            {
                cube.FlipZMapping = FlipZMapping;
            }
        }

        internal Matrix4 Transform
        {
            get => Matrix4.CreateTranslation(Translation + Offset) * Matrix4.CreateScale(1f, -1f, -1f);
        }

        internal Vector3 GetCenter(int index)
        {
            if (!cubes.IndexInRange(index))
                throw new IndexOutOfRangeException();

            return (Transform * Matrix4.CreateTranslation(cubes[index].Center)).ExtractTranslation();
        }

        internal BoundingBox GetCubeBoundingBox(int index)
        {
            if (!cubes.IndexInRange(index))
                throw new IndexOutOfRangeException();

            CubeMesh cube = cubes[index];
            return cube.GetBoundingBox(Transform);
        }

        internal Vector3 GetFaceCenter(int index, Cube.Face face)
        {
            if (!cubes.IndexInRange(index))
                throw new IndexOutOfRangeException();
            return Vector3.TransformPosition(cubes[index].GetFaceCenter(face), Transform);
        }

        internal void SetEnabled(int index, bool enable)
        {
            if (!cubes.IndexInRange(index))
                throw new IndexOutOfRangeException();

            cubes[index].ShouldRender = enable;
        }
    }
}
