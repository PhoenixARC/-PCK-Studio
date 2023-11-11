using System;
using System.Text;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Numerics;
using System.Windows.Forms;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using OMI.Formats.Languages;
using OMI.Formats.Pck;
using OMI.Workers.Language;
using OMI.Workers.Pck;

using PckStudio.Classes.Utils;
using PckStudio.Conversion.Common.JsonDefinitions;
using System.Diagnostics;
using System.Linq;
using PckStudio.Internal;

namespace PckStudio.Conversion.Bedrock
{
	internal class BedrockSkinExporter
    {
        private IExportContext _exportContext;
        private LOCFile _loc;

        /// <summary>
        /// <see cref="https://learn.microsoft.com/en-us/minecraft/creator/documents/packagingaskinpack#languagesjson"/>
        /// </summary>
        static string[] SupportedBedrockLanguages = new string[]
        {
            "en_US",
            "de_DE",
            "ru_RU",
            "zh_CN",
            "fr_FR",
            "it_IT",
            "pt_BR",
            "fr_CA",
            "zh_TW",
            "es_MX",
            "es_ES",
            "pt_PT",
            "en_GB",
            "ko_KR",
            "ja_JP",
            "nl_NL",
            "bg_BG",
            "cs_CZ",
            "da_DK",
            "el_GR",
            "fi_FI",
            "hu_HU",
            "id_ID",
            "nb_NO",
            "pl_PL",
            "sk_SK",
            "sv_SE",
            "tr_TR",
            "uk_UA"
        };

        static string[] OffsetNames = new string[]
        {
            "HEAD", "HELMET",
            "BODY", "CHEST", "BELT",
            "ARM0", "ARMARMOR0", "SHOULDER0", "TOOL0",
            "ARM1", "ARMARMOR1", "SHOULDER1", "TOOL1",
            "LEG0", "LEGGING0", "BOOT0",
            "LEG1", "LEGGING1", "BOOT1"
        };

        /// <summary>
        /// (Bedrock)Languge to key value dictionary.
        /// See <see cref="SupportedBedrockLanguages"/>.
        /// </summary>
        private Dictionary<string, SortedDictionary<string, string>> bedrockLanguageFiles = new Dictionary<string, SortedDictionary<string, string>>(SupportedBedrockLanguages.Length);

        public BedrockSkinExporter(IExportContext exportContext)
        {
            _exportContext = exportContext;
        }

        private static List<(string, string)> GetSkinOffsets(PckFile.PCKProperties skinProperties)
        {
            List<(string, string)> skinOffsets = new List<(string, string)>();

            string part_offset = "0";

            foreach (string offsetName in OffsetNames)
            {
                var v = skinProperties.FirstOrDefault(prop => prop.Key == "OFFSET" && prop.Value.Equals(offsetName)).Value;
                if (v != default && v.Length >= 2)
                    part_offset = v.Split(' ')[2];

                switch (offsetName)
                {
                    case "HEAD":
                        skinOffsets.Add(("HEADWEAR", part_offset));
                        break;
                    case "BODY":
                        skinOffsets.Add(("JACKET", part_offset));
                        skinOffsets.Add(("BODYARMOR", part_offset));
                        break;
                    case "CHEST":
                        skinOffsets.Add(("BELT", part_offset));
                        skinOffsets.Add(("WAIST", part_offset));
                        break;
                    case "ARM0":
                        skinOffsets.Add(("SLEEVE0", part_offset));
                        skinOffsets.Add(("SHOULDER0", part_offset));
                        break;
                    case "ARM1":
                        skinOffsets.Add(("SLEEVE1", part_offset));
                        skinOffsets.Add(("SHOULDER1", part_offset));
                        break;
                    case "LEG0":
                        skinOffsets.Add(("PANTS0", part_offset));
                        skinOffsets.Add(("SOCK0", part_offset));
                        break;
                    case "LEG1":
                        skinOffsets.Add(("PANTS1", part_offset));
                        skinOffsets.Add(("SOCK1", part_offset));
                        break;
                }

                if (skinOffsets.Find(p => p.Item1 == offsetName) != default!)
                    skinOffsets.Add((offsetName, part_offset));
            }

            return skinOffsets;
        }

