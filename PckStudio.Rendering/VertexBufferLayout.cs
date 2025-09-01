using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace PckStudio.Rendering
{
    public enum ShaderDataType
    {
        Float,
        Float2,
        Float3,
        Float4,
        
        Int,
        Int2,
        Int3,
        Int4,

        Mat2,
        Mat3,
        Mat4,
    }

    public struct LayoutElement
    {
        public readonly ShaderDataType Type;
        public readonly bool Normalize;

        public int Size => GetSize(Type);

        public int ComponentCount => GetComponentCount(Type);

        private static int GetSize(ShaderDataType type)
        {
            return type switch
            {
                ShaderDataType.Int    => 1 * 4,
                ShaderDataType.Int2   => 2 * 4,
                ShaderDataType.Int3   => 3 * 4,
                ShaderDataType.Int4   => 4 * 4,

                ShaderDataType.Float  => 1 * 4,
                ShaderDataType.Float2 => 2 * 4,
                ShaderDataType.Float3 => 3 * 4,
                ShaderDataType.Float4 => 4 * 4,

                ShaderDataType.Mat2 => 2 * 2 * 4,
                ShaderDataType.Mat3 => 3 * 3 * 4,
                ShaderDataType.Mat4 => 4 * 4 * 4,
                _ => 0
            };
        }

        private static int GetComponentSize(ShaderDataType type)
        {
            return type switch
            {
                ShaderDataType.Int    => 4,
                ShaderDataType.Int2   => 4,
                ShaderDataType.Int3   => 4,
                ShaderDataType.Int4   => 4,

                ShaderDataType.Float  => 4,
                ShaderDataType.Float2 => 4,
                ShaderDataType.Float3 => 4,
                ShaderDataType.Float4 => 4,

                ShaderDataType.Mat2 => 2 * 4,
                ShaderDataType.Mat3 => 3 * 4,
                ShaderDataType.Mat4 => 4 * 4,
                _ => 0
            };
        }

        private static int GetComponentCount(ShaderDataType type)
        {
            return type switch
            {
                ShaderDataType.Int    => 1,
                ShaderDataType.Int2   => 2,
                ShaderDataType.Int3   => 3,
                ShaderDataType.Int4   => 4,

                ShaderDataType.Float  => 1,
                ShaderDataType.Float2 => 2,
                ShaderDataType.Float3 => 3,
                ShaderDataType.Float4 => 4,

                ShaderDataType.Mat2 => 2 * 2,
                ShaderDataType.Mat3 => 3 * 3,
                ShaderDataType.Mat4 => 4 * 4,
                _ => 0
            };
        }

        public LayoutElement(ShaderDataType type) : this(type, false) { }
        
        public LayoutElement(ShaderDataType type, bool normalize)
        {
            Type = type;
            Normalize = normalize;
        }

        public static implicit operator LayoutElement(ShaderDataType type) => new LayoutElement(type);
    }

    public struct VertexBufferLayout
    {
        private List<LayoutElement> elements;
        private int stride;

        public VertexBufferLayout()
        {
            elements = new List<LayoutElement>();
            stride = 0;
        }

        public readonly ReadOnlyCollection<LayoutElement> GetElements()
        {
            return elements.AsReadOnly();
        }

        public VertexBufferLayout Add(ShaderDataType type)
        {
            var element = new LayoutElement(type);
            elements.Add(element);
            stride += element.Size;
            return this;
        }

        internal readonly int GetStride()
        {
            return stride;
        }
    }
}
