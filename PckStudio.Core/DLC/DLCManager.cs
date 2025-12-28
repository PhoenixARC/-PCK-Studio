using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using OMI;
using OMI.Formats.Color;
using OMI.Formats.GameRule;
using OMI.Formats.Languages;
using OMI.Formats.Model;
using OMI.Formats.Pck;
using OMI.Workers.Color;
using OMI.Workers.GameRule;
using OMI.Workers.Language;
using OMI.Workers.Material;
using OMI.Workers.Model;
using OMI.Workers.Pck;
using PckStudio.Core.App;
using PckStudio.Core.Colors;
using PckStudio.Core.Deserializer;
using PckStudio.Core.Extensions;
using PckStudio.Core.FileFormats;
using PckStudio.Core.Interfaces;
using PckStudio.Core.IO.PckAudio;
using PckStudio.Core.Model;
using PckStudio.Interfaces;

namespace PckStudio.Core.DLC
{
    public class DLCManager
    {
        internal const string DEFAULT_TEXTURE_PACK_FILENAME = "TexturePack.pck";
        internal const string DEFAULT_MINIGAME_PACK_FILENAME = "WorldPack.pck";
        internal const string DATA_DIRECTORY_NAME = "Data";
        internal const string PACKAGE_DISPLAYNAME_ID = "IDS_DISPLAY_NAME";
        private const string BASE_SAVE_NAME_GRF_PARAMETER_KEY = "baseSaveName";
        private const string GRF_MAP_OPTIONS_NAME = "MapOptions";
        private const string TEXTURE_PACK_DATA_PATH_KEY = "DATAPATH";
        private const string CAPE_PATH_KEY = "CAPEPATH";

        public ByteOrder ByteOrder => _byteOrder;

        public ConsolePlatform Platform => _platform;

        public DLCPackageContentSerilasationType ContentSerilasationType => _packageContentSerilasationType;

        /// <summary>
        /// See <see cref="AvailableLanguages"/> for details.
        /// </summary>
        public string PreferredLanguage { get; private set; }

        private readonly DLCPackageRegistry _packageRegistry = new DLCPackageRegistry();
        private readonly Random _rng = new Random();
        private ByteOrder _byteOrder;
        private ConsolePlatform _platform;
        private readonly AppLanguage _preferredAppLanguage;
        private readonly DLCPackageContentSerilasationType _packageContentSerilasationType;
        private PckFileCompiler _pckFileCompiler;


        /// <param name="byteOrder"></param>
        /// <param name="platform"></param>
        /// <param name="preferredLanguage">See <see cref="AvailableLanguages"/> for details.</param>
        public DLCManager(ConsolePlatform platform, AppLanguage preferredLanguage, DLCPackageContentSerilasationType packageContentSerilasationType)
        {
            _platform = platform;
            _preferredAppLanguage = preferredLanguage;
            _packageContentSerilasationType = packageContentSerilasationType;
            _byteOrder = GetByteOrderForPlatform(Platform);
            SetPreferredLanguage(preferredLanguage);
        }

