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

namespace PckStudio.Internal.Deserializer
{
    internal sealed class ImageDeserializer : IPckAssetDeserializer<Image>
    {
        public static readonly ImageDeserializer DefaultDeserializer = new ImageDeserializer();
        // TODO: replace empty image with image displaying something went wrong
        private static Image EmptyImage = new Bitmap(1, 1, PixelFormat.Format32bppArgb);

        public Image Deserialize(PckAsset asset)
        {
            _ = asset ?? throw new ArgumentNullException(nameof(asset));
            if (asset.Size == 0)
                return EmptyImage;

            using var stream = new MemoryStream(asset.Data);
            try
            {
                if (Path.GetExtension(asset.Filename) == ".tga")
                    return TGADeserializer.DeserializeFromStream(stream);
                else
                    return Image.FromStream(stream);
            }
            catch (Exception ex)
            {
                Trace.TraceError($"Failed to read image from pck file data({asset.Filename}).");
                Debug.WriteLine(ex.Message);
                return EmptyImage;
            }
        }
    }
}
