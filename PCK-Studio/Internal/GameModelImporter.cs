﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using OMI.Formats.Model;
using PckStudio.External.Format;
using System.Numerics;
using System.IO;
using PckStudio.Internal.Json;
using System.Collections.ObjectModel;
using PckStudio.Properties;
using System.Diagnostics;

namespace PckStudio.Internal
{
    internal sealed class GameModelImporter : ModelImporter<GameModelInfo>
    {
        public static GameModelImporter Default { get; } = new GameModelImporter();
        
        internal static ReadOnlyDictionary<string, JsonModelMetaData> ModelTextureLocations { get; } = JsonConvert.DeserializeObject<ReadOnlyDictionary<string, JsonModelMetaData>>(Resources.modelMetaData);
        
        private GameModelImporter()
        {
            // TODO: add import functionality -miku
            InternalAddProvider(new FileDialogFilter("Block bench model(*.bbmodel)", "*.bbmodel"), null, ExportBlockBenchModel);
        }

        private static void ExportBlockBenchModel(string fileName, GameModelInfo modelInfo)
        {
            BlockBenchModel blockBenchModel = BlockBenchModel.Create(modelInfo.Model.Name, modelInfo.Model.TextureSize, modelInfo.Textures.Select(nt => (Texture)nt));

            Dictionary<string, Outline> outliners = new Dictionary<string, Outline>(5);
            List<Element> elements = new List<Element>(modelInfo.Model.Parts.Count);

            Vector3 transformAxis = new Vector3(1, 1, 0);

            foreach (ModelPart part in modelInfo.Model.Parts.Values)
            {
                var outline = new Outline(part.Name);

                Vector3 partTranslation = part.Translation;
                outline.Origin = TransformSpace(partTranslation, Vector3.Zero, transformAxis);
                outline.Origin += Vector3.UnitY * 24f;

                Vector3 rotation = part.Rotation + part.AdditionalRotation;
                outline.Rotation = rotation * TransformSpace(Vector3.One, Vector3.Zero, transformAxis);

                foreach (ModelBox box in part.Boxes)
                {
                    Element element = CreateElement(box, partTranslation, part.Name);
                    element.Origin = outline.Origin;
                    elements.Add(element);
                    outline.Children.Add(element.Uuid);
                }
                outliners.Add(part.Name, outline);
            }
            
            if (!ModelTextureLocations.TryGetValue(modelInfo.Model.Name, out JsonModelMetaData modelMetaData))
            {
                Trace.TraceError($"[{nameof(GameModelImporter)}:{nameof(ExportBlockBenchModel)}] Failed to get model meta data for '{modelInfo.Model.Name}'.");
                return;
            }

            TraverseChildren(modelMetaData.RootParts, ref outliners);

            blockBenchModel.Elements = elements.ToArray();
            blockBenchModel.Outliner = JArray.FromObject(outliners.Values.Where(value => modelMetaData.RootParts.Count == 0 || modelMetaData.RootParts.ContainsKey(value.Name)));

            string content = JsonConvert.SerializeObject(blockBenchModel, Formatting.Indented);
            File.WriteAllText(fileName, content);
        }

        private static void TraverseChildren(IReadOnlyDictionary<string, JArray> keyValues, ref Dictionary<string, Outline> outliners)
        {
            foreach (KeyValuePair<string, JArray> item in keyValues)
            {
                if (!outliners.ContainsKey(item.Key))
                {
                    Debug.WriteLine($"{item.Key} not in {nameof(outliners)}.");
                    continue;
                }
                Outline partentOutline = outliners[item.Key];
                foreach (JToken child in item.Value)
                {
                    if (child.Type == JTokenType.String && outliners.TryGetValue(child.ToString(), out Outline childOutline))
                    {
                        childOutline.Rotation += -partentOutline.Rotation;
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
                                Debug.WriteLine($"{key} not in {nameof(outliners)}.");
                                continue;
                            }
                            childOutline = outliners[key];
                            childOutline.Rotation += -partentOutline.Rotation;
                            partentOutline.Children.Add(JToken.FromObject(childOutline));
                        }
                    }
                }
            }
        }

        private static Element CreateElement(ModelBox box, Vector3 origin, string name)
        {
            Vector3 pos = box.Position;
            Vector3 size = box.Size;
            Vector3 transformPos = TransformSpace(pos + origin, size, new Vector3(1, 1, 0)) + 24f * Vector3.UnitY;
            return Element.CreateCube(name, box.Uv, transformPos, size, box.Scale, box.Mirror);
        }
    }
}