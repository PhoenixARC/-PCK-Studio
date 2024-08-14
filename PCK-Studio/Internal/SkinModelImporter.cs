/* Copyright (c) 2024-present miku-666
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
using System;
using System.IO;
using System.Linq;
using System.Drawing;
using System.Numerics;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using PckStudio.Extensions;
using PckStudio.Internal.Skin;
using PckStudio.Internal.IO.PSM;
using PckStudio.External.Format;
using PckStudio.Internal.FileFormats;
using PckStudio.Forms.Additional_Popups;
using System.Diagnostics;

namespace PckStudio.Internal
{
    internal sealed class SkinModelImporter : ModelImporter<SkinModelInfo>
    {
        public static SkinModelImporter Default { get; } = new SkinModelImporter();

        private SkinModelImporter()
        {
            InternalAddProvider(new("Pck skin model(*.psm)", "*.psm"), ImportPsm, ExportPsm);
            InternalAddProvider(new("Block bench model(*.bbmodel)", "*.bbmodel"), ImportBlockBenchModel, ExportBlockBenchModel);
            InternalAddProvider(new("Bedrock (Legacy) Model(*.geo.json;*.json)", "*.geo.json;*.json"), ImportBedrockJson, ExportBedrockJson);
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
            if (!blockBenchModel.Format.UseBoxUv)
            {
                Trace.TraceError($"[{nameof(SkinModelImporter)}:{nameof(ImportBlockBenchModel)}] Failed to import skin '{blockBenchModel.Name}': Skin does not use box uv.");
                return null;
            }
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
                modelInfo.Texture = blockBenchModel.Textures[0];
                modelInfo.ANIM.SetFlag(SkinAnimFlag.RESOLUTION_64x64, modelInfo.Texture.Size.Width == modelInfo.Texture.Size.Height);
            }


            foreach (JToken token in blockBenchModel.Outliner)
            {
                if (token.Type == JTokenType.String && Guid.TryParse((string)token, out Guid tokenGuid))
                {
                    Element element = blockBenchModel.Elements.First(e => e.Uuid.Equals(tokenGuid));
                    LoadElement(element.Name, element, ref modelInfo);
                    continue;
                }
                if (token.Type == JTokenType.Object)
                {
                    Outline outline = token.ToObject<Outline>();
                    string type = outline.Name;
                    ReadOutliner(token, type, blockBenchModel.Elements, ref modelInfo);
                }
            }
            modelInfo.Texture = FixTexture(modelInfo);
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
            if (element.Type != "cube" || !element.UseBoxUv || !element.IsVisibile)
                return;

            boxType = TryConvertToSkinBoxType(boxType);
            if (!SkinBOX.IsValidType(boxType))
                return;

            Vector3 offset = GetModelOffsetAndAddToModelInfo(boxType, element.Origin, ref modelInfo);

            var boundingBox = new Rendering.BoundingBox(element.From, element.To);
            Vector3 pos = boundingBox.Start - offset;
            Vector3 size = boundingBox.Volume;
            Vector2 uv = element.UvOffset;

            pos = TranslateToInternalPosition(boxType, pos, size, new Vector3(1, 1, 0));

            var box = new SkinBOX(boxType, pos, size, uv, mirror: element.MirrorUv);
            if (box.IsBasePart() && ((boxType == "HEAD" && element.Inflate == 0.5f) || (element.Inflate >= 0.25f && element.Inflate <= 0.5f)))
                box.Type = box.GetOverlayType();

            if (!BOX2ANIM(modelInfo.ANIM, box))
                modelInfo.AdditionalBoxes.Add(box);
        }

        internal static void ExportBlockBenchModel(string fileName, SkinModelInfo modelInfo)
        {
            Image exportTexture = FixTexture(modelInfo);
            BlockBenchModel blockBenchModel = BlockBenchModel.Create(BlockBenchFormatInfos.Free, Path.GetFileNameWithoutExtension(fileName), new Size(64, exportTexture.Width == exportTexture.Height ? 64 : 32), [exportTexture]);

            Dictionary<string, Outline> outliners = new Dictionary<string, Outline>(5);
            List<Element> elements = new List<Element>(modelInfo.AdditionalBoxes.Count);

            Dictionary<string, SkinPartOffset> offsetLookUp = new Dictionary<string, SkinPartOffset>(5);

            void AddElement(SkinBOX box)
            {
                Vector3 offset = GetOffset(box.Type, ref offsetLookUp, modelInfo.PartOffsets);
                if (!outliners.ContainsKey(box.Type))
                {
                    outliners.Add(box.Type, new Outline(box.Type)
                    {
                        Origin = GetSkinBoxPivot(box.Type) + offset
                    });
                }

                Element element = CreateElement(box);

                element.From += offset;
                element.To += offset;

                elements.Add(element);
                outliners[box.Type].Children.Add(element.Uuid);
            }

            ANIM2BOX(modelInfo.ANIM, AddElement);

            foreach (SkinBOX box in modelInfo.AdditionalBoxes)
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
            Vector3 transformPos = TranslateFromInternalPosistion(box, new Vector3(1, 1, 0));
            Element element = CreateElement(box.UV, transformPos, box.Size, box.Scale, box.Mirror);
            if (box.IsOverlayPart())
                element.Inflate = box.Type == "HEADWEAR" ? 0.5f : 0.25f;
            return element;
        }

        private static Element CreateElement(Vector2 uvOffset, Vector3 pos, Vector3 size, float inflate, bool mirror)
        {
            return Element.CreateCube("cube", uvOffset, pos, size, inflate, mirror);
        }

        internal static SkinModelInfo ImportBedrockJson(string fileName)
        {
            Geometry selectedGeometry = null;
            // Bedrock Entity (Model)
            if (fileName.EndsWith(".geo.json"))
            {
                BedrockModel bedrockModel = JsonConvert.DeserializeObject<BedrockModel>(File.ReadAllText(fileName));
                var availableModels = bedrockModel.Models.Select(m => m.Description.Identifier).ToArray();
                if (availableModels.Length > 1)
                {
                    ItemSelectionPopUp itemSelectionPopUp = new ItemSelectionPopUp(availableModels);
                    if (itemSelectionPopUp.ShowDialog() == DialogResult.OK && bedrockModel.Models.IndexInRange(itemSelectionPopUp.SelectedIndex))
                    {
                        selectedGeometry = bedrockModel.Models[itemSelectionPopUp.SelectedIndex];
                    }
                    itemSelectionPopUp.Dispose();
                }
                else
                {
                    selectedGeometry = bedrockModel.Models[0];
                }
            }

            // Bedrock Legacy Model
            else if (fileName.EndsWith(".json"))
            {
                BedrockLegacyModel bedrockModel = JsonConvert.DeserializeObject<BedrockLegacyModel>(File.ReadAllText(fileName));
                var availableModels = bedrockModel.Select(m => m.Key).ToArray();
                if (availableModels.Length > 1)
                {
                    ItemSelectionPopUp itemSelectionPopUp = new ItemSelectionPopUp(availableModels);
                    if (itemSelectionPopUp.ShowDialog() == DialogResult.OK && itemSelectionPopUp.SelectedItem is not null)
                    {
                        selectedGeometry = bedrockModel[itemSelectionPopUp.SelectedItem];
                    }
                    itemSelectionPopUp.Dispose();
                }
                else
                {
                    selectedGeometry = bedrockModel[availableModels[0]];
                }
            }

            SkinModelInfo modelInfo = null;
            if (selectedGeometry is not null)
            {
                modelInfo = LoadGeometry(selectedGeometry);
            }
            modelInfo.Texture = FixTexture(modelInfo);
            return modelInfo;
        }

        private static SkinModelInfo LoadGeometry(Geometry geometry)
        {
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
            if (geometry.Description?.TextureSize.Width == geometry.Description?.TextureSize.Height)
                modelInfo.ANIM.SetFlag(SkinAnimFlag.RESOLUTION_64x64, true);

            foreach (Bone bone in geometry.Bones)
            {
                string boxType = TryConvertToSkinBoxType(bone.Name);
                if (!SkinBOX.IsValidType(boxType))
                    continue;

                Vector3 offset = GetModelOffsetAndAddToModelInfo(boxType, bone.Pivot, ref modelInfo);

                foreach (External.Format.Cube cube in bone.Cubes)
                {
                    Vector3 pos = TranslateToInternalPosition(boxType, cube.Origin - offset, cube.Size, Vector3.UnitY);
                    var skinBox = new SkinBOX(boxType, pos, cube.Size, cube.Uv, mirror: cube.Mirror);
                    if (bone.Name == "helmet")
                    {
                        skinBox.HideWithArmor = true;
                    }
                    if (!BOX2ANIM(modelInfo.ANIM, skinBox))
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
            Dictionary<string, SkinPartOffset> offsetLookUp = new Dictionary<string, SkinPartOffset>(5);

            void AddElement(SkinBOX box)
            {
                Vector3 offset = GetOffset(box.Type, ref offsetLookUp, modelInfo.PartOffsets);

                if (!bones.ContainsKey(box.Type))
                {
                    Bone bone = new Bone(box.Type)
                    {
                        Pivot = GetSkinBoxPivot(box.Type) + offset
                    };
                    bones.Add(box.Type, bone);
                }

                Vector3 pos = TranslateFromInternalPosistion(box, new Vector3(1,1,0));

                bones[box.Type].Cubes.Add(new External.Format.Cube()
                {
                    Origin = pos + offset,
                    Size = box.Size,
                    Uv = box.UV,
                    Inflate = box.Scale,
                    Mirror = box.Mirror,
                });
            }

            ANIM2BOX(modelInfo.ANIM, AddElement);

            foreach (SkinBOX box in modelInfo.AdditionalBoxes)
            {
                AddElement(box);
            }

            Geometry selectedGeometry = new Geometry();
            selectedGeometry.Bones.AddRange(bones.Values);
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
                    Models = { selectedGeometry }
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
                FixTexture(modelInfo).Save(texturePath, ImageFormat.Png);
            }
        }

        private static void ANIM2BOX(SkinANIM anim, Action<SkinBOX> converter)
        {
            bool isSlim = anim.GetFlag(SkinAnimFlag.SLIM_MODEL);
            bool is32x64 = !(anim.GetFlag(SkinAnimFlag.RESOLUTION_64x64) || isSlim);
            if (!anim.GetFlag(SkinAnimFlag.HEAD_DISABLED))
                converter(new SkinBOX("HEAD", new Vector3(-4, -8, -4), new Vector3(8), Vector2.Zero));

            if (!is32x64 && !anim.GetFlag(SkinAnimFlag.HEAD_OVERLAY_DISABLED))
                converter(new SkinBOX("HEADWEAR", new Vector3(-4, -8, -4), new Vector3(8), new Vector2(32, 0)));

            if (!anim.GetFlag(SkinAnimFlag.BODY_DISABLED))
                converter(new SkinBOX("BODY", new(-4, 0, -2), new(8, 12, 4), new(16, 16)));

            if (!is32x64 && !anim.GetFlag(SkinAnimFlag.BODY_OVERLAY_DISABLED))
                converter(new SkinBOX("JACKET", new(-4, 0, -2), new(8, 12, 4), new(16, 32)));

            if (!anim.GetFlag(SkinAnimFlag.RIGHT_ARM_DISABLED))
                converter(new SkinBOX("ARM0", new(isSlim ? -2 : - 3, -2, -2), new(isSlim ? 3 : 4, 12, 4), new(40, 16)));

            if (!is32x64 && !anim.GetFlag(SkinAnimFlag.RIGHT_ARM_OVERLAY_DISABLED))
                converter(new SkinBOX("SLEEVE0", new(isSlim ? -2 : - 3, -2, -2), new(isSlim ? 3 : 4, 12, 4), new(40, 32)));

            if (!anim.GetFlag(SkinAnimFlag.LEFT_ARM_DISABLED))
                converter(new SkinBOX("ARM1", new(-1, -2, -2), new(isSlim ? 3 : 4, 12, 4), is32x64 ? new(40, 16) : new(32, 48), mirror: is32x64));

            if (!is32x64 && !anim.GetFlag(SkinAnimFlag.LEFT_ARM_OVERLAY_DISABLED))
                converter(new SkinBOX("SLEEVE1", new(-1, -2, -2), new(isSlim ? 3 : 4, 12, 4), new(48, 48)));

            if (!anim.GetFlag(SkinAnimFlag.RIGHT_LEG_DISABLED))
                converter(new SkinBOX("LEG0", new(-2, 0, -2), new(4, 12, 4), new(0, 16)));

            if (!is32x64 && !anim.GetFlag(SkinAnimFlag.RIGHT_LEG_OVERLAY_DISABLED))
                converter(new SkinBOX("PANTS0", new(-2, 0, -2), new(4, 12, 4), new(0, 32)));

            if (!anim.GetFlag(SkinAnimFlag.LEFT_LEG_DISABLED))
            {
                converter(new SkinBOX("LEG1", new(-2, 0, -2), new(4, 12, 4), is32x64 ? new(0, 16) : new(16, 48), mirror: is32x64));
            }

            if (!is32x64 && !anim.GetFlag(SkinAnimFlag.LEFT_LEG_OVERLAY_DISABLED))
            {
                converter(new SkinBOX("PANTS1", new(-2, 0, -2), new(4, 12, 4), new(0, 48)));
            }
        }

        private static bool BOX2ANIM(SkinANIM anim, SkinBOX box)
        {
            int hash = box.GetHashCode();
            if (SkinBOX.KnownHashes.ContainsKey(hash))
            {
                anim.SetFlag(SkinBOX.KnownHashes[hash], false);
                return true;
            }
            return false;
        }

        private static string TryConvertToSkinBoxType(string name)
        {
            if (!SkinBOX.IsValidType(name) && SkinBOX.IsValidType(name.ToUpper()))
            {
                return name.ToUpper();
            }
            return name switch
            {
                "helmet"      => "HEAD",
                "rightArm"    => "ARM0",
                "leftArm"     => "ARM1",
                "rightLeg"    => "LEG0",
                "leftLeg"     => "LEG1",
                "hat"         => "HEADWEAR",
                "bodyArmor"   => "BODY",
                "rightSleeve" => "SLEEVE0",
                "leftSleeve"  => "SLEEVE1",
                "rightPants"  => "PANTS0",
                "leftPants"   => "PANTS1",
                _             => name,
            };
        }

        private static Vector3 GetModelOffsetAndAddToModelInfo(string boxType, Vector3 origin, ref SkinModelInfo modelInfo)
        {
            Vector3 partTranslation = ModelPartSpecifics.GetPositioningInfo(boxType).Translation;
            Vector3 offset = partTranslation - ((Vector3.UnitY * 24f) - origin);
            if (offset.X != 0f || offset.Z != 0f)
                Trace.TraceWarning($"[{nameof(SkinModelImporter)}:{nameof(LoadElement)}] Warning: skin part({boxType}) offsets only support horizontal offsets.");

            if (offset.Y != 0f)
                modelInfo.PartOffsets.Add(new SkinPartOffset(boxType, -offset.Y));
            return offset * Vector3.UnitY;
        }

        private static Image FixTexture(SkinModelInfo modelInfo)
        {
            return FixTexture(modelInfo.Texture, modelInfo.AdditionalBoxes.Where(box => !(box.Size == Vector3.One || box.Size == Vector3.Zero)).Select(box =>
            {
                var imgPos = Point.Truncate(new PointF(box.UV.X + box.Size.X + box.Size.Z, box.UV.Y));
                var area = new RectangleF(imgPos, Size.Truncate(new SizeF(box.Size.X, box.Size.Z)));
                return Rectangle.Truncate(area);
            }));
        }

        private static Image FixTexture(Image texture, IEnumerable<Rectangle> areasToFix)
        {
            if (texture == null)
            {
                Trace.TraceError($"[{nameof(SkinModelImporter)}:{nameof(FixTexture)}] Failed to fix texture: texture is null.");
                return null;
            }
            Image result = new Bitmap(texture);
            using var g = Graphics.FromImage(result);
            g.ApplyConfig(new GraphicsConfig()
            {
                InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor,
                PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality
            });
            foreach (Rectangle area in areasToFix)
            {
                Image targetAreaImage = texture.GetArea(area);
                targetAreaImage.RotateFlip(RotateFlipType.RotateNoneFlipY);
                Region clip = g.Clip;
                g.SetClip(area);
                g.Clear(Color.Transparent);
                g.DrawImage(targetAreaImage, area.Location);
                g.Clip = clip;
            }
            return result;
        }

        private static Vector3 GetOffset(string name, ref Dictionary<string, SkinPartOffset> offsetLookUp, IEnumerable<SkinPartOffset> partOffsets)
        {
            if (offsetLookUp.ContainsKey(name))
            {
                return -offsetLookUp[name].Value * Vector3.UnitY;
            }
            if (partOffsets.Any(o => o.Type == name))
            {
                SkinPartOffset partOffset = partOffsets.First(o => o.Type == name);
                offsetLookUp.Add(name, partOffset);
                return -partOffset.Value * Vector3.UnitY;
            }
            return Vector3.Zero;
        }

        private static Vector3 GetSkinBoxPivot(string partName)
        {
            return TransformSpace(ModelPartSpecifics.GetPositioningInfo(partName).Pivot, Vector3.Zero, new Vector3(1, 1, 0)) + (24f * Vector3.UnitY);
        }

        private static Vector3 TranslateToInternalPosition(string boxType, Vector3 origin, Vector3 size, Vector3 translationUnit)
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

        private static Vector3 TranslateFromInternalPosistion(SkinBOX skinBox, Vector3 translationUnit)
        {
            return TranslateToInternalPosition(skinBox.Type, skinBox.Pos, skinBox.Size, translationUnit);
        }
    }
}