        private GeometryCube[] ConvertBoxes(string part, PckFile.FileData file, Vector3 pivot, List<(string, string)> offsets)
        {
            List<GeometryCube> cubes = new List<GeometryCube>();
            var anim = new SkinANIM();
            Debug.WriteLine(part);

            float offset = TryGetOffsetValue(part, offsets);

            foreach (var kv in file.Properties)
            {
                switch (kv.Key)
                {
                    case "ANIM":
                        anim = SkinANIM.FromString(kv.Value);
                        break;
                    case "BOX":
                        SkinBOX box = SkinBOX.FromString(kv.Value);
                        if (box.Type == part)
                        {
                            float y = -1 * (box.Pos.Y + offset + box.Size.Y);
                            cubes.Add(new GeometryCube(new Vector3(pivot.X + box.Pos.X, pivot.Y + y, pivot.Z + box.Pos.Z),
                                box.Size, box.UV, box.Mirror, box.Scale));
                        }
                        break;
                    default:
                        break;
                }
            }

            bool slim = anim.GetFlag(ANIM_EFFECTS.SLIM_MODEL);
            bool classic_res = !(slim && anim.GetFlag(ANIM_EFFECTS.RESOLUTION_64x64));

            switch (part)
            {
                case "HEAD":
                    if (!anim.GetFlag(ANIM_EFFECTS.HEAD_DISABLED))
                        cubes.Add(new GeometryCube(new Vector3(-4, 24 - offset, -4), new Vector3(8, 8, 8), new Vector2(0, 0)));
                    break;
                case "BODY":
                    if (!anim.GetFlag(ANIM_EFFECTS.BODY_DISABLED))
                        cubes.Add(new GeometryCube(new Vector3(-4, 12 - offset, -2), new Vector3(8, 12, 4), new Vector2(16, 16)));
                    break;
                case "ARM0":
                    if (!anim.GetFlag(ANIM_EFFECTS.RIGHT_ARM_DISABLED))
                        cubes.Add(new GeometryCube(new Vector3(slim ? -7 : -8, 12 - offset, -2), new Vector3(slim ? 3 : 4, 12, 4), new Vector2(40, 16)));
                    break;
                case "ARM1":
                    if (!anim.GetFlag(ANIM_EFFECTS.LEFT_ARM_DISABLED))
                        cubes.Add(new GeometryCube(new Vector3(4, 12 - offset, -2), new Vector3(slim ? 3 : 4, 12, 4), classic_res ? new Vector2(40, 16) : new Vector2(32, 48), classic_res));
                    break;
                case "LEG0":
                    if (!anim.GetFlag(ANIM_EFFECTS.RIGHT_LEG_DISABLED))
                        cubes.Add(new GeometryCube(new Vector3(-3.9f, 0 - offset, -2), new Vector3(4, 12, 4), new Vector2(0, 16)));
                    break;
                case "LEG1":
                    if (!anim.GetFlag(ANIM_EFFECTS.LEFT_LEG_DISABLED))
                        cubes.Add(new GeometryCube(new Vector3(0.1f, 0 - offset, -2), new Vector3(4, 12, 4), classic_res ? new Vector2(0, 16) : new Vector2(16, 48), classic_res));
                    break;
                case "HEADWEAR":
                    if (!anim.GetFlag(ANIM_EFFECTS.HEAD_OVERLAY_DISABLED))
                        cubes.Add(new GeometryCube(new Vector3(-4, 24 - offset, -4), new Vector3(8, 8, 8), new Vector2(32, 0), false, 0.5f));
                    break;
                case "JACKET":
                    if (!classic_res && !anim.GetFlag(ANIM_EFFECTS.BODY_OVERLAY_DISABLED))
                        cubes.Add(new GeometryCube(new Vector3(0, 24 - offset, 0), new Vector3(8, 12, 4), new Vector2(16, 32), false, 0.25f));
                    break;
                case "SLEEVE0":
                    if (!classic_res && !anim.GetFlag(ANIM_EFFECTS.RIGHT_ARM_OVERLAY_DISABLED))
                        cubes.Add(new GeometryCube(new Vector3(slim ? -7 : -8, 12 - offset, -2), new Vector3(slim ? 3 : 4, 12, 4), new Vector2(40, 32), false, 0.25f));
                    break;
                case "SLEEVE1":
                    if (!classic_res && !anim.GetFlag(ANIM_EFFECTS.LEFT_ARM_OVERLAY_DISABLED))
                        cubes.Add(new GeometryCube(new Vector3(4, 12 - offset, -2), new Vector3(slim ? 3 : 4, 12, 4), new Vector2(48, 48), false, 0.25f));
                    break;
                case "PANTS0":
                    if (!classic_res && !anim.GetFlag(ANIM_EFFECTS.RIGHT_LEG_OVERLAY_DISABLED))
                        cubes.Add(new GeometryCube(new Vector3(-3.9f, 0 - offset, -2), new Vector3(4, 12, 4), new Vector2(0, 32), false, 0.25f));
                    break;
                case "PANTS1":
                    if (!classic_res && !anim.GetFlag(ANIM_EFFECTS.LEFT_LEG_OVERLAY_DISABLED))
                        cubes.Add(new GeometryCube(new Vector3(0.1f, 0 - offset, -2), new Vector3(4, 12, 4), new Vector2(0, 48), false, 0.25f));
                    break;
                default:
                    break;
            }

            return cubes.ToArray();
        }

