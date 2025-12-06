using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Documents;
using OMI;
using OMI.Formats.GameRule;
using OMI.Formats.Languages;
using OMI.Formats.Pck;
using OMI.Workers.GameRule;
using OMI.Workers.Language;
using OMI.Workers.Pck;
using PckStudio.Core.App;
using PckStudio.Core.Deserializer;
using PckStudio.Core.Extensions;
using PckStudio.Core.Interfaces;
using PckStudio.Core.IO.PckAudio;
using PckStudio.Core.Properties;
using PckStudio.Interfaces;

namespace PckStudio.Core.DLC
{
    public sealed class DLCManager
    {
        public static DLCManager Default { get; } = new DLCManager(default, AppLanguage.SystemDefault);
       
        internal const string DEFAULT_TEXTURE_PACK_FILENAME = "TexturePack.pck";
        internal const string DEFAULT_MINIGAME_PACK_FILENAME = "WorldPack.pck";
        internal const string DATA_DIRECTORY_NAME = "Data";
        internal const string PACKAGE_DISPLAYNAME_ID = "IDS_DISPLAY_NAME";

        public ByteOrder ByteOrder => _byteOrder;

        public ConsolePlatform Platform => _platform;

        /// <summary>
        /// See <see cref="AvailableLanguages"/> for details.
        /// </summary>
        public string PreferredLanguage { get; private set; }

        private readonly DLCPackageRegistry _packageRegistry = new DLCPackageRegistry();
        private readonly Random _rng = new Random();
        private ByteOrder _byteOrder;
        private ConsolePlatform _platform;


        /// <param name="byteOrder"></param>
        /// <param name="platform"></param>
        /// <param name="preferredLanguage">See <see cref="AvailableLanguages"/> for details.</param>
        public DLCManager(ConsolePlatform platform, AppLanguage preferredLanguage)
        {
            _platform = platform;
            _byteOrder = GetByteOrderForPlatform(Platform);
            SetPreferredLanguage(preferredLanguage);
        }

        private static ByteOrder GetByteOrderForPlatform(ConsolePlatform platform)
        {
            return platform switch
            {
                ConsolePlatform.Switch => ByteOrder.LittleEndian,
                ConsolePlatform.PS4 => ByteOrder.LittleEndian,
                _ => ByteOrder.BigEndian
            };
        }

        public IDLCPackage CreateNewPackage(string name, DLCPackageType packageType)
        {
            int identifier = _rng.Next(8000, GameConstants.MAX_PACK_ID);
            IDLCPackage package = packageType switch
            {
                DLCPackageType.SkinPack => DLCSkinPackage.CreateEmpty(name, identifier),
                DLCPackageType.TexturePack => DLCTexturePackage.CreateDefaultPackage(name, "", identifier),
                DLCPackageType.MashUpPack => new DLCMashUpPackage(name, "", identifier),
                //! TODO: implemnt minigame dlc packages -null
                DLCPackageType.MG01 => new DLCBattlePackage(name, identifier),
                DLCPackageType.MG02 => new DLCMiniGamePackage(name, identifier, packageType, MiniGameId.Tumble),
                DLCPackageType.MG03 => new DLCMiniGamePackage(name, identifier, packageType, MiniGameId.Glide),
                DLCPackageType.Invalid => InvalidDLCPackage.Instance,
                _ => throw new ArgumentException("Unable to create DLC Package of 'Unknown' type."),
            };

            LOCFile localisation = new LOCFile();
            localisation.AddLanguage(PreferredLanguage);
            localisation.AddLocKey(PACKAGE_DISPLAYNAME_ID, name);
            _packageRegistry.RegisterPackage(identifier, package, localisation);
            
            return package;
        }

        public IDLCPackage OpenDLCPackage(string filepath) => OpenDLCPackage(new FileInfo(filepath));

