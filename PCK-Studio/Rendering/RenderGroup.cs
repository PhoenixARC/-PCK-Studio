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
        internal string Name { get; }

        private List<TextureVertex> vertices;
        private List<uint> indices;
        private uint indicesOffset;

        private VertexArray vertexArray;
        private VertexBuffer<TextureVertex> vertexBuffer;
        private IndexBuffer indexBuffer;
        private readonly VertexBufferLayout layout;
        private readonly PrimitiveType drawType;
        private const int vertexCountPerBox = 16;

        internal RenderGroup(string name, PrimitiveType type = PrimitiveType.Quads)
        {
            Name = name;
            drawType = type;
            vertices = new List<TextureVertex>(10);
            indices = new List<uint>(10);
            indicesOffset = 0;
            layout = new VertexBufferLayout();
            layout.Add<float>(3);
            layout.Add<float>(2);
        }

        internal RenderBuffer GetRenderBuffer()
        {
            //vertexArray?.Dispose();
            indexBuffer?.Dispose();
            vertexBuffer.Dispose();

            var vertexData = vertices.ToArray();
            vertexBuffer = new VertexBuffer<TextureVertex>(vertexData, vertexData.Length * TextureVertex.SizeInBytes);

            vertexArray ??= new VertexArray();

            vertexArray.AddBuffer(vertexBuffer, layout);

            indexBuffer = new IndexBuffer(indices.ToArray());

            return new RenderBuffer(vertexArray, indexBuffer, drawType);
        }

        // TODO: add mirroring support
        internal void AddBox(SkinBOX skinBox)
        {
            AddBox(skinBox.Pos.ToOpenTKVector(), skinBox.Size.ToOpenTKVector(), skinBox.UV.ToOpenTKVector(), skinBox.Scale);
        }


        internal void AddBox(Vector3 position, Vector3 size, Vector2 uv, float scale = 0f)
        {
            var vertexData = GetCubeVertexData(position, size, uv, scale);

            vertices.AddRange(vertexData);

            var indexStorage = new uint[] {
                 0 + indicesOffset,  1  + indicesOffset,  2 + indicesOffset,  3 + indicesOffset, // Face 1 (Front)
                 4 + indicesOffset,  5  + indicesOffset,  6 + indicesOffset,  7 + indicesOffset, // Face 2 (Back)
                 0 + indicesOffset,  8  + indicesOffset,  9 + indicesOffset,  3 + indicesOffset, // Face 3 (Left)
                 1 + indicesOffset,  5  + indicesOffset,  6 + indicesOffset,  2 + indicesOffset, // Face 4 (Right)
                 0 + indicesOffset,  1  + indicesOffset, 10 + indicesOffset, 11 + indicesOffset, // Face 5 (Top)
                13 + indicesOffset, 12  + indicesOffset, 14 + indicesOffset, 15 + indicesOffset  // Face 6 (Bottom)
                };
            indicesOffset += vertexCountPerBox;
            indices.AddRange(indexStorage);
        }
        
        internal void ReplaceBox(int index, Vector3 position, Vector3 size, Vector2 uv, float scale = 0f)
        {
            if (index * vertexCountPerBox > vertices.Count || index < 0)
                throw new IndexOutOfRangeException();

            vertices.InsertRange(index * vertexCountPerBox, GetCubeVertexData(position, size, uv, scale));
        }

        private TextureVertex[] GetCubeVertexData(Vector3 position, Vector3 size, Vector2 uv, float scale)
        {
            return new TextureVertex[]
            {
                new TextureVertex(new Vector3(position.X         , size.Y + position.Y, size.Z + position.Z) * (scale + 1f), new Vector2(uv.X + size.Z, uv.Y + size.Z)),
                new TextureVertex(new Vector3(size.X + position.X, size.Y + position.Y, size.Z + position.Z) * (scale + 1f), new Vector2(uv.X + size.Z + size.X, uv.Y + size.Z)),
                new TextureVertex(new Vector3(size.X + position.X, position.Y         , size.Z + position.Z) * (scale + 1f), new Vector2(uv.X + size.Z + size.X, uv.Y + size.Z + size.Y)),
                new TextureVertex(new Vector3(position.X         , position.Y         , size.Z + position.Z) * (scale + 1f), new Vector2(uv.X + size.Z, uv.Y + size.Z + size.Y)),

                new TextureVertex(new Vector3(position.X         , size.Y + position.Y, position.Z         ) * (scale + 1f), new Vector2(uv.X + size.Z * 2 + size.X * 2, uv.Y + size.Z)),
                new TextureVertex(new Vector3(size.X + position.X, size.Y + position.Y, position.Z         ) * (scale + 1f), new Vector2(uv.X + size.Z * 2 + size.X, uv.Y + size.Z)),
                new TextureVertex(new Vector3(size.X + position.X, position.Y         , position.Z         ) * (scale + 1f), new Vector2(uv.X + size.Z * 2 + size.X, uv.Y + size.Z + size.Y)),
                new TextureVertex(new Vector3(position.X         , position.Y         , position.Z         ) * (scale + 1f), new Vector2(uv.X + size.Z * 2 + size.X * 2, uv.Y + size.Z + size.Y)),

                new TextureVertex(new Vector3(position.X         , size.Y + position.Y, position.Z         ) * (scale + 1f), new Vector2(uv.X, uv.Y + size.Z)),
                new TextureVertex(new Vector3(position.X         , position.Y         , position.Z         ) * (scale + 1f), new Vector2(uv.X, uv.Y + size.Z + size.Y)),
                new TextureVertex(new Vector3(size.X + position.X, size.Y + position.Y, position.Z         ) * (scale + 1f), new Vector2(uv.X + size.Z + size.X, uv.Y)),
                new TextureVertex(new Vector3(position.X         , size.Y + position.Y, position.Z         ) * (scale + 1f), new Vector2(uv.X + size.Z, uv.Y)),

                new TextureVertex(
                    new Vector3(position.X         , position.Y         , size.Z + position.Z) * (scale + 1f),
                    new Vector2(uv.X + size.Z + size.X, uv.Y)),
                new TextureVertex(
                    new Vector3(size.X + position.X, position.Y         , size.Z + position.Z) * (scale + 1f),
                    new Vector2(uv.X + size.Z + size.X * 2, uv.Y)),
                new TextureVertex(
                    new Vector3(position.X         , position.Y         , position.Z         ) * (scale + 1f),
                    new Vector2(uv.X + size.Z + size.X, uv.Y  + size.Z)),
                new TextureVertex(
                    new Vector3(size.X + position.X, position.Y         , position.Z         ) * (scale + 1f),
                    new Vector2(uv.X + size.Z + size.X * 2, uv.Y  + size.Z)),
            };
        }
    }
}