        public void ConvertSkinPack(PckFile skinPck)
        {
            SkinJSON skinJson = new SkinJSON();   // Skins.json
            JObject geometryJson = new JObject(); // Geometry.json

            _loc = AcquireLocFile(skinPck) ?? throw new ArgumentNullException($"{nameof(AcquireLocFile)} failed.");

            string bedrockPackName = "DummySkinPack";
            var packNameTranslations = _loc.GetLocEntries("IDS_DISPLAY_NAME");
            if (packNameTranslations.ContainsKey("en-EN"))
            {
                bedrockPackName = packNameTranslations["en-EN"].ToLower().Replace(":", string.Empty).Replace(' ', '_');
            }
            foreach (var item in packNameTranslations)
            {
                string language = item.Key;
                string value = item.Value;
                string bedrockLanguage = language.Replace('-', '_');
                if (SupportedBedrockLanguages.Contains(bedrockLanguage))
                {
                    if (!bedrockLanguageFiles.ContainsKey(bedrockLanguage))
                        bedrockLanguageFiles.Add(bedrockLanguage, new SortedDictionary<string, string>());
                    bedrockLanguageFiles[bedrockLanguage].Add($"skinpack.{bedrockPackName}", value);
                }
            }

            skinJson.Skins = ConvertSkins(skinPck, geometryJson, bedrockPackName);
            skinJson.LocalizationName = skinJson.SerializeName = bedrockPackName;

            // msdocs says it's always "pack.name" ?
            var skinManifest = CreateSkinPackManifest(skinJson.LocalizationName);

            string skinManifest_JSON = JsonConvert.SerializeObject(skinManifest, Formatting.Indented);
            _exportContext.PutNextEntry("manifest.json");
            byte[] skinManifestData = Encoding.UTF8.GetBytes(skinManifest_JSON);
            _exportContext.Write(skinManifestData, 0, skinManifestData.Length);

            string SKINS_JSON = JsonConvert.SerializeObject(skinJson, Formatting.Indented);
            _exportContext.PutNextEntry("skins.json");
            byte[] skinsJsonData = Encoding.UTF8.GetBytes(SKINS_JSON);
            _exportContext.Write(skinsJsonData, 0, skinsJsonData.Length);

            string GEO_JSON = JsonConvert.SerializeObject(geometryJson, Formatting.Indented);
            _exportContext.PutNextEntry("geometry.json");
            byte[] geometryJsonData = Encoding.UTF8.GetBytes(GEO_JSON);
            _exportContext.Write(geometryJsonData, 0, geometryJsonData.Length);
        }


        public void ConvertCape(PckFile.FileData file, SkinObject skin = default!)
        {
            if (file.Filetype != PckFile.FileData.FileType.CapeFile)
                return;
            string capeId = Path.GetFileNameWithoutExtension(file.Filename);
            _exportContext.PutNextEntry($"{capeId}.png");
            _exportContext.Write(file.Data, 0, file.Size);
        }

