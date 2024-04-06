using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OMI.Formats.Pck;
using PckStudio.Interfaces;
using PckStudio.IO.TGA;

namespace PckStudio.Internal.Serializer
{
    internal sealed class ImageSerializer : IPckFileSerializer<Image>
    {
        public static readonly ImageSerializer DefaultSerializer = new ImageSerializer();

        public void Serialize(Image obj, ref PckFileData file)
        {
            var stream = new MemoryStream();
            try
            {
                if (Path.GetExtension(file.Filename) == ".tga")
                    TGASerializer.SerializeToStream(stream, obj);
                else
                    obj.Save(stream, ImageFormat.Png);
                file.SetData(stream.ToArray());
            }
            catch (Exception ex)
            {
                Trace.TraceError($"Failed to serialize image to pck file data({file.Filename}).");
                Debug.WriteLine(ex.Message);
            }
        }
    }
}
