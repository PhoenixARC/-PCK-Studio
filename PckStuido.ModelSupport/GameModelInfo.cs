using System.Collections.Generic;
using OMI.Formats.Model;

namespace PckStudio.ModelSupport
{
    public sealed class GameModelInfo
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
