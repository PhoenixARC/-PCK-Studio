using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PckStudio.External.Format;
using PckStudio.Forms.Additional_Popups;
using System.Windows.Forms;
using PckStudio.Properties;
using System.IO;
using PckStudio.Extensions;
using System.Numerics;
using PckStudio.Internal.FileFormats;
using PckStudio.Internal.IO.PSM;
using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using PckStudio.Rendering;
using System.Diagnostics;
using System.Xml.Linq;
using System.Drawing.Imaging;

namespace PckStudio.Internal
{
    internal static class ModelImporter
    {
        internal static readonly FileDialogFilter[] fileFilters =
        [
            new ("Pck skin model(*.psm)", "*.psm"),
            new ("Block bench model(*.bbmodel)", "*.bbmodel"),
            new ("Bedrock (Legacy) Model(*.geo.json;*.json)", "*.geo.json;*.json"),
        ];

        internal static string SupportedModelFileFormatsFilter => string.Join("|", fileFilters);

        internal static SkinModelInfo Import(string fileName)
        {
            string fileExtension = Path.GetExtension(fileName);
            switch (fileExtension)
            {
                case ".psm":
                    return ImportPsm(fileName);
                case ".bbmodel":
                    return ImportBlockBenchModel(fileName);
                case ".json":
                    return ImportBedrockJson(fileName);
                default:
                    Trace.TraceWarning($"[{nameof(ModelImporter)}:Import] Model file format: '{fileExtension}' is not supported.");
                    return null;
            }
        }

        internal static void Export(string fileName, SkinModelInfo model)
        {
            if (model is null)
            {
                Trace.TraceError($"[{nameof(ModelImporter)}:Export] Model is null.");
                return;
            }
            string fileExtension = Path.GetExtension(fileName);
            switch (fileExtension)
            {
                case ".psm":
                    ExportPsm(fileName, model);
                    break;
                case ".bbmodel":
                    ExportBlockBenchModel(fileName, model);
                    break;
                case ".json":
                    ExportBedrockJson(fileName, model);
                    break;
                default:
                    Trace.TraceWarning($"[{nameof(ModelImporter)}:Export] Model file format: '{fileExtension}' is not supported.");
                    return;
            }
        }

        internal static SkinModelInfo ImportPsm(string fileName)
        {
            var reader = new PSMFileReader();
            PSMFile csmbFile = reader.FromFile(fileName);
            return new SkinModelInfo(null, csmbFile.SkinANIM, csmbFile.Parts, csmbFile.Offsets);
        }

        internal static void ExportPsm(string fileName, SkinModelInfo modelInfo)
        {
            PSMFile psmFile = new PSMFile(PSMFile.CurrentVersion, modelInfo.ANIM);
            psmFile.Parts.AddRange(modelInfo.AdditionalBoxes);
            psmFile.Offsets.AddRange(modelInfo.PartOffsets);
            var writer = new PSMFileWriter(psmFile);
            writer.WriteToFile(fileName);
        }

        internal static SkinModelInfo ImportBlockBenchModel(string fileName)
        {
            BlockBenchModel blockBenchModel = JsonConvert.DeserializeObject<BlockBenchModel>(File.ReadAllText(fileName));
            SkinModelInfo modelInfo = new SkinModelInfo();
            modelInfo.ANIM.SetMask(
                   SkinAnimMask.HEAD_DISABLED |
                   SkinAnimMask.HEAD_OVERLAY_DISABLED |
                   SkinAnimMask.BODY_DISABLED |
                   SkinAnimMask.BODY_OVERLAY_DISABLED |
                   SkinAnimMask.RIGHT_ARM_DISABLED |
                   SkinAnimMask.RIGHT_ARM_OVERLAY_DISABLED |
                   SkinAnimMask.LEFT_ARM_DISABLED |
                   SkinAnimMask.LEFT_ARM_OVERLAY_DISABLED |
                   SkinAnimMask.RIGHT_LEG_DISABLED |
                   SkinAnimMask.RIGHT_LEG_OVERLAY_DISABLED |
                   SkinAnimMask.LEFT_LEG_DISABLED |
                   SkinAnimMask.LEFT_LEG_OVERLAY_DISABLED);

            if (blockBenchModel.Textures.IndexInRange(0))
            {
                modelInfo.Texture = blockBenchModel.Textures[0].GetTexture();
                modelInfo.ANIM.SetFlag(SkinAnimFlag.RESOLUTION_64x64, modelInfo.Texture.Size.Width == modelInfo.Texture.Size.Height);
            }


            foreach (JToken token in blockBenchModel.Outliner)
            {
                if (token.Type == JTokenType.String && Guid.TryParse((string)token, out Guid tokenGuid))
                {
                    Element element = blockBenchModel.Elements.First(e => e.Uuid.Equals(tokenGuid));
                    if (!SkinBOX.IsValidType(element.Name) || element.Type != "cube")
                        continue;
                    LoadElement(element.Name, element, ref modelInfo);
                    continue;
                }
                if (token.Type == JTokenType.Object)
                {
                    Outline outline = token.ToObject<Outline>();
                    string type = outline.Name;
                    if (!SkinBOX.IsValidType(type))
                        continue;
                    ReadOutliner(token, type, blockBenchModel.Elements, ref modelInfo);
                }
            }
            return modelInfo;
        }

