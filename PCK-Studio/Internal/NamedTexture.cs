using System;
using System.Collections.Generic;
using System.Drawing;
namespace PckStudio.Internal
{
    internal readonly struct NamedTexture
    {
        public readonly string Name;
        public readonly Image Texture;

        public NamedTexture(string name, Image texture)
        {
            Name = name;
            Texture = texture;
        }
    }
}
