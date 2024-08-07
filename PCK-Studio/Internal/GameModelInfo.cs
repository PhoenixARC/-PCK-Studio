using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
