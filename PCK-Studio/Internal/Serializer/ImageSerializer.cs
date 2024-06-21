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
using PckStudio.Internal.IO.TGA;

namespace PckStudio.Internal.Serializer
{
    internal sealed class ImageSerializer : IPckAssetSerializer<Image>
    {
        public static readonly ImageSerializer DefaultSerializer = new ImageSerializer();

        public void Serialize(Image obj, ref PckAsset asset)
        {
            var stream = new MemoryStream();
            try
            {
                if (Path.GetExtension(asset.Filename) == ".tga")
                    TGASerializer.SerializeToStream(stream, obj);
                else
                    obj.Save(stream, ImageFormat.Png);
                asset.SetData(stream.ToArray());
            }
            catch (Exception ex)
            {
                Trace.TraceError($"Failed to serialize image to pck file data({asset.Filename}).");
                Debug.WriteLine(ex.Message);
            }
        }
    }
}