        public IDLCPackage OpenDLCPackage(FileInfo fileInfo)
        {
            if (Platform == ConsolePlatform.Unknown)
                throw new Exception($"{nameof(Platform)} is Unknown.");

            if (!IsValidPckFile(fileInfo))
                return InvalidDLCPackage.Instance;

            using Stream stream = fileInfo.OpenRead();

            PckFileReader fileReader = new PckFileReader(ByteOrder);

            PckFile pckFile = fileReader.FromStream(stream);

            if (!pckFile.TryGetAsset("0", PckAssetType.InfoFile, out PckAsset zeroAsset))
            {
                Trace.TraceError("Could not find asset named:'0'.");
                return new RawAssetDLCPackage(fileInfo.Name, pckFile, ByteOrder);
            }

            int identifier = zeroAsset.HasProperty("PACKID") ? zeroAsset.GetProperty("PACKID", int.Parse) : -1;
            if (identifier <= 0 || identifier > GameConstants.MAX_PACK_ID)
            {
                Trace.TraceError($"{nameof(identifier)}({identifier}) was out of range!");
                return new RawAssetDLCPackage(fileInfo.Name, pckFile, ByteOrder);
            }

            if (_packageRegistry.ContainsPackage(identifier))
                return _packageRegistry[identifier];

            LOCFile localisation = pckFile.GetAssetsByType(PckAssetType.LocalisationFile).FirstOrDefault()?.GetData(new LOCFileReader());
            if (localisation is null)
            {
                Trace.TraceError("No localisation asset found.");
                return new RawAssetDLCPackage(fileInfo.Name, pckFile, ByteOrder);
            }

            IDLCPackage package = LoadDLCPackage(fileInfo, identifier, pckFile, localisation, fileReader);
            if (package.GetDLCPackageType() != DLCPackageType.Invalid)
            {
                _packageRegistry.RegisterPackage(identifier, package, localisation);
            }
            return package;
        }

        internal LOCFile GetLocalisation(int identifier)
        {
            return _packageRegistry.ContainsPackage(identifier) ? _packageRegistry.GetLocalisation(identifier) : default;
        }

        private bool IsValidPckFile(FileInfo fileInfo)
        {
            if (fileInfo is null)
                throw new ArgumentNullException(nameof(fileInfo));

            if (!fileInfo.Exists)
                throw new FileNotFoundException(fileInfo.FullName);

            if (fileInfo.Extension != ".pck")
                throw new FileFormatException("File does not end with '.pck'.");
            return true;
        }

        private IDLCPackage LoadDLCPackage(FileInfo fileInfo, int identifier, PckFile pckFile, LOCFile localisation, PckFileReader fileReader)
        {
            bool hasLanguage = localisation?.Languages?.Contains(PreferredLanguage) ?? default;

            string name = hasLanguage && (localisation?.HasLocEntry(PACKAGE_DISPLAYNAME_ID) ?? default)
                ? localisation.GetLocEntry(PACKAGE_DISPLAYNAME_ID, PreferredLanguage) : fileInfo.Name;

            string description = hasLanguage && (localisation?.HasLocEntry(DLCTexturePackage.TEXTUREPACK_DESCRIPTION_ID) ?? default)
                ? localisation.GetLocEntry(DLCTexturePackage.TEXTUREPACK_DESCRIPTION_ID, PreferredLanguage) : string.Empty;

            bool couldBeTexturePack = fileInfo.Name == DEFAULT_TEXTURE_PACK_FILENAME;
            bool couldBeMiniGamePack = fileInfo.Name == DEFAULT_MINIGAME_PACK_FILENAME;

            bool hasSkins = TryGetDLCSkinPackage(name, identifier, pckFile, fileReader, out IDLCPackage skinPackage);
            DLCPackageType dlcPackageType = hasSkins ? DLCPackageType.SkinPack : DLCPackageType.RawAssets;

            DirectoryInfo dataDirectoryInfo = fileInfo.Directory.EnumerateDirectories().Where(dirInfo => dirInfo.Name == DATA_DIRECTORY_NAME).FirstOrDefault();

            if (dataDirectoryInfo is null)
                return hasSkins ? skinPackage : InvalidDLCPackage.Instance;

            bool hasTexturePack = TryGetTexturePack(name, description, identifier, dataDirectoryInfo, pckFile, fileReader, out IDLCPackage texturePackage);
            if (hasTexturePack)
            {
                    dlcPackageType = DLCPackageType.TexturePack;
                }

            Dictionary<string, IDictionary<string, byte[]>> mapData = GetMapData(pckFile, dataDirectoryInfo);

            if (mapData.Count == 1)
            {
                dlcPackageType = DLCPackageType.MashUpPack;
                }

            Debug.WriteLine(dlcPackageType);
            return new RawAssetDLCPackage(name, pckFile, ByteOrder);
        }

