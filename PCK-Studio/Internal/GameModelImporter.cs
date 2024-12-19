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
        
            public sealed class ModelExportSettings
        {
        public bool CreateModelOutline { get; set; } = true;
        }

        public ModelExportSettings ExportSettings { get; } = new ModelExportSettings();
        internal static ReadOnlyDictionary<string, JsonModelMetaData> ModelMetaData { get; } = JsonConvert.DeserializeObject<ReadOnlyDictionary<string, JsonModelMetaData>>(Resources.modelMetaData);
        internal static ReadOnlyDictionary<string, DefaultModel> DefaultModels { get; } = JsonConvert.DeserializeObject<ReadOnlyDictionary<string, DefaultModel>>(Resources.defaultModels);
        
        private GameModelImporter()
        {
            // TODO: add import functionality -miku
            InternalAddProvider(new FileDialogFilter("Block bench model(*.bbmodel)", "*.bbmodel"), null, ExportBlockBenchModel);
        }

        private readonly Vector3 bbModelTransformAxis = new Vector3(1, 1, 0);
        // maybe get this value from the json. -miku
        private readonly Vector3 _heightOffset = Vector3.UnitY * 24f;

        private void ExportBlockBenchModel(string filepath, GameModelInfo modelInfo)
        {
            BlockBenchModel blockBenchModel = BlockBenchModel.Create(BlockBenchFormatInfos.BedrockEntity, modelInfo.Model.Name, modelInfo.Model.TextureSize, modelInfo.Textures.Select(nt => (Texture)nt));
            blockBenchModel.ModelIdentifier = modelInfo.Model.Name;

            List<Element> elements = new List<Element>(modelInfo.Model.PartCount);

            if (!ModelMetaData.TryGetValue(modelInfo.Model.Name, out JsonModelMetaData modelMetaData))
            {
                Trace.TraceError($"[{nameof(GameModelImporter)}:{nameof(ExportBlockBenchModel)}] Failed to get model meta data for '{modelInfo.Model.Name}'.");
                return;
            }

            IEnumerable<Outline> outlines = ConvertToOutlines(modelInfo.Model, Vector3.Zero, modelMetaData.RootParts, elements.AddRange);

            blockBenchModel.Elements = elements.ToArray();
            if (ExportSettings.CreateModelOutline)
                outlines = new Outline[1]
                {
                    new Outline(modelInfo.Model.Name) { Children = JArray.FromObject(outlines) }
                };

            blockBenchModel.Outliner = JArray.FromObject(outlines);

            string content = JsonConvert.SerializeObject(blockBenchModel, Formatting.Indented);
            File.WriteAllText(filepath, content);
        }

        private Element ToElement(string partName, ModelBox modelBox, Vector3 partTranslation)
        {
            Element element = CreateElement(partName, modelBox, partTranslation, bbModelTransformAxis, _heightOffset);
            //element.Rotation = rotation * TransformSpace(Vector3.One, Vector3.Zero, bbModelTransformAxis);
            //element.Origin = outline.Origin;
            return element;
        }

        private Outline[] ConvertToOutlines(Model model, Vector3 parentRotation, IReadOnlyCollection<ModelMetaDataPart> keyValues, Action<Element[]> addElements, int depth = 0)
        {
            Outline CreateOutline(ModelPart modelPart)
            {
                Outline outline = new Outline(modelPart.Name);

                Vector3 partTranslation = modelPart.Translation;
                outline.Origin = TransformSpace(partTranslation, Vector3.Zero, bbModelTransformAxis);
                outline.Origin += _heightOffset;

                Vector3 rotation = modelPart.Rotation;
                outline.Rotation = rotation * TransformSpace(Vector3.One, Vector3.Zero, bbModelTransformAxis);
                outline.Rotation += parentRotation;

                Element[] elements1 = modelPart.GetBoxes().Select(box => ToElement(modelPart.Name, box, partTranslation)).ToArray();
                addElements(elements1);

                outline.Children.Add(elements1.Select(element => element.Uuid).ToArray());
                return outline;
            }

            if (depth == 0 && keyValues.Count == 0)
                {
                return model.GetParts().Select(CreateOutline).ToArray();
            }

            List<Outline> outlines = new List<Outline>();
            foreach (ModelMetaDataPart item in keyValues)
            {
                if (!model.TryGetPart(item.Name, out ModelPart modelPart))
                {
                    Debug.WriteLine($"{nameof(item.Name)}: '{item.Name}' not in {nameof(model)}.");
                    continue;
                }
                Outline partentOutline = CreateOutline(modelPart);
                JToken[] s = ConvertToOutlines(model, modelPart.Rotation, item.Children, addElements, depth + 1).Select(JToken.FromObject).ToArray();
                partentOutline.Children.Add(s);
                outlines.Add(partentOutline);
            }
            return outlines.ToArray();
        }


        private static Element CreateElement(string name, ModelBox box, Vector3 origin, Vector3 translationUnit, Vector3 offset)
        {
            Vector3 pos = box.Position;
            Vector3 size = box.Size;
            Vector3 transformPos = TransformSpace(pos + origin, size, translationUnit) + offset;
            return Element.CreateCube(name, box.Uv, transformPos, size, box.Inflate, box.Mirror);
        }
    }
}