        private SkinObject ConvertSkin(PckFile.FileData file, string packName, JObject geometryJson)
        {
            if (file.Filetype != PckFile.FileData.FileType.SkinFile)
                return default!;
            
            SkinObject skinObj = new SkinObject();
            skinObj.LocalizationName = Path.GetFileNameWithoutExtension(file.Filename);
            skinObj.TextureName = Path.GetFileName(file.Filename);
            skinObj.GeometryName = $"geometry.{packName}.{skinObj.LocalizationName}";

            Vector3 head_and_body_pivot = new Vector3(0, 24, 0);
            Vector3 right_arm_pivot = new Vector3(-5, 22, 0);
            Vector3 left_arm_pivot = new Vector3(5, 22, 0);
            Vector3 right_leg_pivot = new Vector3(-1.9f, 12, 0);
            Vector3 left_leg_pivot = new Vector3(1.9f, 12, 0);

            var offsets = GetSkinOffsets(file.Properties);

            var geometry = new Geometry(
                new GeometryBone("head", null, head_and_body_pivot, ConvertBoxes("HEAD", file, head_and_body_pivot, offsets), metaBone: "base"),
                new GeometryBone("body", null, head_and_body_pivot, ConvertBoxes("BODY", file, head_and_body_pivot, offsets), metaBone: "base"),

                new GeometryBone("rightArm", null, right_arm_pivot, ConvertBoxes("ARM0", file, right_arm_pivot, offsets), metaBone: "base"),
                new GeometryBone("leftArm", null, left_arm_pivot, ConvertBoxes("ARM1", file, left_arm_pivot, offsets), metaBone: "base"),
                new GeometryBone("rightLeg", null, right_leg_pivot, ConvertBoxes("LEG0", file, right_leg_pivot, offsets), metaBone: "base"),
                new GeometryBone("leftLeg", null, left_leg_pivot, ConvertBoxes("LEG1", file, left_leg_pivot, offsets), metaBone: "base"),

                new GeometryBone("hat", "head", head_and_body_pivot, ConvertBoxes("HEADWEAR", file, head_and_body_pivot, offsets), metaBone: "clothing"),
                new GeometryBone("jacket", "body", head_and_body_pivot, ConvertBoxes("JACKET", file, head_and_body_pivot, offsets), metaBone: "clothing"),

                new GeometryBone("rightSleeve", "rightArm", right_arm_pivot, ConvertBoxes("SLEEVE0", file, right_arm_pivot, offsets), metaBone: "clothing"),
                new GeometryBone("leftSleeve", "leftArm", left_arm_pivot, ConvertBoxes("SLEEVE1", file, left_arm_pivot, offsets), metaBone: "clothing"),
                new GeometryBone("rightPants", "rightLeg", right_leg_pivot, ConvertBoxes("PANTS0", file, right_leg_pivot, offsets), metaBone: "clothing"),
                new GeometryBone("leftPants", "leftLeg", left_leg_pivot, ConvertBoxes("PANTS1", file, left_leg_pivot, offsets), metaBone: "clothing"),
                new GeometryBone("rightSock", "rightLeg", right_leg_pivot, ConvertBoxes("SOCK0", file, right_leg_pivot, offsets), metaBone: "clothing"),
                new GeometryBone("leftSock", "leftLeg", left_leg_pivot, ConvertBoxes("SOCK1", file, left_leg_pivot, offsets), metaBone: "clothing"),

                new GeometryBone("helmet", "head", head_and_body_pivot, ConvertBoxes("HELMET", file, head_and_body_pivot, offsets), "armor"),
                new GeometryBone("bodyArmor", "body", head_and_body_pivot, ConvertBoxes("BODYARMOR", file, head_and_body_pivot, offsets), "armor"),

                new GeometryBone("belt", "body", head_and_body_pivot, ConvertBoxes("BELT", file, head_and_body_pivot, offsets), metaBone: "armor"),
                new GeometryBone("rightArmArmor", "rightArm", right_arm_pivot, ConvertBoxes("ARMARMOR0", file, right_arm_pivot, offsets), metaBone: "armor"),
                new GeometryBone("leftArmArmor", "leftArm", left_arm_pivot, ConvertBoxes("ARMARMOR1", file, left_arm_pivot, offsets), metaBone: "armor"),
                new GeometryBone("rightLegArmor", "rightLeg", right_leg_pivot, ConvertBoxes("LEGGING0", file, right_leg_pivot, offsets), metaBone: "armor"),
                new GeometryBone("leftLegArmor", "leftLeg", left_leg_pivot, ConvertBoxes("LEGGING1", file, left_leg_pivot, offsets), metaBone: "armor"),
                new GeometryBone("rightBoot", "rightLeg", right_leg_pivot, ConvertBoxes("BOOT0", file, right_leg_pivot, offsets), metaBone: "armor"),
                new GeometryBone("leftBoot", "leftLeg", left_leg_pivot, ConvertBoxes("BOOT1", file, left_leg_pivot, offsets), metaBone: "armor"),

                // calculates armor and item offsets
                new GeometryBone("rightItem", "rightArm", new Vector3(-6.0f, 15.0f - TryGetOffsetValue("TOOL0", offsets), 1.0f), null, "item"),
                new GeometryBone("leftItem", "leftArm", new Vector3(6.0f, 15.0f - TryGetOffsetValue("TOOL1", offsets), 1.0f), null, "item"),

                new GeometryBone("helmetArmorOffset", "head", new Vector3(0f, 24f - TryGetOffsetValue("HEAD", offsets), 0f), null, "armor_offset"),
                new GeometryBone("bodyArmorOffset", "body", new Vector3(-4f, 12f - TryGetOffsetValue("BODY", offsets), -2f), null, "armor_offset"),
                new GeometryBone("rightArmArmorOffset", "rightArm", new Vector3(4f, 12f - TryGetOffsetValue("ARM0", offsets), -2f), null, "armor_offset"),
                new GeometryBone("leftArmArmorOffset", "leftArm", new Vector3(-8f, 12f - TryGetOffsetValue("ARM1", offsets), -2f), null, "armor_offset"),
                new GeometryBone("rightLegArmorOffset", "rightLeg", new Vector3(-0.1f, TryGetOffsetValue("LEG0", offsets), -2f), null, "armor_offset"),
                new GeometryBone("leftLegArmorOffset", "leftLeg", new Vector3(-4.1f, TryGetOffsetValue("LEG1", offsets), -2f), null, "armor_offset"),
                new GeometryBone("rightBootArmorOffset", "rightLeg", new Vector3(2f, 12f - TryGetOffsetValue("BOOT0", offsets), 0f), null, "armor_offset"),
                new GeometryBone("leftBootArmorOffset", "leftLeg", new Vector3(-2f, 12f - TryGetOffsetValue("BOOT1", offsets), 0f), null, "armor_offset")
            );

            var ANIM = SkinANIM.FromString(file.Properties.Find(o => o.Key == "ANIM").Value) ?? SkinANIM.Empty;
            bool IsClassicRes = !(ANIM.GetFlag(ANIM_EFFECTS.RESOLUTION_64x64) || ANIM.GetFlag(ANIM_EFFECTS.SLIM_MODEL));

            geometry.TextureWidth = 64;
            geometry.TextureHeight = IsClassicRes ? 32 : 64;
            geometry.AnimationArmsDown = ANIM.GetFlag(ANIM_EFFECTS.STATIC_ARMS);
            geometry.AnimationArmsOutFront = ANIM.GetFlag(ANIM_EFFECTS.ZOMBIE_ARMS);
            geometry.AnimationDontShowArmor = ANIM.GetFlag(ANIM_EFFECTS.ALL_ARMOR_DISABLED);
            geometry.AnimationInvertedCrouch = ANIM.GetFlag(ANIM_EFFECTS.DO_BACKWARDS_CROUCH);
            geometry.AnimationNoHeadBob = ANIM.GetFlag(ANIM_EFFECTS.HEAD_BOBBING_DISABLED);
            geometry.AnimationSingleArmAnimation = ANIM.GetFlag(ANIM_EFFECTS.SYNCED_ARMS);
            geometry.AnimationSingleLegAnimation = ANIM.GetFlag(ANIM_EFFECTS.SYNCED_LEGS);
            geometry.AnimationStationaryLegs = ANIM.GetFlag(ANIM_EFFECTS.STATIC_LEGS);
            geometry.AnimationStatueOfLibertyArms = ANIM.GetFlag(ANIM_EFFECTS.STATUE_OF_LIBERTY);
            geometry.AnimationUpsideDown = ANIM.GetFlag(ANIM_EFFECTS.DINNERBONE);

            string capepath = file.Properties.Find(o => o.Key == "CAPEPATH").Value;

            JToken geo = JToken.FromObject(geometry);
            if (capepath != null)
                geo["cape"] = capepath;

            geometryJson.Add(skinObj.GeometryName, geo);
            _exportContext.PutNextEntry(skinObj.TextureName);
            _exportContext.Write(file.Data, 0, file.Size);
            return skinObj;
        }