        private Dictionary<string, IDictionary<string, byte[]>> GetMapData(PckFile pck, DirectoryInfo dataDirectory)
            {
            GameRuleFile.CompressionType compressionType = GetPlatformCompressionType();
            var reader = new GameRuleFileReader(compressionType);
            IEnumerable<string> values = pck.GetAssetsByType(PckAssetType.GameRulesFile)
                .Concat(pck.GetAssetsByType(PckAssetType.GameRulesHeader))
                .Select(asset => asset.GetData(reader))
                .SelectMany(grf => grf.Root.GetRules().Where(rule => rule.Name == "MapOptions" && rule.ContainsParameter("baseSaveName")))
                .Select(rule => rule.GetRule("MapOptions").GetParameterValue("baseSaveName"));

            Dictionary<string, IDictionary<string, byte[]>> saves = new Dictionary<string, IDictionary<string, byte[]>>();
            foreach (FileInfo worldFile in dataDirectory.EnumerateFiles("*.mcs").Where(file => values.Contains(file.Name)))
            {
                IDictionary<string, byte[]> save = MapReader.OpenSave(worldFile.OpenRead());
                saves.Add(worldFile.Name, save);
            }

            return saves;
        }

        private bool TryGetTexturePack(string name, string description, int identifier, DirectoryInfo dataDirectoryInfo, PckFile pckFile, PckFileReader pckFormatReader, out IDLCPackage texturePackage)
            {
            if (dataDirectoryInfo is null)
            {
                texturePackage = default;
                return false;
            }
            PckAsset texturePackInfo = pckFile.GetAssetsByType(PckAssetType.TexturePackInfoFile).FirstOrDefault();
            if (texturePackInfo is null)
            {
                texturePackage = default;
                return false;
            }

            DLCTexturePackage.TextureResolution resolution = DLCTexturePackage.GetTextureResolutionFromString(texturePackInfo.Filename);
            string dataPath = texturePackInfo.GetProperty("DATAPATH");

            if (string.IsNullOrWhiteSpace(dataPath))
            {
                texturePackage = default;
                return false;
        }

            PckFile infoPck = texturePackInfo.GetData(pckFormatReader);
            FileInfo texturePackFileInfo = dataDirectoryInfo.EnumerateFiles().Where(fileInfo => fileInfo.Name == dataPath).FirstOrDefault();

            if (!IsValidPckFile(texturePackFileInfo))
        {
                texturePackage = null;
                return false;
            }

            using Stream texturePackFileStream = texturePackFileInfo.OpenRead();
            PckFile texturePackPck = pckFormatReader.FromStream(texturePackFileStream);
            texturePackage = GetTexturePackageFromPckFile(name, description, identifier, infoPck, texturePackPck, resolution);

            IEnumerable<FileInfo> audioFiles = dataDirectoryInfo.EnumerateFiles("*.binka");
            IDictionary<string, byte[]> audios = new Dictionary<string, byte[]>();
            foreach (FileInfo audioFile in audioFiles)
            {
                byte[] data = File.ReadAllBytes(audioFile.FullName);
                audios.Add(audioFile.Name, data);
            }

            return texturePackage is not null;
            }

