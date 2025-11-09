using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PckStudio.Core.Interfaces;
using PckStudio.Core.Skin;

namespace PckStudio.Core
{
    public enum DLCSkinPackageOrder
    {
        ById,
        CapesFirst,
        SkinsFirst
    }

    public sealed class DLCSkinPackage : DLCPackage
    {
        public DLCSkinPackageOrder SkinPackageOrder { get; set; } = DLCSkinPackageOrder.CapesFirst;

        private readonly Dictionary<SkinIdentifier, Skin.Skin> _skins;

        internal DLCSkinPackage(string name, int identifier, IEnumerable<Skin.Skin> skins, IDLCPackageLocationInfo packageInfo, IDLCPackage parentPackage)
            : base(name, identifier, packageInfo, parentPackage)
        {
            _skins = skins.ToDictionary(skin => skin.Identifier);
        }

        internal DLCSkinPackage(string name, int identifier, IDLCPackage parentPackage = null)
            : this(name, identifier, Enumerable.Empty<Skin.Skin>(), null, parentPackage)
        {
        }

        internal static IDLCPackage CreateEmpty(string name, int identifier, IDLCPackage parentPackage = null) => new DLCSkinPackage(name, identifier, parentPackage);
        internal static IDLCPackage CreateEmpty(IDLCPackage parentPackage) => CreateEmpty(parentPackage.Name, parentPackage.Identifier, parentPackage);

        public bool TryGetSkin(SkinIdentifier skinIdentifier, out Skin.Skin skin) => _skins.TryGetValue(skinIdentifier, out skin);

        public bool ContainsSkin(SkinIdentifier skinIdentifier) => _skins.ContainsKey(skinIdentifier);

        public void AddSkin(Skin.Skin skin) => _skins.Add(skin.Identifier, skin);

        public bool RemoveSkin(SkinIdentifier skinIdentifier) => _skins.Remove(skinIdentifier);
        
        public Skin.Skin GetSkin(SkinIdentifier skinIdentifier) => _skins[skinIdentifier];

        public IReadOnlyCollection<Skin.Skin> GetSkins() => _skins.Values.Cast<Skin.Skin>().ToArray();

        public override DLCPackageType GetDLCPackageType() => DLCPackageType.SkinPack;
    }
}