        private static void ReadOutliner(JToken token, string type, IReadOnlyCollection<Element> elements, ref SkinModelInfo modelInfo)
        {
            if (TryReadElement(token, type, elements, ref modelInfo))
                return;

            if (token.Type == JTokenType.Object)
            {
                Outline outline = token.ToObject<Outline>();
                foreach (JToken childToken in outline.Children)
                {
                    ReadOutliner(childToken, type, elements, ref modelInfo);
                }
            }
        }

        private static bool TryReadElement(JToken token, string type, IReadOnlyCollection<Element> elements, ref SkinModelInfo modelInfo)
        {
            if (token.Type == JTokenType.String && Guid.TryParse((string)token, out Guid tokenGuid))
            {
                Element element = elements.First(e => e.Uuid.Equals(tokenGuid));
                LoadElement(type, element, ref modelInfo);
                return true;
            }
            return false;
        }

        private static void LoadElement(string boxType, Element element, ref SkinModelInfo modelInfo)
        {
            if (!element.UseBoxUv || !element.IsVisibile)
                return;

            //Debug.WriteLine($"{type} {element.Name}({element.Uuid})");
            BoundingBox boundingBox = new BoundingBox(element.From, element.To);
            Vector3 pos = boundingBox.Start;
            Vector3 size = boundingBox.Volume;
            Vector2 uv = element.UvOffset;
            pos = TranslateToInternalPosition(boxType, pos, size, new Vector3(1, 1, 0));
            //Debug.WriteLine(pos);

            var box = new SkinBOX(boxType, pos, size, uv);
            if (box.IsBasePart() && ((boxType == "HEAD" && element.Inflate == 0.5f) || (element.Inflate == 0.25f)))
                box.Type = box.GetOverlayType();

            // IMPROVMENT: detect default body parts and toggle anim flag instead of adding box data -miku
            //int hash = box.GetHashCode();
            //if (SkinBOX.KnownHashes.ContainsKey(hash))
            //{
            //    Debug.WriteLine("Found known hash of " + box.ToString());
            //    modelInfo.ANIM.SetFlag(SkinBOX.KnownHashes[hash], false);
            //    return;
            //}
            modelInfo.AdditionalBoxes.Add(box);
        }

        internal static void ExportBlockBenchModel(string fileName, SkinModelInfo modelInfo)
        {
            BlockBenchModel blockBenchModel = new BlockBenchModel()
            {
                Textures = new Texture[] { modelInfo.Texture },
                TextureResolution = modelInfo.Texture.Size,
                ModelIdentifier = "",
                Metadata = new Meta()
                {
                    FormatVersion = "4.5",
                    ModelFormat = "free",
                    UseBoxUv = true,
                }
            };

            Dictionary<string, Outline> outliners = new Dictionary<string, Outline>(5);
            List<Element> elements = new List<Element>(modelInfo.AdditionalBoxes.Count);

            void AddElement(SkinBOX box)
            {
                if (!outliners.ContainsKey(box.Type))
                {
                    outliners.Add(box.Type, new Outline(box.Type));
                }

                Element element = CreateElement(box);
                elements.Add(element);
                outliners[box.Type].Children.Add(element.Uuid);
            }

            ANIM2BOX(modelInfo.ANIM, AddElement);

            foreach (var box in modelInfo.AdditionalBoxes)
            {
                AddElement(box);
            }
            blockBenchModel.Elements = elements.ToArray();
            blockBenchModel.Outliner = JArray.FromObject(outliners.Values);

            string content = JsonConvert.SerializeObject(blockBenchModel);
            File.WriteAllText(fileName, content);
        }

