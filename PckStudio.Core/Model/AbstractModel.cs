using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiscordRPC;

namespace PckStudio.Core.Model
{
    public class AbstractModel
    {
        public string Name { get; }

        public Size TextureSize { get; }
        public NamedData<Image> DefaultTexture { get; }
        public IReadOnlyDictionary<string, Image> Textures { get; }

        private IDictionary<string, AbstractModelPart> _parts;

        public AbstractModel(string name, Size textureSize, NamedData<Image> defaultTexture, IReadOnlyDictionary<string, Image> textures)
        {
            Name = name;
            TextureSize = textureSize;
            DefaultTexture = defaultTexture;
            Textures = textures;
        }

        internal bool AddPart(AbstractModelPart abstractModelPart)
        {
            if (abstractModelPart is null || _parts.ContainsKey(abstractModelPart.Name))
                return false;
            _parts.Add(abstractModelPart.Name, abstractModelPart);
            return true;
        }

        internal void AddParts(IEnumerable<AbstractModelPart> abstractModelParts)
        {
            foreach (AbstractModelPart abstractModelPart in abstractModelParts)
            {
                AddPart(abstractModelPart);
            }
        }
    }
}
