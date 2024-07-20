/* Copyright (c) 2024-present miku-666
 * This software is provided 'as-is', without any express or implied
 * warranty. In no event will the authors be held liable for any damages
 * arising from the use of this software.
 * 
 * Permission is granted to anyone to use this software for any purpose,
 * including commercial applications, and to alter it and redistribute it
 * freely, subject to the following restrictions:
 * 
 * 1.The origin of this software must not be misrepresented; you must not
 *    claim that you wrote the original software. If you use this software
 *    in a product, an acknowledgment in the product documentation would be
 *    appreciated but is not required.
 * 2. Altered source versions must be plainly marked as such, and must not be
 *    misrepresented as being the original software.
 * 3. This notice may not be removed or altered from any source distribution.
**/
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
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