        private IDLCPackage GetTexturePackageFromPckFile(string name, string description, int identifier, PckFile infoPck, PckFile dataPck, DLCTexturePackage.TextureResolution resolution)
        {
            if (infoPck is null || dataPck is null)
                return null;

            if (!infoPck.TryGetAsset("comparison.png", PckAssetType.TextureFile, out PckAsset comparisonAsset))
            {
                Trace.TraceError($"Could not find 'comparison.png'.");
            }
            if (!infoPck.TryGetAsset("icon.png", PckAssetType.TextureFile, out PckAsset iconnAsset))
            {
                Trace.TraceError($"Could not find 'icon.png'.");
            }

            Image comparisonImg = comparisonAsset?.GetTexture();
            Image iconImg = iconnAsset?.GetTexture() ?? Resources.unknown_pack;
            DLCTexturePackage.MetaData metaData = new DLCTexturePackage.MetaData(comparisonImg, iconImg);

            bool hasTerrainAtlas = TryGetAtlasFromResourceCategory(dataPck, AtlasResource.AtlasType.BlockAtlas, out Atlas terrainAtlas);
            bool hasItemAtlas = TryGetAtlasFromResourceCategory(dataPck, AtlasResource.AtlasType.ItemAtlas, out Atlas itemAtlas);
            bool hasParticleAtlas = TryGetAtlasFromResourceCategory(dataPck, AtlasResource.AtlasType.ParticleAtlas, out Atlas particleAtlas);
            bool hasPaintingAtlas = TryGetAtlasFromResourceCategory(dataPck, AtlasResource.AtlasType.PaintingAtlas, out Atlas paintingAtlas);

            string itemAnimationAssetPath = ResourceLocation.GetPathFromCategory(ResourceCategory.ItemAnimation);

            IPckAssetDeserializer<Animation> deserializer = AnimationDeserializer.DefaultDeserializer;
            Animation compassAnimation = dataPck.TryGetAsset(itemAnimationAssetPath + "/compass.png", PckAssetType.TextureFile, out PckAsset compassAsset) ? comparisonAsset.GetDeserializedData(deserializer) : Animation.CreateEmpty();
            Animation clockAnimation = dataPck.TryGetAsset(itemAnimationAssetPath + "/clock.png", PckAssetType.TextureFile, out PckAsset clockAsset) ? clockAsset.GetDeserializedData(deserializer) : Animation.CreateEmpty();

            if (compassAnimation.FrameCount == 0)
                Trace.TraceError("No compass animation found!");

            if (clockAnimation.FrameCount == 0)
                Trace.TraceError("No clock animation found!");

            ITryGet<string, Image> tryGet = TryGet<string, Image>.FromDelegate((string path, out Image image) =>
            {
                bool success = dataPck.TryGetAsset(path, PckAssetType.TextureFile, out PckAsset asset);
                image = asset?.GetTexture();
                return success;
            });
                
            Image[] blockEntityBreakingAnimation = new Image[10];
            for (int i = 0; i < blockEntityBreakingAnimation.Length; i++)
            {
                if (dataPck.TryGetAsset("", PckAssetType.TextureFile, out PckAsset asset))
                    blockEntityBreakingAnimation[i] = asset.GetTexture();
            }

            ArmorSet[] armorSets = new ArmorSet[6]
            {
                ArmorSetDescription.Leather.GetArmorSet(tryGet),
                ArmorSetDescription.Chain.GetArmorSet(tryGet),
                ArmorSetDescription.Iron.GetArmorSet(tryGet),
                ArmorSetDescription.Gold.GetArmorSet(tryGet),
                ArmorSetDescription.Diamond.GetArmorSet(tryGet),
                ArmorSetDescription.Turtle.GetArmorSet(tryGet)
            };
            return new DLCTexturePackage(name, description, identifier, metaData, resolution, terrainAtlas, itemAtlas, particleAtlas, paintingAtlas,
                armorSets, null, null, null, null, null, null, null, null);
        }

        private bool TryGetAtlasFromResourceCategory(PckFile pck, AtlasResource.AtlasType atlasType, out Atlas atlas)
        {
            ResourceLocation resourceLocation = ResourceLocation.GetFromCategory((ResourceCategory)((int)ResourceCategory.Atlas | (int)atlasType));
            if (!pck.TryGetAsset(resourceLocation.ToString(), PckAssetType.TextureFile, out PckAsset asset))
            {
                Trace.TraceWarning($"Could not find '{resourceLocation}'.");
                atlas = null;
                return false;
            }
            atlas = asset.GetDeserializedData(new AtlasDeserializer(resourceLocation));
            return true;
        }

        private bool TryGetDLCSkinPackage(string name, int identifier, PckFile pck, PckFileReader fileReader, out IDLCPackage skinPackage, IDLCPackage parentPackage = null)
        {
            if (!(pck.Contains(PckAssetType.SkinFile) || pck.Contains(PckAssetType.SkinDataFile)))
            {
                skinPackage = default;
                return false;
            }

            IDictionary<int, Image> capes = pck.GetAssetsByType(PckAssetType.CapeFile)
                .Where(asset => asset.GetId() != -1)
                .ToDictionary(PckAssetExtensions.GetId, PckAssetExtensions.GetTexture);

            Skin.Skin GetSkinWithCape(PckAsset skinAsset)
            {
                Skin.Skin skin = skinAsset.GetSkin();
                if (skinAsset.TryGetProperty("CAPEPATH", out string capeAssetPath) && pck.TryGetAsset(capeAssetPath, PckAssetType.CapeFile, out PckAsset capeAsset))
                    skin.CapeId = capeAsset.GetId();
                return skin;
            }

            IEnumerable<Skin.Skin> skins = pck.GetAssetsByType(PckAssetType.SkinFile).Select(GetSkinWithCape);

            skins = skins.Concat(pck.GetAssetsByType(PckAssetType.SkinDataFile)
                .Select(asset => asset.GetData(fileReader))
                .SelectMany(pck => pck.GetAssetsByType(PckAssetType.SkinFile))
                .Select(GetSkinWithCape)
                );

            skinPackage = new DLCSkinPackage(name, identifier, skins, capes, parentPackage);
            return true;
        }
        }

