using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using OMI.Formats.GameRule;
using OMI.Formats.Languages;
using OMI.Formats.Pck;
using OMI.Workers.GameRule;
using OMI.Workers.Language;
using OMI.Workers.Pck;
using PckStudio.Core.Deserializer;
using PckStudio.Core.Extensions;
using PckStudio.Core.Interfaces;

namespace PckStudio.Core.DLC
{
    public sealed class DLCManager
    {
        public static DLCManager Default { get; } = new DLCManager(default, default, AvailableLanguages.English);

        public OMI.ByteOrder ByteOrder { get; set; }

        public ConsolePlatform Platform { get; set; }

        /// <summary>
        /// See <see cref="AvailableLanguages"/> for details.
        /// </summary>
        public string PreferredLanguage { get; set; }

        private const string PackageDisplayNameId = "IDS_DISPLAY_NAME";
        private readonly IDictionary<int, IDLCPackage> _openPackages = new Dictionary<int, IDLCPackage>();
        private readonly IDictionary<int, LOCFile> _localisationFiles = new Dictionary<int, LOCFile>();
        private readonly Random _rng = new Random();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="byteOrder"></param>
        /// <param name="platform"></param>
        /// <param name="preferredLanguage">See <see cref="AvailableLanguages"/> for details.</param>
        public DLCManager(OMI.ByteOrder byteOrder, ConsolePlatform platform, string preferredLanguage)
        {
            ByteOrder = byteOrder;
            Platform = platform;
            PreferredLanguage = preferredLanguage;
        }

        public IDLCPackage CreateNewPackage(string name, DLCPackageType packageType)
        {
            int identifier = _rng.Next(8000, GameConstants.MAX_PACK_ID);
            IDLCPackage package = packageType switch
            {
                DLCPackageType.SkinPack    => DLCSkinPackage.CreateEmpty(name, identifier),
                DLCPackageType.TexturePack => DLCTexturePackage.CreateDefaultPackage(name, "", identifier),
                DLCPackageType.MashUpPack  => new DLCMashUpPackage(name, "", identifier),
                //! TODO: implemnt minigame dlc packages -null
                DLCPackageType.MG01        => new DLCBattlePackage(name, identifier),
                DLCPackageType.MG02        => new DLCMiniGamePackage(name, identifier, packageType, MiniGameId.Tumble),
                DLCPackageType.MG03        => new DLCMiniGamePackage(name, identifier, packageType, MiniGameId.Glide),
                DLCPackageType.Invalid     => InvalidDLCPackage.Instance,
                _ => throw new ArgumentException("Unable to create DLC Package of 'Unknown' type."),
            };

            LOCFile localisation = new LOCFile();
            localisation.AddLanguage(PreferredLanguage);
            localisation.AddLocKey(PackageDisplayNameId, name);
            _localisationFiles.Add(identifier, localisation);
            _openPackages.Add(identifier, package);
            
            return package;
        }

        public IDLCPackage OpenDLCPackage(string filepath) => OpenDLCPackage(new FileInfo(filepath));

        public IDLCPackage OpenDLCPackage(FileInfo fileInfo)
        {
            if (!IsValidPckFile(fileInfo))
                return InvalidDLCPackage.Instance;

            using Stream stream = fileInfo.OpenRead();

            PckFileReader fileReader = new PckFileReader(ByteOrder);

            PckFile pckFile = fileReader.FromStream(stream);

            if (!pckFile.TryGetAsset("0", PckAssetType.InfoFile, out PckAsset zeroAsset))
            {
                Trace.TraceError("Could not find asset named:'0'.");
                return new UnknownDLCPackage(fileInfo.Name, pckFile);
            }

            int identifier = (zeroAsset?.HasProperty("PACKID") ?? default) ? zeroAsset.GetProperty("PACKID", int.Parse) : -1;
            if (identifier <= 0 || identifier > GameConstants.MAX_PACK_ID)
            {
                Trace.TraceError($"{nameof(identifier)}({identifier}) was out of range!");
                return new UnknownDLCPackage(fileInfo.Name, pckFile);
            }

            if (_openPackages.ContainsKey(identifier))
                return _openPackages[identifier];

            LOCFile localisation = pckFile.GetAssetsByType(PckAssetType.LocalisationFile).FirstOrDefault()?.GetData(new LOCFileReader());
            if (localisation is null)
                return new UnknownDLCPackage(fileInfo.Name, pckFile);
            _localisationFiles.Add(identifier, localisation);

            IDLCPackage package = ScanForPackageType(fileInfo, identifier, pckFile, localisation, fileReader);
            _openPackages.Add(identifier, package);
            return package;
        }

