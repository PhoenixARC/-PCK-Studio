using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OMI;
using OMI.Formats.GameRule;
using OMI.Formats.Languages;
using OMI.Formats.Pck;
using OMI.Workers.GameRule;
using OMI.Workers.Language;
using OMI.Workers.Pck;
using PckStudio.Core.Extensions;
using PckStudio.Core.Interfaces;
using PckStudio.Core.IO.PckAudio;

namespace PckStudio.Core.DLC
{
    internal sealed class PckFileCompiler
    {
        private ByteOrder _byteOrder;
        private GameRuleFile.CompressionType _compressionType;
        private GameRuleFile.CompressionLevel _compressionLevel;

        internal PckFileCompiler(ByteOrder byteOrder, GameRuleFile.CompressionType compressionType, GameRuleFile.CompressionLevel compressionLevel)
        {
            _byteOrder = byteOrder;
            _compressionType = compressionType;
            _compressionLevel = compressionLevel;
        }

        private PckFile CreateRootPckFile(int packId, int packVerison, LOCFile localisation)
        {
            PckFile mainPck = new PckFile();
            PckAsset meta = mainPck.CreateNewAsset("0", PckAssetType.InfoFile);
            meta.AddProperty("PACKID", packId);
            meta.AddProperty("PACKVERSION", packVerison);
            mainPck.CreateNewAsset("localisation.loc", PckAssetType.LocalisationFile, new LOCFileWriter(localisation, 2));
            return mainPck;
        }

        internal DLCPackageContent CompileSkinPackage(IDLCPackage package, LOCFile localisation)
        {
            if (package is not DLCSkinPackage skinPackage)
                return DLCPackageContent.Empty;

            PckFile skinsPck = skinPackage.IsRootPackage ? CreateRootPckFile(package.Identifier, 0, localisation) : new PckFile();
            foreach (KeyValuePair<int, Image> kv in skinPackage.GetCapes())
            {
                PckAsset capeAsset = skinsPck.CreateNewAsset($"dlccape{kv.Key:08}.png", PckAssetType.CapeFile);
                capeAsset.SetTexture(kv.Value);
            }
            foreach (Skin.Skin skin in skinPackage.GetSkins())
            {
                skinsPck.AddSkin("", skin, localisation);
            }

            return new DLCPackageContent(skinsPck);
        }

        internal DLCPackageContent CompileTexturePackage(IDLCPackage package, LOCFile localisation)
        {
            if (package is not DLCTexturePackage texturePackage)
                return DLCPackageContent.Empty;

            PckFile texturePackInfoPck = new PckFile();
            {
                texturePackInfoPck.AddTexture("comparison.png", texturePackage.Info.ComparisonImg);
                texturePackInfoPck.AddTexture("icon.png", texturePackage.Info.IconImg);
            }

            PckFile texturePck = new PckFile();
            {
                texturePck.AddTexture(ResourceLocations.GetPathFromCategory(AtlasResource.GetId(AtlasResource.AtlasType.ParticleAtlas)), texturePackage.GetParticleAtlas());
                texturePck.AddTexture(ResourceLocations.GetPathFromCategory(AtlasResource.GetId(AtlasResource.AtlasType.ItemAtlas)), texturePackage.GetItemsAtlas());
                texturePck.AddTexture(ResourceLocations.GetPathFromCategory(AtlasResource.GetId(AtlasResource.AtlasType.BlockAtlas)), texturePackage.GetTerrainAtlas());
                texturePck.AddTexture(ResourceLocations.GetPathFromCategory(AtlasResource.GetId(AtlasResource.AtlasType.PaintingAtlas)), texturePackage.GetPaintingAtlas());
            }

            if (package.IsRootPackage)
            {
                PckFile mainPck = CreateRootPckFile(package.Identifier, 0, localisation);
                DLCTexturePackage.TextureResolution res = texturePackage.GetResolution();
                PckAsset textureInfoAsset = mainPck.CreateNewAsset($"{res}/{res}Info.pck", PckAssetType.TexturePackInfoFile, new PckFileWriter(texturePackInfoPck, _byteOrder));
                textureInfoAsset.AddProperty("PACKID", "0");
                textureInfoAsset.AddProperty("DATAPATH", $"{res}Data.pck");
                return new DLCPackageContent(mainPck, new NamedData<PckFile>($"{res}Data.pck", texturePck), default);
            }

            return new DLCPackageContent(texturePackInfoPck);
        }

        internal DLCPackageContent CompileMashUpPackage(IDLCPackage package, LOCFile localisation)
        {
            if (package is not DLCMashUpPackage mashUpPackage)
                return DLCPackageContent.Empty;

            PckFile skinsPck = CompileSkinPackage(mashUpPackage.GetSkinPackage(), localisation).MainPck;

            DLCTexturePackage texturePackage = mashUpPackage.GetTexturePackage() as DLCTexturePackage;
            DLCPackageContent texturePackContent = CompileTexturePackage(texturePackage, localisation);
            DLCTexturePackage.TextureResolution res = texturePackage.GetResolution();
            PckFile texturePackInfoPck = texturePackContent.MainPck;
            PckFile texturePck = texturePackContent.DataFolder.TexturePck.Value;

            PckFile mainPck = CreateRootPckFile(package.Identifier, 0, localisation);
            _ = mainPck.CreateNewAssetIf(skinsPck is PckFile && skinsPck.AssetCount > 0, "Skins.pck", PckAssetType.SkinDataFile, new PckFileWriter(skinsPck, _byteOrder));

            if (texturePackInfoPck is PckFile && texturePackInfoPck.AssetCount > 0)
            {
                PckAsset textureInfoAsset = mainPck.CreateNewAsset($"{res}/{res}Info.pck", PckAssetType.TexturePackInfoFile, new PckFileWriter(texturePackInfoPck, _byteOrder));
                textureInfoAsset.AddProperty("PACKID", "0");
                textureInfoAsset.AddProperty("DATAPATH", texturePackContent.DataFolder.TexturePck.Name);
            }


            {
                GameRuleFile grf = mashUpPackage.GetGameRule();
                grf.Header.CompressionType = _compressionType;
                grf.Header.CompressionLevel = _compressionLevel;
                mainPck.CreateNewAsset("GameRule.grf", PckAssetType.GameRulesFile, new GameRuleFileWriter(grf));
            }

            if (mashUpPackage.HasAudioData)
            {
                mainPck.CreateNewAsset("audio.pck", PckAssetType.AudioFile, new PckAudioFileWriter(mashUpPackage.GetAudioPack(), _byteOrder));
                texturePackContent.DataFolder.AddFiles(mashUpPackage.GetAudioFiles());
            }

            return new DLCPackageContent(mainPck, texturePackContent.DataFolder);
        }

        internal DLCPackageContent CompileRawAssets(IDLCPackage package)
        {
            return package is RawAssetDLCPackage rawAssetDLCPackage ? new DLCPackageContent(rawAssetDLCPackage.PckFile) : DLCPackageContent.Empty;
        }
    }
}
