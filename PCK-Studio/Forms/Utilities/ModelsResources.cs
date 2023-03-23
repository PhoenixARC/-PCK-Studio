using Newtonsoft.Json.Linq;
using System.Drawing;
using System.Linq;
using System.IO;

using PckStudio.Properties;
using PckStudio.Classes.Extentions;
using OMI.Formats.Model;
using OMI.Formats.Pck;
using OMI.Workers.Model;

namespace PckStudio.Forms.Utilities
{
    public static class ModelsResources
    {
        public static readonly JObject entityData = JObject.Parse(Resources.entityModelData);
        private static Image[] _entityImages;
        
        public static Image[] entityImages => _entityImages ??= Resources.entities_sheet.CreateImageList(32).ToArray();

        public static void ModelsFileInitializer(PckFile.FileData file)
        {
            using (var stream = new MemoryStream())
            {
                var writer = new ModelFileWriter(new ModelContainer());
                writer.WriteToStream(stream);
                file.SetData(stream.ToArray());
            }
        }

        public static PckFile.FileData CreateNewModelsFile()
        {
            PckFile.FileData file = new PckFile.FileData("models.bin", PckFile.FileData.FileType.ModelsFile);
            ModelsFileInitializer(file);
            return file;
        }
    }
}
