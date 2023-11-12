using System.IO;

using OMI.Formats.Behaviour;
using OMI.Workers.Behaviour;

namespace PckStudio.Helper
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
