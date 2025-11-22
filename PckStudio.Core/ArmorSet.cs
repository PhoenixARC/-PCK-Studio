using System.Drawing;
using PckStudio.Core.Skin;

namespace PckStudio.Core
{
    public sealed class ArmorSet
    {
        public string Name { get; }
        public Image BaseTexture => _layers[0];
        public Image Layer => _layers[1];
        private Image[] _layers = new Image[2];

        public ArmorSet(string name, Image baseTexture, Image overlay)
        {
            Name = name;
            _layers[0] = baseTexture;
            _layers[1] = overlay;
        }
    }
}