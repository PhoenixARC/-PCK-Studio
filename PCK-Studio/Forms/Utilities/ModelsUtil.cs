using Newtonsoft.Json.Linq;
using System.Drawing;
using System.Linq;
using System.IO;

using PckStudio.Properties;
using PckStudio.Classes.FileTypes;
using PckStudio.Classes.IO.Model;
using PckStudio.Classes.Extentions;

namespace PckStudio.Forms.Utilities
{
    public static class ModelsUtil
    {
        public static readonly JObject entityData = JObject.Parse(Resources.entityModelData);
        private static Image[] _entityImages;
        
        public static Image[] entityImages => _entityImages ??= Resources.entities_sheet.CreateImageList(32).ToArray();

        public static PCKFile.FileData CreateNewModelsFile()
        {
            PCKFile.FileData file = new PCKFile.FileData($"models.bin", PCKFile.FileData.FileType.ModelsFile);

            using (var stream = new MemoryStream())
            {
                ModelFileWriter.Write(stream, new ModelFile());
                file.SetData(stream.ToArray());
            }
            
            return file;
        }
    }
}
