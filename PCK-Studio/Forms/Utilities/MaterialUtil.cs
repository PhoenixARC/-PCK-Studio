﻿using Newtonsoft.Json.Linq;
using System.Drawing;
using System.Linq;
using System.IO;

using PckStudio.Properties;
using PckStudio.Classes.FileTypes;
using PckStudio.Classes.IO.Materials;
using PckStudio.Classes.Extentions;

namespace PckStudio.Forms.Utilities
{
    public static class MaterialUtil
    {
        public static readonly JObject entityData = JObject.Parse(Resources.entityMaterialData);
        private static Image[] _entityImages;
        public static Image[] entityImages => _entityImages ??= Resources.entities_sheet.CreateImageList(32).ToArray();

        public static PCKFile.FileData CreateNewMaterialsFile()
        {
            PCKFile.FileData file = new PCKFile.FileData($"entityMaterials.bin", PCKFile.FileData.FileType.MaterialFile);

            using (var stream = new MemoryStream())
            {
                var matFile = new MaterialsFile();
				matFile.entries.Add(new MaterialsFile.MaterialEntry("bat", "entity_alphatest"));
				MaterialsWriter.Write(stream, matFile);
                file.SetData(stream.ToArray());
            }
            
            return file;
        }
    }
}