        private static Element CreateElement(SkinBOX box)
        {
            Element element = new Element
            {
                Name = "cube",
                UseBoxUv = true,
                Locked = false,
                Rescale = false,
                Type = "cube",
                Uuid = Guid.NewGuid(),
                UvOffset = box.UV,
                MirrorUv = box.Mirror
            };
            Vector3 transformPos = TranslateFromInternalPosistion(box, new Vector3(1, 1, 0));

            element.From = transformPos;
            element.To = transformPos + box.Size;
            if (box.IsOverlayPart())
                element.Inflate = box.Type == "HEADWEAR" ? 0.5f : 0.25f;
            return element;
        }

        internal static SkinModelInfo ImportBedrockJson(string fileName)
        {
            Geometry selectedGeometry = null;
            // Bedrock Entity (Model)
            if (fileName.EndsWith(".geo.json"))
            {
                BedrockModel bedrockModel = JsonConvert.DeserializeObject<BedrockModel>(File.ReadAllText(fileName));
                ItemSelectionPopUp itemSelectionPopUp = new ItemSelectionPopUp(bedrockModel.Models.Select(m => m.Description.Identifier).ToArray());
                if (itemSelectionPopUp.ShowDialog() == DialogResult.OK && bedrockModel.Models.IndexInRange(itemSelectionPopUp.SelectedIndex))
                {
                    selectedGeometry = bedrockModel.Models[itemSelectionPopUp.SelectedIndex];
                }
                itemSelectionPopUp.Dispose();
            }

            // Bedrock Legacy Model
            else if (fileName.EndsWith(".json"))
            {
                BedrockLegacyModel bedrockModel = JsonConvert.DeserializeObject<BedrockLegacyModel>(File.ReadAllText(fileName));
                ItemSelectionPopUp itemSelectionPopUp = new ItemSelectionPopUp(bedrockModel.Select(m => m.Key).ToArray());
                if (itemSelectionPopUp.ShowDialog() == DialogResult.OK && itemSelectionPopUp.SelectedItem is not null)
                {
                    selectedGeometry = bedrockModel[itemSelectionPopUp.SelectedItem];
                }
                itemSelectionPopUp.Dispose();
            }

            SkinModelInfo modelInfo = null;
            if (selectedGeometry is not null)
            {
                modelInfo = LoadGeometry(selectedGeometry);
                modelInfo.ANIM.SetMask(
                    SkinAnimMask.RESOLUTION_64x64 |
                    SkinAnimMask.HEAD_DISABLED |
                    SkinAnimMask.HEAD_OVERLAY_DISABLED |
                    SkinAnimMask.BODY_DISABLED |
                    SkinAnimMask.BODY_OVERLAY_DISABLED |
                    SkinAnimMask.RIGHT_ARM_DISABLED |
                    SkinAnimMask.RIGHT_ARM_OVERLAY_DISABLED |
                    SkinAnimMask.LEFT_ARM_DISABLED |
                    SkinAnimMask.LEFT_ARM_OVERLAY_DISABLED |
                    SkinAnimMask.RIGHT_LEG_DISABLED |
                    SkinAnimMask.RIGHT_LEG_OVERLAY_DISABLED |
                    SkinAnimMask.LEFT_LEG_DISABLED |
                    SkinAnimMask.LEFT_LEG_OVERLAY_DISABLED);
            }
            return modelInfo;
        }

        private static SkinModelInfo LoadGeometry(Geometry geometry)
        {
            SkinModelInfo modelInfo = new SkinModelInfo();
            foreach (Bone bone in geometry.Bones)
            {
                string boxType = bone.Name;
                if (!SkinBOX.IsValidType(boxType))
                {
                    switch (bone.Name)
                    {
                        case "head":
                        case "helmet":
                            boxType = "HEAD";
                            break;
                        case "body":
                            boxType = "BODY";
                            break;
                        case "rightArm":
                            boxType = "ARM0";
                            break;
                        case "leftArm":
                            boxType = "ARM1";
                            break;
                        case "rightLeg":
                            boxType = "LEG0";
                            break;
                        case "leftLeg":
                            boxType = "LEG1";
                            break;
                        case "hat":
                            boxType = "HEADWEAR";
                            break;
                        case "jacket":
                            boxType = "JACKET";
                            break;
                        case "bodyArmor":
                            boxType = "BODY";
                            break;
                        case "rightSleeve":
                            boxType = "SLEEVE0";
                            break;
                        case "leftSleeve":
                            boxType = "SLEEVE1";
                            break;
                        case "rightPants":
                            boxType = "PANTS0";
                            break;
                        case "leftPants":
                            boxType = "PANTS1";
                            break;
                        default:
                            continue;
                    }
                }
                foreach (External.Format.Cube cube in bone.Cubes)
                {
                    Vector3 pos = TranslateToInternalPosition(boxType, cube.Origin, cube.Size, Vector3.UnitY);
                    var skinBox = new SkinBOX(boxType, pos, cube.Size, cube.Uv);
                    if (bone.Name == "helmet")
                    {
                        skinBox.HideWithArmor = true;
                    }
                    modelInfo.AdditionalBoxes.Add(skinBox);
                }
            }
            return modelInfo;
        }