        private static ByteOrder GetByteOrderForPlatform(ConsolePlatform platform)
        {
            return platform switch
            {
                ConsolePlatform.Switch => ByteOrder.LittleEndian,
                ConsolePlatform.PS_4 => ByteOrder.LittleEndian,
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
                DLCPackageType.Invalid => DLCPackage.Invalid,
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
                return DLCPackage.Invalid;

            using Stream stream = fileInfo.OpenRead();

            PckFileReader fileReader = new PckFileReader(ByteOrder);

            PckFile pckFile = fileReader.FromStream(stream);

            if (!pckFile.TryGetAsset("0", PckAssetType.InfoFile, out PckAsset zeroAsset))
            {
                Trace.TraceError("Could not find asset named:'0'.");
                return new RawAssetDLCPackage(fileInfo.Name, pckFile, ByteOrder);
            }

            int identifier = zeroAsset.HasProperty("PACKID") ? zeroAsset.GetProperty("PACKID", int.Parse) : -1;
            if (!identifier.IsWithinRangeOf(1, GameConstants.MAX_PACK_ID))
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
            return new RawAssetDLCPackage(fileInfo.Name, identifier, pckFile, ByteOrder);
        }

        public bool CloseDLCPackage(int identifier) => _packageRegistry.UnregisterPackage(identifier);

        internal LOCFile GetLocalisation(int identifier)
        {
            return _packageRegistry.ContainsPackage(identifier) ? _packageRegistry.GetLocalisation(identifier) : new LOCFile();
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

            DirectoryInfo dataDirectoryInfo = fileInfo.Directory.EnumerateDirectories().Where(dirInfo => dirInfo.Name == DATA_DIRECTORY_NAME).FirstOrDefault();

            if (dataDirectoryInfo is null)
            {
                Trace.TraceInformation("No data directory found.");
                return skinPackage ?? new RawAssetDLCPackage(name, pckFile, ByteOrder);
            }

            bool hasTexturePack = TryGetTexturePack(name, description, identifier, dataDirectoryInfo, pckFile, fileReader, out IDLCPackage texturePackage);
            if (!hasTexturePack)
            {
                Trace.TraceWarning("Couldn't parse texturepack.");
                return skinPackage ?? new RawAssetDLCPackage(name, pckFile, ByteOrder);
            }

            PckAudioFile pckAudioFile = pckFile.GetAssetsByType(PckAssetType.AudioFile).FirstOrDefault()?.GetData(new PckAudioFileReader(ByteOrder));
            IDictionary<string, byte[]> audios = default;
            if (pckAudioFile != null)
            {
                var songs = pckAudioFile.Categories.SelectMany(audioCategory => audioCategory.SongNames).ToList();
                audios = dataDirectoryInfo.EnumerateFiles("*.binka")
                    .Where(audioFile => songs.Contains(Path.GetFileNameWithoutExtension(audioFile.Name)))
                    .ToDictionary(audioFile => audioFile.Name, audioFile => File.ReadAllBytes(audioFile.FullName));
            }

            GameRuleFile.CompressionType compressionType = GetPlatformCompressionType();
            var reader = new GameRuleFileReader(compressionType);
            IEnumerable<GameRuleFile> gameRules = pckFile.GetAssetsByType(PckAssetType.GameRulesFile)
                .Concat(pckFile.GetAssetsByType(PckAssetType.GameRulesHeader))
                .Select(asset => asset.GetData(reader));

            Dictionary<string, SaveData> mapData = GetMapData(gameRules, dataDirectoryInfo);
            if (mapData.Count == 0)
                return texturePackage ?? skinPackage ?? new RawAssetDLCPackage(name, pckFile, ByteOrder);

            if (mapData.Count == 1)
                return new DLCMashUpPackage(name, description, identifier, null, null, pckAudioFile, audios, parentPackage: null, skinPackage: skinPackage, texturePackage: texturePackage);
            
            return new RawAssetDLCPackage(name, pckFile, ByteOrder);
        }

        private Dictionary<string, SaveData> GetMapData(IEnumerable<GameRuleFile> gameRuleFiles, DirectoryInfo dataDirectory)
        {
            IEnumerable<string> values = gameRuleFiles
                .SelectMany(grf => grf.Root.GetRules().Where(rule => rule.Name == GRF_MAP_OPTIONS_NAME && rule.ContainsParameter(BASE_SAVE_NAME_GRF_PARAMETER_KEY)))
                .Select(rule => rule.GetParameterValue(BASE_SAVE_NAME_GRF_PARAMETER_KEY));

            return dataDirectory.EnumerateFiles("*.mcs")
                .Where(file => values.Contains(file.Name))
                .ToDictionary(worldFile => worldFile.Name, worldFile => MapReader.OpenSaveData(worldFile.OpenRead()));
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
            string dataPath = texturePackInfo.GetProperty(TEXTURE_PACK_DATA_PATH_KEY);

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

            return texturePackage is not null;
        }

        private IDLCPackage GetTexturePackageFromPckFile(string name, string description, int identifier, PckFile infoPck, PckFile dataPck, DLCTexturePackage.TextureResolution resolution)
        {
            if (infoPck is null || dataPck is null)
                return null;

            if (!infoPck.TryGetAsset("comparison.png", PckAssetType.TextureFile, out PckAsset comparisonAsset))
                Trace.TraceWarning($"Could not find 'comparison.png'.");
            
            if (!infoPck.TryGetAsset("icon.png", PckAssetType.TextureFile, out PckAsset iconnAsset))
                Trace.TraceWarning($"Could not find 'icon.png'.");

            DLCTexturePackage.MetaData metaData = new DLCTexturePackage.MetaData(comparisonAsset?.GetTexture(), iconnAsset?.GetTexture());

            TryGetAtlasFromResourceCategory(dataPck, AtlasResource.AtlasType.BlockAtlas, out Atlas terrainAtlas);
            TryGetAtlasFromResourceCategory(dataPck, AtlasResource.AtlasType.ItemAtlas, out Atlas itemAtlas);
            TryGetAtlasFromResourceCategory(dataPck, AtlasResource.AtlasType.ParticleAtlas, out Atlas particleAtlas);
            TryGetAtlasFromResourceCategory(dataPck, AtlasResource.AtlasType.PaintingAtlas, out Atlas paintingAtlas);
            TryGetAtlasFromResourceCategory(dataPck, AtlasResource.AtlasType.MoonPhaseAtlas, out Atlas moonPhaseAtlas);
            TryGetAtlasFromResourceCategory(dataPck, AtlasResource.AtlasType.MapIconAtlas, out Atlas mapIconsAtlas);
            TryGetAtlasFromResourceCategory(dataPck, AtlasResource.AtlasType.AdditionalMapIconsAtlas, out Atlas additionalMapIconsAtlas);

            string itemAnimationAssetPath = ResourceLocation.GetPathFromCategory(ResourceCategory.ItemAnimation);
            string blockAnimationAssetPath = ResourceLocation.GetPathFromCategory(ResourceCategory.BlockAnimation);

            IPckAssetDeserializer<Animation> deserializer = AnimationDeserializer.DefaultDeserializer;

            IDictionary<string, Animation> itemAnimations = dataPck.GetDirectoryContent(itemAnimationAssetPath, PckAssetType.TextureFile)
                .Where(asset => !asset.IsMipmappedFile())
                .ToDictionary(asset => Path.GetFileNameWithoutExtension(asset.Filename), deserializer.Deserialize);

            IDictionary<string, Animation> blockAnimations = dataPck.GetDirectoryContent(blockAnimationAssetPath, PckAssetType.TextureFile)
                .Where(asset => !asset.IsMipmappedFile())
                .ToDictionary(asset => Path.GetFileNameWithoutExtension(asset.Filename), deserializer.Deserialize);

            if (!itemAnimations.ContainsKey("compass"))
                Trace.TraceError("No compass animation found!");

            if (!itemAnimations.ContainsKey("clock"))
                Trace.TraceError("No clock animation found!");

            ITryGet<string, Image> tryGetTexture = TryGet<string, Image>.FromDelegate((string path, out Image image) =>
            {
                bool success = dataPck.TryGetAsset(path, PckAssetType.TextureFile, out PckAsset asset);
                image = asset?.GetTexture();
                return success;
            });

            ImageDeserializer defaultDeserializer = ImageDeserializer.DefaultDeserializer;
            IEnumerable<Image> blockEntityBreakingFrames = dataPck.GetDirectoryContent("res/textures/", PckAssetType.TextureFile)
                .Select(defaultDeserializer.Deserialize);
            Animation blockEntityBreakAnimation = new Animation(blockEntityBreakingFrames);

            ColorContainer colorContainer = dataPck.GetAssetsByType(PckAssetType.ColourTableFile).FirstOrDefault()?.GetData(new COLFileReader()) ?? new ColorContainer();

            IDictionary<string, Image> environmentTextures = dataPck.GetDirectoryContent("res/environment/", PckAssetType.TextureFile)
                .ToDictionary(a => Path.GetFileNameWithoutExtension(a.Filename), defaultDeserializer.Deserialize);
            environmentTextures.TryGetValue("clouds", out Image clouds);
            environmentTextures.TryGetValue("rain", out Image rain);
            environmentTextures.TryGetValue("snow", out Image snow);

            DLCTexturePackage.EnvironmentData environmentData = new DLCTexturePackage.EnvironmentData(clouds, rain, snow);

            IDictionary<string, Image> terrainTextures = dataPck.GetDirectoryContent("res/terrain/", PckAssetType.TextureFile)
                .ToDictionary(a => Path.GetFileNameWithoutExtension(a.Filename), defaultDeserializer.Deserialize);

            terrainTextures.TryGetValue("sun", out Image sun);
            terrainTextures.TryGetValue("moon", out Image moon);

            AbstractModelContainer customModels = null;
            if (dataPck.TryGetAsset("models.bin", PckAssetType.ModelsFile, out PckAsset modelsAsset))
            {
                ModelContainer models = modelsAsset.GetData(new ModelFileReader());
                customModels = AbstractModelContainer.FromModelContainer(models, null);
            }

            IDictionary<string, string> materials = dataPck.GetAssetsByType(PckAssetType.MaterialFile).FirstOrDefault()?.GetData(new MaterialFileReader())
                .ToDictionary(mat => mat.Name, mat => mat.Type);

            return new DLCTexturePackage(name, description, identifier, metaData, resolution,
                terrainAtlas, itemAtlas, particleAtlas, paintingAtlas, moonPhaseAtlas, mapIconsAtlas, additionalMapIconsAtlas,
                ArmorSetDescription.Leather.GetArmorSet(tryGetTexture),
                ArmorSetDescription.Chain.GetArmorSet(tryGetTexture),
                ArmorSetDescription.Iron.GetArmorSet(tryGetTexture),
                ArmorSetDescription.Gold.GetArmorSet(tryGetTexture),
                ArmorSetDescription.Diamond.GetArmorSet(tryGetTexture),
                ArmorSetDescription.Turtle.GetArmorSet(tryGetTexture),
                environmentData,
                AbstractColorContainer.FromColorContainer(colorContainer),
                null,
                null,
                customModels: customModels,
                materials: materials,
                blockEntityBreakAnimation: blockEntityBreakAnimation,
                itemAnimations: itemAnimations,
                blockAnimations: blockAnimations,
                sun: sun, moon: moon,
                parentPackage: null);
        }

        private bool TryGetAtlasFromResourceCategory(PckFile pck, AtlasResource.AtlasType atlasType, out Atlas atlas)
        {
            ResourceLocation resourceLocation = ResourceLocation.GetFromCategory(AtlasResource.GetId(atlasType));
            if (!pck.TryGetAsset(resourceLocation.FullPath, PckAssetType.TextureFile, out PckAsset asset))
            {
                Trace.TraceWarning($"Could not find '{resourceLocation.FullPath}'.");
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
                if (skinAsset.TryGetProperty(CAPE_PATH_KEY, out string capeAssetPath) && pck.TryGetAsset(capeAssetPath, PckAssetType.CapeFile, out PckAsset capeAsset))
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

        public static GameRuleFile.CompressionType GetCompressionTypeForPlatform(ConsolePlatform platform)
        {
            switch (platform)
            {
                case ConsolePlatform.Xbox_360:
                    return GameRuleFile.CompressionType.XMem;

                case ConsolePlatform.PS_3:
                    return GameRuleFile.CompressionType.Deflate;

                case ConsolePlatform.Xbox_One:
                case ConsolePlatform.PS_4:
                case ConsolePlatform.PS_Vita:
                case ConsolePlatform.Wii_U:
                case ConsolePlatform.Switch:
                    return GameRuleFile.CompressionType.Zlib;

                case ConsolePlatform.Unknown:
                default:
                    throw new ArgumentException("Platform was not set");
            }
        }

        internal GameRuleFile.CompressionType GetPlatformCompressionType() => GetCompressionTypeForPlatform(Platform);

        internal static string GetPreferredLanguage(AppLanguage appLanguage)
        {
            return appLanguage switch
            {
                AppLanguage.System_Default => LOCFile.ValidLanguages.Contains(CultureInfo.CurrentUICulture.Name) ? CultureInfo.CurrentUICulture.Name : AvailableLanguages.English,
                AppLanguage.Czech_Czechia => AvailableLanguages.CzechCzechia,
                AppLanguage.Czechia => AvailableLanguages.Czechia,
                AppLanguage.Denmark_Danish => AvailableLanguages.DenmarkDanish,
                AppLanguage.German => AvailableLanguages.German,
                AppLanguage.Greece => AvailableLanguages.Greece,
                AppLanguage.English => AvailableLanguages.English,
                AppLanguage.English_UnitedKingdom => AvailableLanguages.EnglishUnitedKingdom,
                AppLanguage.Spanish_Spain => AvailableLanguages.SpanishSpain,
                AppLanguage.Spanish_Mexico => AvailableLanguages.SpanishMexico,
                AppLanguage.Finnish_Finland => AvailableLanguages.FinnishFinland,
                AppLanguage.French_France => AvailableLanguages.FrenchFrance,
                AppLanguage.Italian_Italy => AvailableLanguages.ItalianItaly,
                AppLanguage.Japanese_Japan => AvailableLanguages.JapaneseJapan,
                AppLanguage.Korean_South_Korea => AvailableLanguages.KoreanSouthKorea,
                AppLanguage.Norwegian_Bokmål_Norway => AvailableLanguages.NorwegianBokmålNorway,
                AppLanguage.Dutch_Netherlands => AvailableLanguages.DutchNetherlands,
                AppLanguage.Polish_Poland => AvailableLanguages.PolishPoland,
                AppLanguage.Portuguese_Brazil => AvailableLanguages.PortugueseBrazil,
                AppLanguage.Portuguese_Portugal => AvailableLanguages.PortuguesePortugal,
                AppLanguage.Russian_Russia => AvailableLanguages.RussianRussia,
                AppLanguage.Slovak_Slovakia => AvailableLanguages.SlovakSlovakia,
                AppLanguage.Swedish_Sweden => AvailableLanguages.SwedishSweden,
                AppLanguage.Turkish_Turkey => AvailableLanguages.TurkishTurkey,
                AppLanguage.Chinese_China => AvailableLanguages.ChineseChina,
                _ => AvailableLanguages.English,
            };
        }

        public void SetPreferredLanguage(AppLanguage lang) => PreferredLanguage = GetPreferredLanguage(lang);

        public void SetPlatform(ConsolePlatform platform)
        {
            _platform = platform;
            _byteOrder = GetByteOrderForPlatform(platform);
        }

        public DLCPackageContent CompilePackage(IDLCPackage package)
        {
            _pckFileCompiler = new PckFileCompiler(this);
            LOCFile localisation = GetLocalisation(package.Identifier);
            switch (package.GetDLCPackageType())
            {
                case DLCPackageType.Invalid:
                    break;
                case DLCPackageType.RawAssets: return _pckFileCompiler.CompileRawAssets(package);
                case DLCPackageType.SkinPack: return _pckFileCompiler.CompileSkinPackage(package, localisation);
                case DLCPackageType.TexturePack: return _pckFileCompiler.CompileTexturePackage(package, localisation);
                case DLCPackageType.MashUpPack: return _pckFileCompiler.CompileMashUpPackage(package, localisation);
                case DLCPackageType.MG01:
                    break;
                case DLCPackageType.MG02:
                    break;
                case DLCPackageType.MG03:
                    break;
            }
            return DLCPackageContent.Empty;
        }

        private enum ConsoleRegion
        {
            US,
            EU,
            JP
        }

        public string GetInstallPath()
        {
            switch (Platform)
            {
                case ConsolePlatform.Wii_U:
                    ConsoleRegion region = GetRegionFromLanguage();
                    string titleId = region switch
                    {
                        ConsoleRegion.US => "101d9d00",
                        ConsoleRegion.EU => "101d7500",
                        ConsoleRegion.JP => "101dbe00",
                        _ => throw new Exception()
                    };
                    return $"usr/title/0005000e/{titleId}/content/WiiU/DLC";
                default:
                    return "";
            }
        }

        private ConsoleRegion GetRegionFromLanguage()
        {
            switch (_preferredAppLanguage)
            {
                case AppLanguage.System_Default:
                case AppLanguage.Czech_Czechia:
                case AppLanguage.Czechia:
                case AppLanguage.Danish:
                case AppLanguage.Denmark_Danish:
                case AppLanguage.German_Austria:
                case AppLanguage.German:
                case AppLanguage.Greek_Greece:
                case AppLanguage.English_UnitedKingdom:
                case AppLanguage.English_Ireland:
                case AppLanguage.Spanish_Spain:
                case AppLanguage.Finnish_Finland:
                case AppLanguage.French_France:
                case AppLanguage.Dutch_Netherlands:
                case AppLanguage.Dutch_Belgium:
                case AppLanguage.Polish_Poland:
                case AppLanguage.Norwegian_Norway:
                case AppLanguage.Norwegian_Bokmål_Norway:
                case AppLanguage.Italian_Italy:
                case AppLanguage.Slovak_Slovakia:
                case AppLanguage.Swedish_Sweden:
                case AppLanguage.Turkish_Turkey:
                case AppLanguage.Russian_Russia:
                case AppLanguage.Greece:
                    return ConsoleRegion.EU;
                case AppLanguage.English_Australia:
                case AppLanguage.English_Canada:
                case AppLanguage.English:
                case AppLanguage.English_NewZealand:
                case AppLanguage.Spanish_Mexico:
                case AppLanguage.French_Canada:
                case AppLanguage.English_USA:
                    return ConsoleRegion.US;
                case AppLanguage.Korean_South_Korea:
                case AppLanguage.Chinese_China:
                case AppLanguage.Chinese_HongKong:
                case AppLanguage.Chinese_Singapore:
                case AppLanguage.Chinese_Taiwan:
                case AppLanguage.Japanese_Japan:
                    return ConsoleRegion.JP;
                case AppLanguage.Portuguese_Brazil:
                case AppLanguage.Portuguese_Portugal:
                case AppLanguage.Latin:
                default:
                    throw new NotImplementedException();
            }
        }
    }
}