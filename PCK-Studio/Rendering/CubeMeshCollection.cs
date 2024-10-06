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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using PckStudio.Extensions;
using PckStudio.External.Format;
using PckStudio.Internal;
using PckStudio.Internal.Skin;

namespace PckStudio.Rendering
{
    static class CubeMeshCollectionExtensions
    {
        internal static void AddSkinBox(this CubeMeshCollection cubeMeshes, SkinBOX skinBox, float inflate = 0f)
        {
            var cube = skinBox.ToCube(inflate, cubeMeshes.FlipZMapping);
            cubeMeshes.Add(new CubeMesh(cube).SetName(skinBox.Type));
        }
    }

    internal class CubeMeshCollection : GenericMesh<TextureVertex>, ICollection<GenericMesh<TextureVertex>>
    {
        private List<GenericMesh<TextureVertex>> cubes;
        private Dictionary<string, CubeMeshCollection> subCollection;
        
        public bool FlipZMapping
        {
            get => _flipZMapping;
            set => _flipZMapping = value;
            }

        public Vector3 Translation { get; set; }
        public Vector3 Rotation { get; }
        public Vector3 Pivot { get; } 
        private Vector3 _offset { get; set; } = Vector3.Zero;
        public Vector3 Offset
        {
            get => _offset;
            set => _offset = value;
                }

        public override Matrix4 Transform => (Matrix4.CreateRotationX(MathHelper.DegreesToRadians(Rotation.X)) * Matrix4.CreateRotationY(MathHelper.DegreesToRadians(Rotation.Y)) * Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(Rotation.Z))).Pivoted(Translation + _offset, Pivot);

        public int Count => cubes.Count;

        public bool IsReadOnly => false;

        private bool _flipZMapping = false;

        internal CubeMeshCollection(string name, bool visible = true) : base(name, visible, PrimitiveType.Triangles, CubeMesh.VertexBufferLayout)
        {
            cubes = new List<GenericMesh<TextureVertex>>(5);
            subCollection = new Dictionary<string, CubeMeshCollection>();
        }

        internal CubeMeshCollection(string name, Vector3 translation, Vector3 pivot, Vector3 rotation = default)
            : this(name)
        {
            Translation = translation;
            Pivot = pivot;
            Rotation = rotation;
        }

        public override GenericMesh<TextureVertex> SetVisible(bool visible)
        {
            if (Visible == visible)
                return this;

            var mesh = new CubeMeshCollection(Name, visible);
            mesh.cubes = this.cubes;
            return mesh;
        }

        internal override IEnumerable<TextureVertex> GetVertices()
            => cubes.Where(c => c.Visible).SelectMany(c =>
                c.GetVertices().Select(vertex => new TextureVertex(Vector3.TransformPosition(vertex.Position, c.Transform), vertex.TexPosition))
            );

        internal override IEnumerable<int> GetIndices()
        {
            int offset = 0;
            IEnumerable<int> selector(GenericMesh<TextureVertex> c)
            {
                IEnumerable<int> result = c.GetIndices().Select(i => i + offset).ToArray();
                int vertexCount = c.GetVertices().Count();
                offset += vertexCount;
                return result;
            }
            return cubes.Where(c => c.Visible).SelectMany(selector);
        }

        internal void Add(Vector3 position, Vector3 size, Vector2 uv, float inflate = 0f, bool mirrorTexture = false)
        {
            var cube = new Cube(position, size, uv, inflate, mirrorTexture, FlipZMapping);
            Add(new CubeMesh(cube));
        }

        internal void AddNamed(string name, Vector3 position, Vector3 size, Vector2 uv, float inflate = 0f, bool mirrorTexture = false)
        {
            var cube = new Cube(position, size, uv, inflate, mirrorTexture, FlipZMapping);
            Add(new CubeMesh(cube).SetName(name));
        }

        internal void AddSubCollection(string name, Vector3 translation, Vector3 pivot, Vector3 rotation = default)
        {
            var item = new CubeMeshCollection(name, translation, pivot, rotation);
            Add(item);
            subCollection.Add(name, item);
        }

        internal CubeMeshCollection GetCollection(string collectionName)
        {
            _ = collectionName ?? throw new ArgumentNullException(nameof(collectionName));
            return ContainsCollection(collectionName) ? subCollection[collectionName] : null; 
        }

        internal void Remove(int index)
        {
            if (!cubes.IndexInRange(index))
                throw new IndexOutOfRangeException();
            
            cubes.RemoveAt(index);
        }

        internal void ReplaceCube(int index, Vector3 position, Vector3 size, Vector2 uv, float inflate = 0f, bool mirrorTexture = false)
        {
            if (!cubes.IndexInRange(index))
                throw new IndexOutOfRangeException();


            if (cubes[index] is CubeMesh cubeMesh)
            cubes[index] = cubeMesh.SetCube(new Cube(position, size, uv, inflate, mirrorTexture, FlipZMapping));
        }

        internal Vector3 GetCenter(int index)
        {
            if (!cubes.IndexInRange(index))
                throw new IndexOutOfRangeException();

            return cubes[index] is CubeMesh c ? c.Center + Offset : Vector3.Zero;
        }
         
        internal BoundingBox GetCubeBoundingBox(int index)
        {
            if (!cubes.IndexInRange(index))
                throw new IndexOutOfRangeException();

            Cube cube = cubes[index].GetCube();
            return cube.GetBoundingBox(Transform);
        }

        internal Vector3 GetFaceCenter(int index, Cube.Face face)
        {
            if (!cubes.IndexInRange(index))
                throw new IndexOutOfRangeException();
            Cube cube = cubes[index].GetCube();
            return Vector3.TransformPosition(cube.GetFaceCenter(face), Transform);
        }

        internal void SetVisible(int index, bool visible)
        {
            if (!cubes.IndexInRange(index))
                throw new IndexOutOfRangeException();
            if (cubes[index].ShouldRender == visible)
                return;
            cubes[index] = cubes[index].SetVisible(visible);
        }

        public void Add(CubeMesh item) => cubes.Add(item);

        public void Clear() => cubes.Clear();

        public bool Contains(CubeMesh item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(CubeMesh[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool Remove(CubeMesh item) => cubes.Remove(item);

        public IEnumerator<CubeMesh> GetEnumerator() => cubes.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
