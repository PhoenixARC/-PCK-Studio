/*
 * Author: MikuX666 (Github: https://github.com/NessieHax)
 * See License usage at the bottom of file!
*/
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using OMI;
using OMI.Formats.Archive;
using OMI.Formats.GameRule;
using OMI.Formats.Languages;
using OMI.Formats.Pck;
using OMI.Workers.Archive;
using OMI.Workers.GameRule;
using OMI.Workers.Language;
using OMI.Workers.Pck;
using PckStudio.Core.Extensions;
using PckStudio.Core.Interfaces;
using PckStudio.Core.IO.PckAudio;
using PckStudio.Core.Properties;
using PckStudio.Core.Serializer;

namespace PckStudio.Core.DLC
{
    internal sealed class PckFileCompiler
    {
        private readonly string _language;
        private ByteOrder _byteOrder;
        private GameRuleFile.CompressionType _compressionType;
        private GameRuleFile.CompressionLevel _compressionLevel;

        internal PckFileCompiler(DLCManager dlcManager)
        {
            _language = dlcManager.PreferredLanguage;
            _byteOrder = dlcManager.ByteOrder;
            _compressionType = dlcManager.GetPlatformCompressionType();
            _compressionLevel = GameRuleFile.CompressionLevel.None;
        }

        private PckFile CreateRootPckFile(int packId, int packVerison)
        {
            PckFile mainPck = new PckFile(3, true);
            PckAsset meta = mainPck.CreateNewAsset("0", PckAssetType.InfoFile);
            meta.AddProperty("PACKID", packId);
            meta.AddProperty("PACKVERSION", packVerison);
            return mainPck;
        }

        internal DLCPackageContent CompileSkinPackage(IDLCPackage package, LOCFile localisation)
        {
            if (package is not DLCSkinPackage skinPackage)
                return DLCPackageContent.Empty;

            PckFile pckFile = new PckFile();
            if (skinPackage.IsRootPackage)
            {
                localisation.AddLanguage(_language);
                localisation.AddLocKey(DLCManager.PACKAGE_DISPLAYNAME_ID, package.Name);
                pckFile = CreateRootPckFile(package.Identifier, 0);
            }
            foreach (KeyValuePair<int, Image> kv in skinPackage.GetCapes())
            {
                PckAsset capeAsset = pckFile.CreateNewAsset($"dlccape{kv.Key:08}.png", PckAssetType.CapeFile);
                capeAsset.SetTexture(kv.Value);
            }
            foreach (Skin.Skin skin in skinPackage.GetSkins())
            {
                pckFile.AddSkin("", skin, localisation);
            }

            pckFile.CreateNewAsset("localisation.loc", PckAssetType.LocalisationFile, new LOCFileWriter(localisation, 2));
            return new DLCPackageContent(package.Name, pckFile);
        }

