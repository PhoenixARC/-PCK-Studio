using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.Linq;

using PckStudio.Properties;
using PckStudio.Classes.FileTypes;
using PckStudio.Classes.IO.Behaviour;
using PckStudio.Classes.Utils;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO;

namespace PckStudio.Forms.Utilities
{
    public static class BehaviourUtil
    {
        public static readonly JObject entityData = JObject.Parse(Resources.entityBehaviourData);
        private static Image[] _entityImages;
        public static Image[] entityImages
        {
            get { 
                if (_entityImages == null)
                    _entityImages = ImageUtils.CreateImageList(Resources.entities_sheet, 32).ToArray();
                return _entityImages;
            }
        }
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
