using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using OMI.Formats.Pck;
using PckStudio.Interfaces;
using PckStudio.Core.IO.TGA;

namespace PckStudio.Core.Deserializer
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

            Image img = Path.GetExtension(asset.Filename) == ".tga"
                ? TGADeserializer.DeserializeFromStream(stream)
                : Image.FromStream(stream);
            return img.RawFormat != ImageFormat.Jpeg || img.RawFormat != ImageFormat.Png ? new Bitmap(img) : img;
        }
    }
}
