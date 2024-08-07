using System;
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

namespace PckStudio.Internal
{
    internal sealed class GameModelImporter : ModelImporter<GameModelInfo>
    {
        public static GameModelImporter Default { get; } = new GameModelImporter();

        private GameModelImporter()
        {
            InternalAddProvider(new FileDialogFilter("", "*.bbmodel"), null, ExportBlockBenchModel);
        }

        internal static void ExportBlockBenchModel(string fileName, GameModelInfo modelInfo)
        {
            BlockBenchModel blockBenchModel = BlockBenchModel.Create(Path.GetFileNameWithoutExtension(fileName), modelInfo.Model.TextureSize, modelInfo.Textures.Select(nt => (Texture)nt));

            Dictionary<string, Outline> outliners = new Dictionary<string, Outline>(5);
            List<Element> elements = new List<Element>(modelInfo.Model.Parts.Count);

            Vector3 transformAxis = new Vector3(1, 1, 0);

            Outline GetOrCreateOutline(string partName)
            {
                if (!outliners.ContainsKey(partName))
                    outliners.Add(partName, new Outline(partName));
                return outliners[partName];
            }

            foreach (ModelPart part in modelInfo.Model.Parts.Values)
            {
                //Outline outline = GetOrCreateOutline(part.Name);

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

            blockBenchModel.Elements = elements.ToArray();
            blockBenchModel.Outliner = JArray.FromObject(outliners);

            string content = JsonConvert.SerializeObject(blockBenchModel);
            File.WriteAllText(fileName, content);
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
