using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using OMI.Formats.GameRule;
using PckStudio.Core.FileFormats;
using PckStudio.Core.GameRule;
using PckStudio.Core.Interfaces;

namespace PckStudio.Core.DLC
{
    public sealed class DLCMashUpPackage : DLCPackage
    {
        public override string Description { get; }

        private IDLCPackage _skinPackage;
        private IDLCPackage _texturePackage;
        private MapData _mapData;
        private IDictionary<string, byte[]> _audioData;
        private PckAudioFile _pckAudio;

        internal DLCMashUpPackage(string name, string description, int identifier, IDLCPackageSerialization packageInfo, AbstractGameRule gameRule, IDLCPackage parentPackage, IDLCPackage skinPackage = null, IDLCPackage texturePackage = null)
            : base(name, identifier, packageInfo, parentPackage)
        {
            Description = description;
            _skinPackage = skinPackage;
            _texturePackage = texturePackage;
        }

        internal DLCMashUpPackage(string name, string description, int identifier)
            : this(name, description, identifier, null, new RootGameRule(), null)
        {
            _skinPackage = DLCSkinPackage.CreateEmpty(this);
            _texturePackage = DLCTexturePackage.CreateDefaultPackage(this);
            _audioData = new Dictionary<string, byte[]>();
            _pckAudio = new PckAudioFile();
        }

        public IDLCPackage GetSkinPackage() => _skinPackage;
        public IDLCPackage GetTexturePackage() => _texturePackage;

        public override DLCPackageType GetDLCPackageType() => DLCPackageType.MashUpPack;
    }
}
