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
        internal static byte[] BehaviourFileInitializer()
        {
            using var stream = new MemoryStream();
            var writer = new BehavioursWriter(new BehaviourFile());
            writer.WriteToStream(stream);
            return stream.ToArray();
        }
    }
}