        private float TryGetOffsetValue(string name, List<(string, string)> offsets)
        {
            _ = name ?? throw new ArgumentNullException(nameof(name));
            _ = offsets ?? throw new ArgumentNullException(nameof(offsets));
            var offsetValue = offsets.Find(o => o.Item1.Equals(name));
            if (offsetValue.Equals(default))
                return 0.0f;
            float value = 0;
            float.TryParse(offsetValue.Item2, out value);
            return value;
        }

        private LOCFile AcquireLocFile(PckFile sourcePck)
        {
            if (sourcePck.TryGetFile("localisation.loc", PckFile.FileData.FileType.LocalisationFile, out PckFile.FileData locFileData) ||
                sourcePck.TryGetFile("languages.loc", PckFile.FileData.FileType.LocalisationFile, out locFileData))
            {
                var reader = new LOCFileReader();
                LOCFile locFile;
                using (var ms = new MemoryStream(locFileData.Data))
                {
                    locFile = reader.FromStream(ms);
                }
                return locFile;
            }
            return null;
        }

        private SkinObject[] ConvertSkins(PckFile sourcePck, JObject geometryJson, string packName)
        {
            List<SkinObject> skins = new List<SkinObject>();
            foreach (PckFile.FileData file in sourcePck.Files)
            {
                switch (file.Filetype)
                {
                    case PckFile.FileData.FileType.SkinFile:
                        var skin = ConvertSkin(file, packName, geometryJson);
                        if (skin != null)
                            skins.Add(skin);
                        break;
                    case PckFile.FileData.FileType.CapeFile:
                        ConvertCape(file);
                        break;
                    case PckFile.FileData.FileType.SkinDataFile:
                        var reader = new PckFileReader();
                        using (var ms = new MemoryStream(file.Data))
                        {
                            PckFile subPack = reader.FromStream(ms);
                            skins.AddRange(ConvertSkins(subPack, geometryJson, packName));
                        }
                        break;
                }
            }
            return skins.ToArray();
        }