        internal LOCFile GetLocalisation(int identifier)
        {
            return _localisationFiles.ContainsKey(identifier) ? _localisationFiles[identifier] : default;
        }

        private bool IsValidPckFile(FileInfo fileInfo)
        {
            if (!fileInfo.Exists)
                throw new FileNotFoundException(fileInfo.FullName);

            if (fileInfo.Extension != ".pck")
                throw new FileFormatException("File does not end with '.pck'.");
            return true;
        }

        private IDLCPackage ScanForPackageType(FileInfo fileInfo, int identifier, PckFile pckFile, LOCFile localisation, PckFileReader fileReader)
        {
            bool hasLanguage = localisation?.Languages?.Contains(PreferredLanguage) ?? default;

            string name = hasLanguage && (localisation?.HasLocEntry(PackageDisplayNameId) ?? default)
                ? localisation.GetLocEntry(PackageDisplayNameId, PreferredLanguage) : fileInfo.Name;

            string description = hasLanguage && (localisation?.HasLocEntry(DLCTexturePackage.TexturePackDescriptionId) ?? default)
                ? localisation.GetLocEntry(DLCTexturePackage.TexturePackDescriptionId, PreferredLanguage) : string.Empty;

            bool couldBeTexturePack = fileInfo.Name == "TexturePack.pck";

            bool hasSkins = pckFile.Contains(PckAssetType.SkinFile) || pckFile.Contains(PckAssetType.SkinDataFile);

            DLCPackageType dlcPackageType = hasSkins ? DLCPackageType.SkinPack : DLCPackageType.Unknown;

            IDLCPackage skinPackage = hasSkins ? GetDLCSkinPackage(name, identifier, pckFile, fileReader) : null;

            PckAsset texturePackInfo = pckFile.GetAssetsByType(PckAssetType.TexturePackInfoFile).FirstOrDefault();
            string dataPath = texturePackInfo is not null ? texturePackInfo.GetProperty("DATAPATH") : string.Empty;

            DirectoryInfo dataDirectoryInfo = fileInfo.Directory.EnumerateDirectories().Where(dirInfo => dirInfo.Name == "Data").FirstOrDefault();
            bool hasDataFolder = dataDirectoryInfo is not null;

            if (hasDataFolder && !string.IsNullOrWhiteSpace(dataPath))
            {
                PckFile infoPck = texturePackInfo.GetData(fileReader);
                FileInfo texturePackFileInfo = dataDirectoryInfo.EnumerateFiles().Where(fileInfo => fileInfo.Name == dataPath).FirstOrDefault();
                if (IsValidPckFile(texturePackFileInfo))
                {
                    using Stream texturePackFileStream = texturePackFileInfo.OpenRead();
                    PckFile texturePackPck = fileReader.FromStream(texturePackFileStream);
                    IDLCPackage texturePackage = GetTexturePackageFromPckFile(infoPck, texturePackPck);
                    dlcPackageType = DLCPackageType.TexturePack;
                }

                bool hasSavefile = dataDirectoryInfo.EnumerateFiles("*.mcs").Any();
                PckAsset gameRuleAsset = null;
                if (Platform == ConsolePlatform.Unknown)
                    throw new Exception($"{nameof(Platform)} is Unknown.");
                bool hasGameRuleFile = pckFile.Contains("GameRules.grf", PckAssetType.GameRulesFile) && pckFile.TryGetAsset("GameRules.grf", PckAssetType.GameRulesFile, out gameRuleAsset);

                if (hasSavefile && hasGameRuleFile)
                {
                    GameRuleFile gameRuleFile = gameRuleAsset.GetData(new GameRuleFileReader(GetPlatformCompressionType()));
                    dlcPackageType = DLCPackageType.MashUpPack;
                }
            }
            Debug.WriteLine(dlcPackageType);
            return InvalidDLCPackage.Instance;
        }

