using System.Collections.Generic;
using OMI.Formats.Model;

namespace PckStudio.Internal
{
    internal class GameModelInfo
    {
        public Model Model { get; }

        public IEnumerable<NamedTexture> Textures { get; }

        public GameModelInfo(Model model, IEnumerable<NamedTexture> textures)
        {
            Model = model;
            Textures = textures;
        }

    }
}
