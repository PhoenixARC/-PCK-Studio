using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using PckStudio.Extensions;
using PckStudio.Internal;

namespace PckStudio.Rendering
{
    internal class RenderGroup
    {
        internal readonly string Name;

        private List<TextureVertex> vertices;
        private List<uint> indices;

        private uint indicesOffset;

        internal RenderGroup(string name)
        {
            Name = name;
            vertices = new List<TextureVertex>();
            indices = new List<uint>();
            indicesOffset = 0;
        }

        internal RenderBuffer GetRenderBuffer()
        {
            var vertexData = vertices.ToArray();
            var buffer = new VertexBuffer<TextureVertex>(vertexData, vertexData.Length * TextureVertex.SizeInBytes);
            var layout = new VertexBufferLayout();
            layout.Add<float>(3);
            layout.Add<float>(2);

            var vertexArray = new VertexArray();

            vertexArray.AddBuffer(buffer, layout);

            var indexBuffer = new IndexBuffer(indices.ToArray());

            return new RenderBuffer(vertexArray, indexBuffer, PrimitiveType.Quads);
        }

        internal void AddBox(SkinBOX skinBOX, Vector2 textureScale)
        {
            var position = skinBOX.Pos.ToOpenTKVector();
            var size = skinBOX.Size.ToOpenTKVector();
            var uv = skinBOX.UV.ToOpenTKVector();

            var vertexData = new TextureVertex[]
            {
                new TextureVertex(new Vector3(position.X         , size.Y + position.Y, size.Z + position.Z), new Vector2(uv.X + size.Z, uv.Y  + size.Z) * textureScale),
                new TextureVertex(new Vector3(size.X + position.X, size.Y + position.Y, size.Z + position.Z), new Vector2(uv.X + size.Z + size.X, uv.Y + size.Z) * textureScale),
                new TextureVertex(new Vector3(size.X + position.X, position.Y, size.Z + position.Z), new Vector2(uv.X + size.Z + size.X, uv.Y + size.Z + size.Y) * textureScale),
                new TextureVertex(new Vector3(position.X         , position.Y, size.Z + position.Z), new Vector2(uv.X + size.Z, uv.Y + size.Z + size.Y) * textureScale),

                new TextureVertex(new Vector3(position.X         , size.Y + position.Y, position.Z), new Vector2(uv.X + size.Z * 2 + size.X * 2, uv.Y + size.Z) * textureScale),
                new TextureVertex(new Vector3(size.X + position.X, size.Y + position.Y, position.Z), new Vector2(uv.X + size.Z * 2 + size.X, uv.Y + size.Z) * textureScale),
                new TextureVertex(new Vector3(size.X + position.X, position.Y, position.Z), new Vector2(uv.X + size.Z * 2 + size.X, uv.Y + size.Z + size.Y) * textureScale),
                new TextureVertex(new Vector3(position.X         , position.Y, position.Z), new Vector2(uv.X + size.Z * 2 + size.X * 2, uv.Y + size.Z + size.Y) * textureScale),

                new TextureVertex(new Vector3(position.X         , size.Y + position.Y, position.Z), new Vector2(uv.X, uv.Y + size.Z) * textureScale),
                new TextureVertex(new Vector3(position.X         , position.Y, position.Z), new Vector2(uv.X, uv.Y + size.Z + size.Y) * textureScale),
                new TextureVertex(new Vector3(size.X + position.X, size.Y + position.Y, position.Z), new Vector2(uv.X + size.Z + size.X, uv.Y) * textureScale),
                new TextureVertex(new Vector3(position.X         , size.Y + position.Y, position.Z), new Vector2(uv.X + size.Z, uv.Y) * textureScale),

                new TextureVertex(new Vector3(position.X         , position.Y, position.Z), new Vector2(uv.X + size.Z + size.X, uv.Y) * textureScale),
                new TextureVertex(new Vector3(size.X + position.X, position.Y, position.Z), new Vector2(uv.X + size.Z + size.X * 2, uv.Y) * textureScale),
                new TextureVertex(new Vector3(size.X + position.X, position.Y, size.Z + position.Z), new Vector2(uv.X + size.Z + size.X * 2, uv.Y  + size.Z) * textureScale),
                new TextureVertex(new Vector3(position.X         , position.Y, size.Z + position.Z), new Vector2(uv.X + size.Z + size.X, uv.Y  + size.Z) * textureScale),
            };

            vertices.AddRange(vertexData);

            var indexStorage = new uint[] {
                 0 + indicesOffset, 1  + indicesOffset, 2 + indicesOffset, 3 + indicesOffset, // Face 1 (Front)
                 4 + indicesOffset, 5  + indicesOffset, 6 + indicesOffset, 7 + indicesOffset, // Face 2 (Back)
                 0 + indicesOffset, 8  + indicesOffset, 9 + indicesOffset, 3 + indicesOffset, // Face 3 (Left)
                 1 + indicesOffset, 5  + indicesOffset, 6 + indicesOffset, 2 + indicesOffset, // Face 4 (Right)
                 0 + indicesOffset, 1  + indicesOffset, 10 + indicesOffset, 11 + indicesOffset, // Face 5(Top)
                12 + indicesOffset, 13  + indicesOffset, 14 + indicesOffset, 15 + indicesOffset// Face 6 (Bottom)
                };
            indicesOffset += (uint)indexStorage.Length;
            indices.AddRange(indexStorage);
        }
    }
}
