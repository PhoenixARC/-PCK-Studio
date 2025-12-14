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
        private AbstractGameRule _gameRule;
        public bool HasAudioData => _pckAudio is not null && _audioData.Count > 0;

        private IDLCPackage _skinPackage;
        private IDLCPackage _texturePackage;
        private MapData _mapData;
        private IDictionary<string, byte[]> _audioData;
        private PckAudioFile _pckAudio;

        internal DLCMashUpPackage(string name, string description, int identifier, AbstractGameRule gameRule, MapData mapData, PckAudioFile pckAudio, IDictionary<string, byte[]> audioData, IDLCPackage parentPackage, IDLCPackage skinPackage = null, IDLCPackage texturePackage = null)
            : base(name, identifier, parentPackage)
        {
            Description = description;
            _gameRule = gameRule;
            _skinPackage = skinPackage;
            _texturePackage = texturePackage;
            _mapData = mapData;
            _audioData = new Dictionary<string, byte[]>();
            _pckAudio = pckAudio;
            _audioData = audioData;
        }

        internal DLCMashUpPackage(string name, string description, int identifier)
            : this(name, description, identifier, new RootGameRule(), null, new PckAudioFile(), null, null)
        {
            _skinPackage = DLCSkinPackage.CreateEmpty(this);
            _texturePackage = DLCTexturePackage.CreateDefaultPackage(this);
        }

        public override DLCPackageType GetDLCPackageType() => DLCPackageType.MashUpPack;

        public IDLCPackage GetSkinPackage() => _skinPackage;
        public IDLCPackage GetTexturePackage() => _texturePackage;

        public bool AddAudio(string name, byte[] audioData, PckAudioFile.Category category)
        {
            if (_audioData.ContainsKey(name) || !_pckAudio.HasCategory(category))
                return false;
            if (_pckAudio.TryGetCategory(category, out PckAudioFile.AudioCategory audioCategory))
            {
                audioCategory.SongNames.Add(name);
                return true;
            }
            return false;
        }

        public AbstractGameRule GetGameRule() => _gameRule;
        internal PckAudioFile GetAudioPack() => _pckAudio;

        internal NamedData<byte[]>[] GetAudioFiles() => _audioData.Select(kv => new NamedData<byte[]>(kv.Key, kv.Value)).ToArray();
    }
}
