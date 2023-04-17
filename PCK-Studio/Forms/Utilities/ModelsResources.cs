using Newtonsoft.Json.Linq;
using System.Drawing;
using System.Linq;
using System.IO;

using PckStudio.Properties;
using PckStudio.Extensions;
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

        public static byte[] ModelsFileInitializer()
        {
            using var stream = new MemoryStream();
            var writer = new ModelFileWriter(new ModelContainer());
            writer.WriteToStream(stream);
            return stream.ToArray();
        }
    }
}
