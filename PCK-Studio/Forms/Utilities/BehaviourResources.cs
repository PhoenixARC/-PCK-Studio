using Newtonsoft.Json.Linq;
using System.Drawing;
using System.Linq;
using System.IO;

using PckStudio.Properties;
using PckStudio.Extensions;
using OMI.Formats.Behaviour;
using OMI.Workers.Behaviour;

namespace PckStudio.Forms.Utilities
{
    public static class BehaviourResources
    {
        public static readonly JObject entityData = JObject.Parse(Resources.entityData);
        private static Image[] _entityImages;
        
        public static Image[] entityImages => _entityImages ??= Resources.entities_sheet.CreateImageList(32).ToArray();

        internal static byte[] BehaviourFileInitializer()
        {
            using var stream = new MemoryStream();
            var writer = new BehavioursWriter(new BehaviourFile());
            writer.WriteToStream(stream);
            return stream.ToArray();
        }
    }
}
