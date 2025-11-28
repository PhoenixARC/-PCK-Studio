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
        private AbstractGameRule _gameRule { get; }
        public bool HasAudioData => _pckAudio is not null && _audioData.Count > 0;

        private IDLCPackage _skinPackage;
        private IDLCPackage _texturePackage;
        private MapData _mapData;
        private IDictionary<string, byte[]> _audioData;
        private PckAudioFile _pckAudio;

        internal DLCMashUpPackage(string name, string description, int identifier, AbstractGameRule gameRule, IDLCPackage parentPackage, IDLCPackage skinPackage = null, IDLCPackage texturePackage = null)
            : base(name, identifier, parentPackage)
        {
            Description = description;
            _gameRule = gameRule;
            _skinPackage = skinPackage;
            _texturePackage = texturePackage;
        }

        internal DLCMashUpPackage(string name, string description, int identifier)
            : this(name, description, identifier, new RootGameRule(), null)
        {
            _skinPackage = DLCSkinPackage.CreateEmpty(this);
            _texturePackage = DLCTexturePackage.CreateDefaultPackage(this);
            _audioData = new Dictionary<string, byte[]>();
            _pckAudio = new PckAudioFile();
        }

        public IDLCPackage GetSkinPackage() => _skinPackage;
        public IDLCPackage GetTexturePackage() => _texturePackage;
        public AbstractGameRule GetGameRule() => _gameRule;
        public PckAudioFile GetAudioPack() => _pckAudio;

        public override DLCPackageType GetDLCPackageType() => DLCPackageType.MashUpPack;

        internal NamedData<byte[]>[] GetAudioFiles() => _audioData.Select(kv => new NamedData<byte[]>(kv.Key, kv.Value)).ToArray();
    }
}