        private static string GetPreferredLanguage(AppLanguage appLanguage)
        {
            return appLanguage switch
            {
                AppLanguage.SystemDefault => LOCFile.ValidLanguages.Contains(CultureInfo.CurrentUICulture.Name) ? CultureInfo.CurrentUICulture.Name : AvailableLanguages.English,
                AppLanguage.CzechCzechia => AvailableLanguages.CzechCzechia,
                AppLanguage.Czechia => AvailableLanguages.Czechia,
                AppLanguage.Danish => AvailableLanguages.Danish,
                AppLanguage.DenmarkDanish => AvailableLanguages.DenmarkDanish,
                AppLanguage.GermanAustria => AvailableLanguages.GermanAustria,
                AppLanguage.German => AvailableLanguages.German,
                AppLanguage.GreekGreece => AvailableLanguages.GreekGreece,
                AppLanguage.Greece => AvailableLanguages.Greece,
                AppLanguage.EnglishAustralia => AvailableLanguages.EnglishAustralia,
                AppLanguage.EnglishCanada => AvailableLanguages.EnglishCanada,
                AppLanguage.English => AvailableLanguages.English,
                AppLanguage.EnglishUnitedKingdom => AvailableLanguages.EnglishUnitedKingdom,
                AppLanguage.EnglishIreland => AvailableLanguages.EnglishIreland,
                AppLanguage.EnglishNewZealand => AvailableLanguages.EnglishNewZealand,
                AppLanguage.EnglishUnitedStatesOfAmerica => AvailableLanguages.EnglishUnitedStatesOfAmerica,
                AppLanguage.SpanishSpain => AvailableLanguages.SpanishSpain,
                AppLanguage.SpanishMexico => AvailableLanguages.SpanishMexico,
                AppLanguage.FinnishFinland => AvailableLanguages.FinnishFinland,
                AppLanguage.FrenchFrance => AvailableLanguages.FrenchFrance,
                AppLanguage.FrenchCanada => AvailableLanguages.FrenchCanada,
                AppLanguage.ItalianItaly => AvailableLanguages.ItalianItaly,
                AppLanguage.JapaneseJapan => AvailableLanguages.JapaneseJapan,
                AppLanguage.KoreanSouthKorea => AvailableLanguages.KoreanSouthKorea,
                AppLanguage.Latin => AvailableLanguages.Latin,
                AppLanguage.NorwegianNorway => AvailableLanguages.NorwegianNorway,
                AppLanguage.NorwegianBokmålNorway => AvailableLanguages.NorwegianBokmålNorway,
                AppLanguage.DutchNetherlands => AvailableLanguages.DutchNetherlands,
                AppLanguage.DutchBelgium => AvailableLanguages.DutchBelgium,
                AppLanguage.PolishPoland => AvailableLanguages.PolishPoland,
                AppLanguage.PortugueseBrazil => AvailableLanguages.PortugueseBrazil,
                AppLanguage.PortuguesePortugal => AvailableLanguages.PortuguesePortugal,
                AppLanguage.RussianRussia => AvailableLanguages.RussianRussia,
                AppLanguage.SlovakSlovakia => AvailableLanguages.SlovakSlovakia,
                AppLanguage.SwedishSweden => AvailableLanguages.SwedishSweden,
                AppLanguage.TurkishTurkey => AvailableLanguages.TurkishTurkey,
                AppLanguage.ChineseChina => AvailableLanguages.ChineseChina,
                AppLanguage.ChineseHongKong => AvailableLanguages.ChineseHongKong,
                AppLanguage.ChineseSingapore => AvailableLanguages.ChineseSingapore,
                AppLanguage.ChineseTaiwan => AvailableLanguages.ChineseTaiwan,
                _ => AvailableLanguages.English,
            };
        }

        public void SetPreferredLanguage(AppLanguage lang) => PreferredLanguage = GetPreferredLanguage(lang);

        public void SetPlatform(ConsolePlatform platform)
        {
            _platform = platform;
            _byteOrder = GetByteOrderForPlatform(platform);
        }
    }
}