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
namespace PckStudio.Rendering
{
    internal class CubeMesh : GenericMesh<TextureVertex>
    {
        internal bool ShouldRender { get; } = true;

        private Cube _cube;

        public Vector3 Center => _cube.Center;

        internal static int[] IndicesData { get; } = [
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

        public CubeMesh(Cube cube) : this(cube, true)
        {
        }

        private CubeMesh(Cube cube, bool visible)
            : base(nameof(CubeMesh), OpenTK.Graphics.OpenGL.PrimitiveType.Triangles)
        {
            ShouldRender = visible;
            _cube = cube;
            indices.AddRange(IndicesData);
            transform = Matrix4.Identity;
            SetData();
        }

        public CubeMesh SetCube(Cube cube)
        {
            _ = cube ?? throw new ArgumentNullException(nameof(cube));
            return new CubeMesh(cube, ShouldRender);
        }

        public CubeMesh SetVisible(bool visible) => new CubeMesh(_cube, visible);

        public Cube GetCube() => _cube;

        private TextureVertex[] GetCubeVertexData()
        {
            int mirror = Convert.ToInt32(_cube.MirrorTexture);
            List<TextureVertex> vertices = new List<TextureVertex>();

            Vector2 uv = _cube.Uv;

            BoundingBox boundingBox = _cube.GetBoundingBox();
            Vector3 from = boundingBox.Start.ToOpenTKVector();
            Vector3 to   = boundingBox.End.ToOpenTKVector();

            Vector3 size = _cube.Size;

            var back = new TextureVertex[4]
            {
                // Back
                new TextureVertex(new Vector3(from.X,   to.Y, to.Z), new Vector2(uv.X + size.Z * 2 + size.X + size.X * (1 - mirror), uv.Y + size.Z + size.Y)),
                new TextureVertex(new Vector3(  to.X,   to.Y, to.Z), new Vector2(uv.X + size.Z * 2 + size.X + size.X * mirror, uv.Y + size.Z + size.Y)),
                new TextureVertex(new Vector3(  to.X, from.Y, to.Z), new Vector2(uv.X + size.Z * 2 + size.X + size.X * mirror, uv.Y + size.Z)),
                new TextureVertex(new Vector3(from.X, from.Y, to.Z), new Vector2(uv.X + size.Z * 2 + size.X + size.X * (1 - mirror), uv.Y + size.Z))
            };
            var front = new TextureVertex[4]
            {
                // Front
                new TextureVertex(new Vector3(from.X,   to.Y, from.Z), new Vector2(uv.X + size.Z + size.X * mirror, uv.Y + size.Z + size.Y)),
                new TextureVertex(new Vector3(  to.X,   to.Y, from.Z), new Vector2(uv.X + size.Z + size.X * (1 - mirror), uv.Y + size.Z + size.Y)),
                new TextureVertex(new Vector3(  to.X, from.Y, from.Z), new Vector2(uv.X + size.Z + size.X * (1 - mirror), uv.Y + size.Z)),
                new TextureVertex(new Vector3(from.X, from.Y, from.Z), new Vector2(uv.X + size.Z + size.X * mirror, uv.Y + size.Z)),
            };
            var top = new TextureVertex[4]
            {
                // Top
                new TextureVertex(new Vector3(from.X, from.Y, from.Z), new Vector2(uv.X + size.Z + size.X * mirror, uv.Y + size.Z)),
                new TextureVertex(new Vector3(from.X, from.Y,   to.Z), new Vector2(uv.X + size.Z + size.X * mirror, uv.Y)),
                new TextureVertex(new Vector3(  to.X, from.Y,   to.Z), new Vector2(uv.X + size.Z + size.X * (1 - mirror), uv.Y)),
                new TextureVertex(new Vector3(  to.X, from.Y, from.Z), new Vector2(uv.X + size.Z + size.X * (1 - mirror), uv.Y + size.Z)),
            };
            var bottom = new TextureVertex[4]
            {
                // Bottom
                new TextureVertex(new Vector3(  to.X, to.Y, from.Z), new Vector2(uv.X + size.Z + size.X + size.X * (1 - mirror), uv.Y + (_cube.FlipZMapping ? size.Z : 0))),
                new TextureVertex(new Vector3(  to.X, to.Y,   to.Z), new Vector2(uv.X + size.Z + size.X + size.X * (1 - mirror), uv.Y + (!_cube.FlipZMapping ? size.Z : 0))),
                new TextureVertex(new Vector3(from.X, to.Y,   to.Z), new Vector2(uv.X + size.Z + size.X + size.X * mirror, uv.Y + (!_cube.FlipZMapping ? size.Z : 0))),
                new TextureVertex(new Vector3(from.X, to.Y, from.Z), new Vector2(uv.X + size.Z + size.X + size.X * mirror, uv.Y + (_cube.FlipZMapping ? size.Z : 0))),
            };
            var left = new TextureVertex[4]
            {
                // Left
                new TextureVertex(new Vector3(_cube.MirrorTexture ? from.X : to.X, from.Y, from.Z), new Vector2(uv.X + size.X + size.Z, uv.Y + size.Z)),
                new TextureVertex(new Vector3(_cube.MirrorTexture ? from.X : to.X, to.Y  , from.Z), new Vector2(uv.X + size.X + size.Z, uv.Y + size.Z + size.Y)),
                new TextureVertex(new Vector3(_cube.MirrorTexture ? from.X : to.X, to.Y  , to.Z), new Vector2(uv.X + size.X + size.Z * 2, uv.Y + size.Z + size.Y)),
                new TextureVertex(new Vector3(_cube.MirrorTexture ? from.X : to.X, from.Y, to.Z), new Vector2(uv.X + size.X + size.Z * 2, uv.Y + size.Z)),
            };
            var right = new TextureVertex[4]
            {
                // Right
                new TextureVertex(new Vector3(_cube.MirrorTexture ? to.X : from.X, from.Y, from.Z), new Vector2(uv.X + size.Z, uv.Y + size.Z)),
                new TextureVertex(new Vector3(_cube.MirrorTexture ? to.X : from.X, to.Y  , from.Z), new Vector2(uv.X + size.Z, uv.Y + size.Z + size.Y)),
                new TextureVertex(new Vector3(_cube.MirrorTexture ? to.X : from.X, to.Y  , to.Z), new Vector2(uv.X, uv.Y + size.Z + size.Y)),
                new TextureVertex(new Vector3(_cube.MirrorTexture ? to.X : from.X, from.Y, to.Z), new Vector2(uv.X, uv.Y + size.Z)),
            };
            
            vertices.AddRange(back);
            vertices.AddRange(front);
            vertices.AddRange(top);
            vertices.AddRange(bottom);
            vertices.AddRange(left);
            vertices.AddRange(right);

            return vertices.ToArray();
        }

        internal override void SetData()
        {
            vertices.Clear();
            vertices.AddRange(GetCubeVertexData());
        }
    }
}