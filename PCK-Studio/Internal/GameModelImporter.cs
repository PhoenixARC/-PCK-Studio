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
using System.Diagnostics;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using OMI.Formats.Model;

using PckStudio.External.Format;
using PckStudio.Internal.Json;
using PckStudio.Properties;

namespace PckStudio.Internal
{
    internal sealed class GameModelImporter : ModelImporter<GameModelInfo>
    {
        public static GameModelImporter Default { get; } = new GameModelImporter();
        
        public class ModelExportSettings
        {
        public bool CreateModelOutline { get; set; } = true;
        }

        public ModelExportSettings ExportSettings { get; } = new ModelExportSettings();
        internal static ReadOnlyDictionary<string, JsonModelMetaData> ModelMetaData { get; } = JsonConvert.DeserializeObject<ReadOnlyDictionary<string, JsonModelMetaData>>(Resources.modelMetaData);
        
        private GameModelImporter()
        {
            // TODO: add import functionality -miku
            InternalAddProvider(new FileDialogFilter("Block bench model(*.bbmodel)", "*.bbmodel"), null, ExportBlockBenchModel);
        }

        Vector3 bbModelTransformAxis = new Vector3(1, 1, 0);
        Vector3 _heightOffset = Vector3.Zero;

        private void ExportBlockBenchModel(string filepath, GameModelInfo modelInfo)
        {
            BlockBenchModel blockBenchModel = BlockBenchModel.Create(BlockBenchFormatInfos.BedrockEntity, modelInfo.Model.Name, modelInfo.Model.TextureSize, modelInfo.Textures.Select(nt => (Texture)nt));
            blockBenchModel.ModelIdentifier = modelInfo.Model.Name;

            Dictionary<string, Outline> outliners = new Dictionary<string, Outline>(5);
            List<Element> elements = new List<Element>(modelInfo.Model.PartCount);

            if (!ModelMetaData.TryGetValue(modelInfo.Model.Name, out JsonModelMetaData modelMetaData))
            {
                Trace.TraceError($"[{nameof(GameModelImporter)}:{nameof(ExportBlockBenchModel)}] Failed to get model meta data for '{modelInfo.Model.Name}'.");
                return;
            }

            _heightOffset = Vector3.UnitY * 24f;

            foreach (ModelPart part in modelInfo.Model.GetParts())
            {
                var outline = new Outline(part.Name);

                Vector3 partTranslation = part.Translation;
                outline.Origin = TransformSpace(partTranslation, Vector3.Zero, bbModelTransformAxis);
                outline.Origin += _heightOffset;

                Vector3 rotation = part.Rotation;
                outline.Rotation = rotation * TransformSpace(Vector3.One, Vector3.Zero, bbModelTransformAxis);

                foreach (ModelBox box in part.GetBoxes())
                {
                    Element element = CreateElement(part.Name, box, partTranslation);
                    element.Rotation = rotation * TransformSpace(Vector3.One, Vector3.Zero, bbModelTransformAxis);
                    element.Origin = outline.Origin;
                    elements.Add(element);
                    outline.Children.Add(element.Uuid);
                }
                outliners.Add(part.Name, outline);
            }
            
            TraverseChildren(modelMetaData.RootParts, ref outliners);

            blockBenchModel.Elements = elements.ToArray();
            IEnumerable<Outline> outlines = outliners.Values.Where(value => modelMetaData.RootParts.Count == 0 || modelMetaData.RootParts.ContainsKey(value.Name));
            if (ExportSettings.CreateModelOutline)
                outlines = new Outline[1]
                {
                    new Outline(modelInfo.Model.Name) { Children = JArray.FromObject(outlines) }
                };

            blockBenchModel.Outliner = JArray.FromObject(outlines);

            string content = JsonConvert.SerializeObject(blockBenchModel, Formatting.Indented);
            File.WriteAllText(filepath, content);
        }

        private static void TraverseChildren(IReadOnlyDictionary<string, JArray> keyValues, ref Dictionary<string, Outline> outliners)
        {
            foreach (KeyValuePair<string, JArray> item in keyValues)
            {
                if (!outliners.ContainsKey(item.Key))
                {
                    Debug.WriteLine($"{nameof(item.Key)}: '{item.Key}' not in {nameof(outliners)}.");
                    continue;
                }
                Outline partentOutline = outliners[item.Key];
                foreach (JToken child in item.Value)
                {
                    if (child.Type == JTokenType.String && outliners.TryGetValue(child.ToString(), out Outline childOutline))
                    {
                        childOutline.Rotation -= partentOutline.Rotation;
                        partentOutline.Children.Add(JToken.FromObject(childOutline));
                    }
                    if (child.Type == JTokenType.Object)
                    {
                        IReadOnlyDictionary<string, JArray> childKeyValues = child.ToObject<ReadOnlyDictionary<string, JArray>>();
                        TraverseChildren(childKeyValues, ref outliners);
                        foreach (var key in childKeyValues.Keys)
                        {
                            if (!outliners.ContainsKey(key))
                            {
                                Debug.WriteLine($"{nameof(key)}: '{key}' not in {nameof(outliners)}.");
                                continue;
                            }
                            childOutline = outliners[key];
                            childOutline.Rotation -= partentOutline.Rotation;
                            partentOutline.Children.Add(JToken.FromObject(childOutline));
                        }
                    }
                }
            }
        }

        private Element CreateElement(string name, ModelBox box, Vector3 origin)
        {
            Vector3 pos = box.Position;
            Vector3 size = box.Size;
            Vector3 transformPos = TransformSpace(pos + origin, size, bbModelTransformAxis) + _heightOffset;
            return Element.CreateCube(name, box.Uv, transformPos, size, box.Inflate, box.Mirror);
        }
    }
}