        [Obsolete]
        void ExportLOC(LOCFile locFile, string packName)
        {
            List<string> supportedLanguages = new List<string>();

            foreach (var language in locFile.Languages)
            {
                // string bedrockLang = languageMap[language];
                string bedrockLang = language.Replace('-', '_');
                if (!SupportedBedrockLanguages.Contains(bedrockLang))
                    continue;

                supportedLanguages.Add(bedrockLang);
                _exportContext.PutNextEntry($"texts/{bedrockLang}.lang");
                //var languageKeyToValue = locFile.GetLanguage(language);
                //var wrapper = new MemoryStream();
                //using (var stringWriter = new StreamWriter(wrapper, Encoding.UTF8, 1024, leaveOpen: true))
                //{
                //    foreach (var kvp in languageKeyToValue)
                //    {
                //        if (kvp.Key.EndsWith("_THEMENAME"))
                //            continue;
                //        string keyName = string.Empty;
                //        if (kvp.Key.Equals("IDS_DISPLAY_NAME"))
                //        {
                //            keyName = $"skinpack.{packName}";
                //        }

                //        if (kvp.Key.StartsWith("IDS_DLCSKIN"))
                //        {
                //            keyName = $"skin.{packName}.{kvp.Key}";
                //        }

                //        stringWriter.WriteLine($"{keyName}={kvp.Value}");
                //    }
                //}
                //byte[] data = wrapper.ToArray();
                //_exportContext.Write(data, 0, data.Length);
            }
            string serializedLanguages = JsonConvert.SerializeObject(supportedLanguages.ToArray(), Formatting.Indented);
            var serializedLanguageData = Encoding.UTF8.GetBytes(serializedLanguages);

            _exportContext.PutNextEntry("languages.json");
            _exportContext.Write(serializedLanguageData, 0, serializedLanguageData.Length);
        }

        Manifest CreateSkinPackManifest(string name)
        {
            return new Manifest
            {
                Header = new ManifestHeader()
                {
                    Name = name,
                    Description = "Exported with " + Application.ProductName,
                },
                Modules = new ManifestModule[1]
                {
                    new ManifestModule("skin_pack")
                }
            };
        }
    }
}
