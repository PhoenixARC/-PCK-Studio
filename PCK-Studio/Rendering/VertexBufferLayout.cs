using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;

namespace PckStudio.Rendering
{
    struct VertexBufferElement
    {
        public readonly VertexAttribPointerType Type;
        public readonly int Count;
        public readonly bool Normalize;

        public static int GetStrideSize(VertexAttribPointerType type)
        {
            return type switch
            {
                VertexAttribPointerType.Int => 4,
                VertexAttribPointerType.UnsignedInt => 4,
                VertexAttribPointerType.Float => 4,
                _ => 0
            };
        }

        public VertexBufferElement(VertexAttribPointerType type, int count, bool normalize)
        {
            Type = type;
            Count = count;
            Normalize = normalize;
        }
    }

    internal struct VertexBufferLayout
    {
        private List<VertexBufferElement> elements;
        private int stride;


        public VertexBufferLayout()
        {
            elements = new List<VertexBufferElement>();
            stride = 0;
        }

        public ReadOnlyCollection<VertexBufferElement> GetElements()
        {
            return elements.AsReadOnly();
        }

        public void Add<T>(int count)
        {
            if (typeof(T).Equals(typeof(float)))
            {
                elements.Add(new VertexBufferElement(VertexAttribPointerType.Float, count, false));
                stride += count * VertexBufferElement.GetStrideSize(VertexAttribPointerType.Float);
            }
            if (typeof(T).Equals(typeof(int)))
            {
                elements.Add(new VertexBufferElement(VertexAttribPointerType.Int, count, false));
                stride += count * VertexBufferElement.GetStrideSize(VertexAttribPointerType.Int);
            }
            if (typeof(T).Equals(typeof(uint)))
            {
                elements.Add(new VertexBufferElement(VertexAttribPointerType.UnsignedInt, count, false));
                stride += count * VertexBufferElement.GetStrideSize(VertexAttribPointerType.UnsignedInt);
            }
        }

        internal int GetStride()
        {
            return stride;
        }
    }
}
