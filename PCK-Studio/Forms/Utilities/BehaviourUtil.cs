using Newtonsoft.Json.Linq;
using System.Drawing;
using System.Linq;
using System.IO;

using PckStudio.Properties;
using PckStudio.Classes.FileTypes;
using PckStudio.Classes.IO.Behaviour;
using PckStudio.Classes.Extentions;

namespace PckStudio.Forms.Utilities
{
    public static class BehaviourUtil
    {
        public static readonly JObject entityData = JObject.Parse(Resources.entityBehaviourData);
        private static Image[] _entityImages;
        
        public static Image[] entityImages => _entityImages ??= Resources.entities_sheet.CreateImageList(32).ToArray();

        public static PCKFile.FileData CreateNewBehaviourFile()
        {
            PCKFile.FileData file = new PCKFile.FileData($"behaviours.bin", PCKFile.FileData.FileType.BehavioursFile);

            using (var stream = new MemoryStream())
            {
                BehavioursWriter.Write(stream, new BehaviourFile());
                file.SetData(stream.ToArray());
            }
            
            return file;
        }
    }
}
