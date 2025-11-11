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
        private AbstractGameRule _gameRule;
        private IDictionary<string, byte[]> _audioData;
        private PckAudioFile _pckAudio;
        private NamedData<byte[]> _savegameData;

        internal DLCMashUpPackage(string name, string description, int identifier, IDLCPackageSerialization packageInfo, AbstractGameRule gameRule, IDLCPackage parentPackage, IDLCPackage skinPackage = null, IDLCPackage texturePackage = null)
            : base(name, identifier, packageInfo, parentPackage)
        {
            Description = description;
            _gameRule = gameRule;
            _skinPackage = skinPackage;
            _texturePackage = texturePackage;
        }

        internal DLCMashUpPackage(string name, string description, int identifier)
            : this(name, description, identifier, null, new RootGameRule(), null)
        {
            _skinPackage = DLCSkinPackage.CreateEmpty(this);
            _texturePackage = DLCTexturePackage.CreateDefaultPackage(this);
            _savegameData = new NamedData<byte[]>("world.mcs", Array.Empty<byte>());
            _audioData = new Dictionary<string, byte[]>();
            _pckAudio = new PckAudioFile();

            //! TODO(null): only create grf when adding savefile ?...
            _gameRule.AddRule(new NamedRule("MapOptions",
                new GameRuleFile.IntParameter("seed", 0),
                new GameRuleFile.GameRuleParameter("baseSaveName", _savegameData.Name),
                new GameRuleFile.BoolParameter("flatworld", false),
                new GameRuleFile.IntParameter("texturePackId", Identifier))
                );
            _gameRule.AddRule(LevelRules.GetDefault(pos: Vector3.Zero, rot: Vector2.Zero));
        }

        public IDLCPackage GetSkinPackage() => _skinPackage;
        public IDLCPackage GetTexturePackage() => _texturePackage;

        public override DLCPackageType GetDLCPackageType() => DLCPackageType.MashUpPack;
    }
}