        internal static void ExportBedrockJson(string fileName, SkinModelInfo modelInfo)
        {
            if (string.IsNullOrEmpty(fileName) || !fileName.EndsWith(".json"))
                return;

            Dictionary<string, Bone> bones = new Dictionary<string, Bone>(5);

            void AddElement(SkinBOX box)
            {
                if (!bones.ContainsKey(box.Type))
                {
                    Bone bone = new Bone(box.Type);
                    Vector3 translation = ModelPartSpecifics.GetPositioningInfo(box.Type).Translation;
                    Vector3 pivot = ModelPartSpecifics.GetPositioningInfo(box.Type).Pivot;

                    bone.Pivot = (translation * -Vector3.UnitX - pivot * Vector3.UnitY) + (Vector3.UnitY * 24);
                    bones.Add(box.Type, bone);
                }

                box.GetHashCode();

                Vector3 pos = TranslateFromInternalPosistion(box, Vector3.UnitY);

                bones[box.Type].Cubes.Add(new External.Format.Cube()
                {
                    Origin = pos,
                    Size = box.Size,
                    Uv = box.UV,
                    Inflate = box.Scale,
                    Mirror = box.Mirror,
                });
            }

            ANIM2BOX(modelInfo.ANIM, AddElement);

            foreach (var box in modelInfo.AdditionalBoxes)
            {
                AddElement(box);
            }

            Geometry selectedGeometry = new Geometry();
            selectedGeometry.Bones = bones.Values.ToList();
            object bedrockModel = null;
            // Bedrock Entity (Model)
            if (fileName.EndsWith(".geo.json"))
            {
                selectedGeometry.Description = new GeometryDescription()
                {
                    Identifier = $"geometry.{Application.ProductName}.{Path.GetFileNameWithoutExtension(fileName)}",
                    TextureSize = modelInfo.Texture.Size,
                };
                bedrockModel = new BedrockModel
                {
                    FormatVersion = "1.12.0",
                    Models = new List<Geometry>() { selectedGeometry }
                };
            }
            // Bedrock Legacy Model
            else if (fileName.EndsWith(".json") && modelInfo.Texture.Height == modelInfo.Texture.Width)
            {
                bedrockModel = new BedrockLegacyModel
                {
                    { $"geometry.{Application.ProductName}.{Path.GetFileNameWithoutExtension(fileName)}", selectedGeometry }
                };
            }
            else
            {
                MessageBox.Show("Can't export to Bedrock Legacy Model.", "Invalid Texture Dimensions", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (bedrockModel is not null)
            {
                string content = JsonConvert.SerializeObject(bedrockModel);
                File.WriteAllText(fileName, content);
                string texturePath = Path.Combine(Path.GetDirectoryName(fileName), Path.GetFileNameWithoutExtension(fileName)) + ".png";
                modelInfo.Texture.Save(texturePath, ImageFormat.Png);
            }
        }

        internal static void ANIM2BOX(SkinANIM anim, Action<SkinBOX> converter)
        {
            bool is32x64 = !(anim.GetFlag(SkinAnimFlag.RESOLUTION_64x64) || anim.GetFlag(SkinAnimFlag.SLIM_MODEL));
            if (!anim.GetFlag(SkinAnimFlag.HEAD_DISABLED))
                converter(new SkinBOX("HEAD", new Vector3(-4, -8, -4), new Vector3(8), Vector2.Zero));

            if (!is32x64 && !anim.GetFlag(SkinAnimFlag.HEAD_OVERLAY_DISABLED))
                converter(new SkinBOX("HEADWEAR", new Vector3(-4, -8, -4), new Vector3(8), new Vector2(32, 0)));

            if (!anim.GetFlag(SkinAnimFlag.BODY_DISABLED))
                converter(new SkinBOX("BODY", new(-4, 0, -2), new(8, 12, 4), new(16, 16)));

            if (!is32x64 && !anim.GetFlag(SkinAnimFlag.BODY_OVERLAY_DISABLED))
                converter(new SkinBOX("JACKET", new(-4, 0, -2), new(8, 12, 4), new(16, 32)));

            if (!anim.GetFlag(SkinAnimFlag.RIGHT_ARM_DISABLED))
                converter(new SkinBOX("ARM0", new(-3, -2, -2), new(4, 12, 4), new(40, 16)));

            if (!is32x64 && !anim.GetFlag(SkinAnimFlag.RIGHT_ARM_OVERLAY_DISABLED))
                converter(new SkinBOX("SLEEVE0", new(-3, -2, -2), new(4, 12, 4), new(40, 16)));

            if (!anim.GetFlag(SkinAnimFlag.LEFT_ARM_DISABLED))
                converter(new SkinBOX("ARM1", new(-1, -2, -2), new(4, 12, 4), is32x64 ? new(40, 16) : new(32, 48), mirror: is32x64));

            if (!is32x64 && !anim.GetFlag(SkinAnimFlag.LEFT_ARM_OVERLAY_DISABLED))
                converter(new SkinBOX("SLEEVE1", new(-1, -2, -2), new(4, 12, 4), new(32, 48)));

            if (!anim.GetFlag(SkinAnimFlag.RIGHT_LEG_DISABLED))
                converter(new SkinBOX("LEG0", new(-2, 0, -2), new(4, 12, 4), new(0, 16)));

            if (!is32x64 && !anim.GetFlag(SkinAnimFlag.RIGHT_LEG_OVERLAY_DISABLED))
                converter(new SkinBOX("PATNS0", new(-2, 0, -2), new(4, 12, 4), new(0, 16)));

            if (!anim.GetFlag(SkinAnimFlag.LEFT_LEG_DISABLED))
            {
                converter(new SkinBOX("LEG1", new(-2, 0, -2), new(4, 12, 4), is32x64 ? new(0, 16) : new(32, 48), mirror: is32x64));
            }

            if (!is32x64 && !anim.GetFlag(SkinAnimFlag.LEFT_LEG_OVERLAY_DISABLED))
            {
                converter(new SkinBOX("PATNS1", new(-2, 0, -2), new(4, 12, 4), new(32, 48)));
            }
        }

        internal static Vector3 TranslateToInternalPosition(string boxType, Vector3 origin, Vector3 size, Vector3 translationUnit)
        {
            Vector3 pos = TransformSpace(origin, size, translationUnit);
            // Skin Renderer (and Game) specific offset value.
            pos.Y += 24f;

            Vector3 translation = ModelPartSpecifics.GetPositioningInfo(boxType).Translation;
            Vector3 pivot = ModelPartSpecifics.GetPositioningInfo(boxType).Pivot;

            // This will cancel out the part specific translation and pivot.
            pos += translation * -Vector3.UnitX - pivot * Vector3.UnitY;

            return pos;
        }

        internal static Vector3 TranslateFromInternalPosistion(SkinBOX skinBox, Vector3 translationUnit)
        {
            return TranslateToInternalPosition(skinBox.Type, skinBox.Pos, skinBox.Size, translationUnit);
        }

        /// <summary>
        /// Translates coordinate unit system into our coordinate system
        /// </summary>
        /// <param name="origin">Position/Origin of the Object(Cube).</param>
        /// <param name="size">The Size of the Object(Cube).</param>
        /// <param name="translationUnit">Describes what axises need translation.</param>
        /// <returns>The translated position</returns>
        private static Vector3 TransformSpace(Vector3 origin, Vector3 size, Vector3 translationUnit)
        {
            // The translation unit describes what axises need to be swapped
            // Example:
            //      translation unit = (1, 0, 0) => This translation unit will ONLY swap the X axis
            translationUnit = Vector3.Clamp(translationUnit, Vector3.Zero, Vector3.One);
            // To better understand see:
            // https://sharplab.io/#v2:C4LgTgrgdgNAJiA1AHwAICYCMBYAUKgBgAJVMA6AOQgFsBTMASwGMBnAbj1QGYT0iBhIgG88RMb3SjxI3OLlEAbgEMwRBlAAOEYEQC8RKLQDuRAGq0mwAPZguACkwwijogQCUHWfLHLVtAB4aFsC0cHoGxmbBNvYAtC7xTpgeUt6+RGC0LOEAKmBKUCwAYjbU/FY2cOpKISx26lrAKV7epACcdpkszd5i7Z1ZevoBQZahPeIAvqlEM9wkmABsUZYxRHkFxaXlldW1duartmqa2m4zMr2KKhmD+ofWtmT8ADZK1Br1p8BODzFkAC16FZftEngB5QwTbxdIgAKn06E8V1hsXuYK4ZEhtGRvVQAHYiLEurixNNcJMgA
            Vector3 transformUnit = -((translationUnit * 2) - Vector3.One);

            Vector3 pos = origin;
            // The next line essentialy does uses the fomular below just on all axis.
            // x = -(pos.x + size.x)
            pos *= transformUnit;
            pos -= size * translationUnit;
            return pos;
        }
    }
}