        private GameRuleFile.CompressionType GetPlatformCompressionType()
        {
            switch (Platform)
            {
                case ConsolePlatform.Xbox360:
                    return GameRuleFile.CompressionType.XMem;

                case ConsolePlatform.PS3:
                    return GameRuleFile.CompressionType.Deflate;

                case ConsolePlatform.XboxOne:
                case ConsolePlatform.PS4:
                case ConsolePlatform.PSVita:
                case ConsolePlatform.WiiU:
                case ConsolePlatform.Switch:
                    return GameRuleFile.CompressionType.Zlib;

                case ConsolePlatform.Unknown:
                default:
                    return GameRuleFile.CompressionType.Unknown;
            }
        }

        private IDLCPackage GetTexturePackageFromPckFile(PckFile infoPck, PckFile dataPck)
        {
            if (!infoPck.TryGetAsset("comparison.png", PckAssetType.TextureFile, out PckAsset comparisonAsset))
            {
                Trace.TraceError($"Could not find 'comparison.png'.");
                return InvalidDLCPackage.Instance;
            }
            if (!infoPck.TryGetAsset("icon.png", PckAssetType.TextureFile, out PckAsset iconnAsset))
            {
                Trace.TraceError($"Could not find 'icon.png'.");
                return InvalidDLCPackage.Instance;
            }

            Image comparisonImg = comparisonAsset.GetTexture();
            Image iconImg = iconnAsset.GetTexture();
            DLCTexturePackage.MetaData metaData = new DLCTexturePackage.MetaData(comparisonImg, iconImg);

            bool hasTerrainAtlas  = TryGetAtlasFromResourceCategory(dataPck, ResourceCategory.BlockAtlas, out Atlas terrainAtlas);
            bool hasItemAtlas     = TryGetAtlasFromResourceCategory(dataPck, ResourceCategory.ItemAtlas, out Atlas itemAtlas);
            bool hasParticleAtlas = TryGetAtlasFromResourceCategory(dataPck, ResourceCategory.ParticleAtlas, out Atlas particleAtlas);
            bool hasPaintingAtlas = TryGetAtlasFromResourceCategory(dataPck, ResourceCategory.PaintingAtlas, out Atlas paintingAtlas);

            string itemAnimationResourcePath = ResourceLocation.GetPathFromCategory(ResourceCategory.ItemAnimation);
            if (dataPck != null &&
                dataPck.TryGetAsset(itemAnimationResourcePath + "/compass.png", PckAssetType.TextureFile, out PckAsset compassAsset) &&
                dataPck.TryGetAsset(itemAnimationResourcePath + "/clock.png", PckAssetType.TextureFile, out PckAsset clockAsset))
            {
                
            }
            return null;
        }

        private bool TryGetAtlasFromResourceCategory(PckFile pck, ResourceCategory resourceCategory, out Atlas atlas)
        {
            ResourceLocation resourceLocation = ResourceLocation.GetFromCategory(resourceCategory);
            if (!pck.TryGetAsset(resourceLocation.ToString(), PckAssetType.TextureFile, out PckAsset asset))
            {
                Trace.TraceWarning($"Could not find '{resourceLocation}'.");
                atlas = null;
                return false;
            }
            atlas = asset.GetDeserializedData(new AtlasDeserializer(resourceLocation));
            return true;
        }

        private IDLCPackage GetDLCSkinPackage(string name, int identifier, PckFile pck, PckFileReader fileReader, IDLCPackage parentPackage = null)
        {
            Skin.Skin GetSkinWithCape(PckAsset skinAsset)
            {
                Skin.Skin skin = skinAsset.GetSkin();
                if (skinAsset.TryGetProperty("CAPEPATH", out string capeAssetPath) && pck.TryGetAsset(capeAssetPath, PckAssetType.CapeFile, out PckAsset capeAsset))
                    skin.CapeTexture = capeAsset.GetTexture();
                return skin;
            }

            IEnumerable<Skin.Skin> skins = pck.GetAssetsByType(PckAssetType.SkinFile).Select(GetSkinWithCape);

            skins = skins.Concat(pck.GetAssetsByType(PckAssetType.SkinDataFile)
                .Select(asset => asset.GetData(fileReader))
                .SelectMany(pck => pck.GetAssetsByType(PckAssetType.SkinFile))
                .Select(GetSkinWithCape)
                );

            return new DLCSkinPackage(name, identifier, skins, null, parentPackage);
        }
    }
}