        internal DLCPackageContent CompileTexturePackage(IDLCPackage package, LOCFile localisation)
        {
            if (package is not DLCTexturePackage texturePackage)
                return DLCPackageContent.Empty;

            string resourceFolderName = "res";

            PckFile texturePackInfoPck = new PckFile();
            {
                Image comparison = texturePackage.Info?.ComparisonImg ?? Resources.Comparison;
                Image icon = texturePackage.Info?.IconImg ?? Resources.TexturePackIcon;
                texturePackInfoPck.AddTexture("comparison.png", comparison);
                texturePackInfoPck.AddTexture("icon.png", icon);
            }

            PckFile texturePck = new PckFile();
            {
                texturePck.CreateNewAsset("0", PckAssetType.InfoFile);

                texturePck.AddTexture(ResourceLocations.GetPathFromCategory(AtlasResource.GetId(AtlasResource.AtlasType.BlockAtlas)), texturePackage.GetTerrainAtlas(), 2);

                texturePck.AddTexture(ResourceLocations.GetPathFromCategory(AtlasResource.GetId(AtlasResource.AtlasType.ItemAtlas)), texturePackage.GetItemsAtlas());
                texturePck.AddTexture(ResourceLocations.GetPathFromCategory(AtlasResource.GetId(AtlasResource.AtlasType.ParticleAtlas)), texturePackage.GetParticleAtlas());
                texturePck.AddTexture(ResourceLocations.GetPathFromCategory(AtlasResource.GetId(AtlasResource.AtlasType.PaintingAtlas)), texturePackage.GetPaintingAtlas());
                texturePck.AddTexture(ResourceLocations.GetPathFromCategory(AtlasResource.GetId(AtlasResource.AtlasType.MoonPhaseAtlas)), texturePackage.GetMoonPhaseAtlas());

                texturePck.AddTexture($"{resourceFolderName}/terrain/sun.png", texturePackage.GetSunTexture());
                texturePck.AddTexture($"{resourceFolderName}/terrain/moon.png", texturePackage.GetMoonTexture());

                string itemAnimationsPath = ResourceLocations.GetPathFromCategory(ResourceCategory.ItemAnimation);
                foreach (KeyValuePair<string, Animation> animation in texturePackage.GetItemAnimations())
                {
                    string path = Path.Combine(itemAnimationsPath, animation.Key).Replace("\\", "/");
                    PckAsset asset = texturePck.CreateNewAsset(path + ".png", PckAssetType.TextureFile);
                    asset.SetSerializedData(animation.Value, AnimationSerializer.DefaultSerializer);
                    if (animation.Key == "clock" || animation.Key == "compass")
                    {
                        asset.SetProperty("ANIM", "");
                    }
                }

                string blockAnimationsPath = ResourceLocations.GetPathFromCategory(ResourceCategory.BlockAnimation);
                foreach (KeyValuePair<string, Animation> animation in texturePackage.GetBlockAnimations())
                {
                    string path = Path.Combine(blockAnimationsPath, animation.Key).Replace("\\", "/");
                    PckAsset asset = texturePck.CreateNewAsset(path + ".png", PckAssetType.TextureFile);
                    asset.SetSerializedData(animation.Value, AnimationSerializer.DefaultSerializer);
                }

                string armorTexturePath = ResourceLocations.GetPathFromCategory(ResourceCategory.ArmorTextures);
                foreach (NamedData<Image> armorTexture in texturePackage.GetArmorSets().SelectMany(ArmorSetDescription.GetArmorSetTextures))
                {
                    string path = Path.Combine(armorTexturePath, armorTexture.Name).Replace("\\", "/");
                    PckAsset asset = texturePck.CreateNewAsset(path + ".png", PckAssetType.TextureFile);
                    asset.SetSerializedData(armorTexture.Value, ImageSerializer.DefaultSerializer);
                }

                DLCTexturePackage.EnvironmentData environmentData = texturePackage.GetEnvironmentData();

                texturePck.AddTexture($"{resourceFolderName}/environment/rain.png", environmentData.Rain);
                texturePck.AddTexture($"{resourceFolderName}/environment/snow.png", environmentData.Snow);
                texturePck.AddTexture($"{resourceFolderName}/environment/clouds.png", environmentData.Clouds);

                {
                    int i = 0;
                    foreach (Animation.Frame frame in texturePackage.GetBlockEntityBreakAnimation().GetFrames())
                    {
                        texturePck.AddTexture($"res/textures/destroy_stage_{i++}.png", frame.Texture);
                    }
                }

                {
                    string mapIconAtlasPath = ResourceLocations.GetPathFromCategory(AtlasResource.GetId(AtlasResource.AtlasType.MapIconAtlas)).Replace("\\", "/");
                    string additionalMapIconsAtlasPath = ResourceLocations.GetPathFromCategory(AtlasResource.GetId(AtlasResource.AtlasType.AdditionalMapIconsAtlas)).Replace("\\", "/");
                    PckAsset mapIconAtlasAsset = texturePck.CreateNewAsset(mapIconAtlasPath, PckAssetType.TextureFile);
                    PckAsset additionalMapIconsAtlasAsset = texturePck.CreateNewAsset(additionalMapIconsAtlasPath, PckAssetType.TextureFile);
                    mapIconAtlasAsset.SetTexture(Resources.map_icons_atlas);
                    additionalMapIconsAtlasAsset.SetTexture(Resources.additional_map_icons_atlas);
                }

                //{
                //    PckAsset modelsAsset = texturePck.CreateNewAsset("models.bin", PckAssetType.ModelsFile);
                //    modelsAsset.SetData(new ModelFileWriter(new ModelContainer(), 0));
                //}

                //{
                //    PckAsset colorsAsset = texturePck.CreateNewAsset("colours.col", PckAssetType.ColourTableFile);
                //    colorsAsset.SetData(Resources.tu69colours);
                //}

                {
                    foreach (KeyValuePair<string, Image> itemTexture in texturePackage.GetItemModelTextures())
                    {
                        texturePck.AddTexture($"{resourceFolderName}/{itemTexture.Key}.png", itemTexture.Value);
                    }
                    foreach (KeyValuePair<string, Image> mobTexture in texturePackage.GetMobModelTextures())
                    {
                        texturePck.AddTexture($"{resourceFolderName}/{mobTexture.Key}.png", mobTexture.Value);
                    }
                }

                {
                    foreach (KeyValuePair<string, Image> item in texturePackage.GetMisc())
                    {
                        texturePck.AddTexture($"{resourceFolderName}/misc/{item.Key}.png", item.Value);
                    }
                }

            }

            if (package.IsRootPackage)
            {
                localisation.AddLanguage(_language);
                PckFile mainPck = CreateRootPckFile(package.Identifier, 0);               
                localisation.AddLocKey(DLCManager.PACKAGE_DISPLAYNAME_ID, package.Name);
                localisation.AddLocKey(DLCTexturePackage.TEXTUREPACK_DESCRIPTION_ID, package.Description);
                DLCTexturePackage.TextureResolution res = texturePackage.GetResolution();
                localisation.AddLocKey($"IDS_{res.ToString().ToUpper()}_{res.ToString().ToUpper()}INFO_DISPLAYNAME", "");
                PckAsset textureInfoAsset = mainPck.CreateNewAsset($"{res}/{res}Info.pck", PckAssetType.TexturePackInfoFile, new PckFileWriter(texturePackInfoPck, _byteOrder));
                textureInfoAsset.AddProperty("PACKID", "0");
                textureInfoAsset.AddProperty("DATAPATH", $"{res}Data.pck");

                PckAsset loc = new PckAsset("languages.loc", PckAssetType.LocalisationFile);
                loc.SetData(new LOCFileWriter(localisation, 2));

                //! LOC file needs to be the first asset in the pack -_-.... don't ask why.. -null
                mainPck.InsertAsset(0, loc);

                NamedData<byte[]>[] dataFiles = Array.Empty<NamedData<byte[]>>();
                if (texturePackage.GetMediaArc() is ConsoleArchive arc)
                {
                    MemoryStream stream = new MemoryStream();
                    new ARCFileWriter(arc).WriteToStream(stream);
                    dataFiles = [new NamedData<byte[]>("media.arc", stream.ToArray())];
                }

                return new DLCPackageContent(package.Name, mainPck, new NamedData<PckFile>($"{res}Data.pck", texturePck), dataFiles);
            }

            return new DLCPackageContent(package.Name, texturePackInfoPck);
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

            PckFile mainPck = CreateRootPckFile(package.Identifier, 0);
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

            return new DLCPackageContent(package.Name, mainPck, texturePackContent.DataFolder);
        }

        internal DLCPackageContent CompileRawAssets(IDLCPackage package)
        {
            return package is RawAssetDLCPackage rawAssetDLCPackage ? new DLCPackageContent(package.Name, rawAssetDLCPackage.PckFile) : DLCPackageContent.Empty;
        }
    }
}
/* Copyright (c) 2026-present miku-666
 * This software is provided 'as-is', without any express or implied
 * warranty. In no event will the authors be held liable for any damages
 * arising from the use of this software.
 * 
 * Permission is granted to anyone to use this software for any purpose,
 * including commercial applications, and to alter it and redistribute it
 * freely, subject to the following restrictions:
 * 
 * 1.The origin of this software must not be misrepresented; you must not
 *    claim that you wrote the original software. If you use this software
 *    in a product, an acknowledgment in the product documentation would be
 *    appreciated but is not required.
 * 2. Altered source versions must be plainly marked as such, and must not be
 *    misrepresented as being the original software.
 * 3. This notice may not be removed or altered from any source distribution.
**/