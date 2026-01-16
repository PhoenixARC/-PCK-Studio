using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OMI.Formats.Model;
using PckStudio.Core.Extensions;
using PckStudio.Core.Json;
using PckStudio.Core.Properties;
using PckStudio.Interfaces;

namespace PckStudio.Core.Model
{
    public class AbstractModelContainer
    {
        static Dictionary<string, JsonModelMetaData> _metaData = JsonConvert.DeserializeObject<Dictionary<string, JsonModelMetaData>>(Resources.entityModelMetaData);

        private IDictionary<string, AbstractModel> _models = new Dictionary<string, AbstractModel>();

        public AbstractModel GetModelByName(string name) => _models[name];
        
        public bool AddModel(AbstractModel model)
        {
            if (model == null || _models.ContainsKey(model.Name))
                return false;
            _models.Add(model.Name, model);
            return true;
        }

        public bool RemoveModel(AbstractModel model) => model is not null && _models.Remove(model.Name);

        public static AbstractModelContainer FromModelContainer(ModelContainer models, ITryGet<string, Image> texture)
        {
            var abstractModelContainer = new AbstractModelContainer();
            if (models is null)
                return abstractModelContainer;
            foreach (OMI.Formats.Model.Model model in models.GetModels().Where(m => _metaData.ContainsKey(m.Name) && m.Name.EqualsAny(_metaData[m.Name].RootParts.Select(mdp => mdp.Name).ToArray())))
            {
                if (!_metaData.TryGetValue(model.Name, out JsonModelMetaData modelMetaData))
                {
                    Trace.TraceWarning($"No model meta data found for: '{model.Name}'.");
                    continue;
                }

                IDictionary<string, ModelPart> parts = model.GetParts().ToDictionary(part => part.Name);

                IReadOnlyDictionary<string, Image> textures = modelMetaData.TextureLocations.Where(s => texture.TryGet(s, out _)).ToDictionary(Path.GetFileNameWithoutExtension, s =>
                {
                    texture.TryGet(s, out Image img);
                    return img;
                });
                AbstractModel abstractModel = new AbstractModel(model.Name, model.TextureSize, textures.FirstOrDefault(), textures);
                abstractModel.AddParts(GetRootAbstractModelPart(modelMetaData.RootParts, parts, null));
                abstractModelContainer.AddModel(abstractModel);
            }
            return abstractModelContainer;
        }

        private static IEnumerable<AbstractModelPart> GetRootAbstractModelPart(ModelMetaDataPart[] dataParts, IDictionary<string, ModelPart> parts, AbstractModelPart parent)
        {
            foreach (ModelMetaDataPart rootPart in dataParts)
            {
                if (!parts.TryGetValue(rootPart.Name, out ModelPart part))
                    continue;
                var abstractModelPart = new AbstractModelPart(part.Name, parent, part.Translation, part.Rotation + part.AdditionalRotation, part.GetBoxes().Select(mb => new Box(mb.Position, mb.Size, mb.Uv, mb.Inflate, mb.Mirror)));
                abstractModelPart.AddParts(GetRootAbstractModelPart(rootPart.Children, parts, abstractModelPart));
                yield return abstractModelPart;
            }
        }
    }
}