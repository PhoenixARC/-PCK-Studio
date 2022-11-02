using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace PckStudio.Classes.FileTypes
{
    [Serializable]
    internal class ModelNotFoundException : Exception
    {
        public ModelNotFoundException()
        {
        }

        public ModelNotFoundException(string message) : base(message)
        {
        }

        public ModelNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ModelNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
    public class ModelFile
    {
        public List<Model> Models { get; } = new List<Model>();

        public void AddModel(Model model)
        {
            Models.Add(model);
        }

        bool Contains(string name) => Models.FindIndex(m => m.name == name) > -1;

        /// <exception cref="ModelNotFoundException"></exception>
        Model GetModelByName(string name)
        {
            return Contains(name) ? Models.First(m => m.name.Equals(name)) : throw new ModelNotFoundException(nameof(name));
        }

        public struct Model
        {
            public readonly string name;
            public Size textureSize;
            public List<Part> parts { get; } = new List<Part>();

            public Model(string name, int textureWidth, int textureHeight)
            {
                this.name = name;
                textureSize = new Size(textureWidth, textureHeight);
            }

            public struct Part
            {
                string name;
                (float x, float y, float z) position;
                (float yaw, float pitch, float roll) rotation;
                List<Box> Boxes { get; } = new List<Box>();

                public struct Box
                {
                    (float x, float y, float z) Position;
                    (int width, int height, int length) Size;
                    float U, V;
                    float Scale;
                    bool Mirror;

                    public Box((float x, float y, float z) position,
                               (int width, int height, int length) size,
                               float u, float v, float scale, bool mirror)
                    {
                        Position = position;
                        Size = size;
                        U = u;
                        V = v;
                        Scale = scale;
                        Mirror = mirror;
                    }
                }

                public Part(string name,
                    (float x, float y, float z) pos,
                    (float yaw, float pitch, float roll) rot) : this(name)
                {
                    position = pos;
                    rotation = rot;
                }

                public Part(string name)
                {
                    this.name = name;
                    this.position = (0, 0, 0);
                    this.rotation = (0, 0, 0);
                }

                public void AddBox((float x, float y, float z) position,
                            (int width, int height, int length) size,
                            float u, float v, float scale, bool mirror)
                {
                    Boxes.Add(new Box(position, size, u, v, scale, mirror));
                }

            }

            public void AddPart(Part part)
            {
                parts.Add(part);
            }
        }
    }
}
