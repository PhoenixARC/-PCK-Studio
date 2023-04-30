/* Copyright (c) 2022-present miku-666
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

namespace PckStudio.IO.TGA
{
    public enum TGADataTypeCode : byte
    {
        /// <summary>
        /// No image data included.
        /// </summary>
        NO_DATA = 0,
        /// <summary>
        /// Uncompressed, color-mapped images.
        /// </summary>
        COLORMAPPED = 1,
        /// <summary>
        /// Uncompressed, RGB images.
        /// </summary>
        RGB = 2,
        /// <summary>
        /// Uncompressed, black and white images.
        /// </summary>
        BLACK_WHITE = 3,
        /// <summary>
        /// Runlength encoded color-mapped images.
        /// </summary>
        RLE_COLORMAPPED = 9,
        /// <summary>
        /// Runlength encoded RGB images.
        /// </summary>
        RLE_RGB = 10,
        /// <summary>
        /// Compressed, black and white images.
        /// </summary>
        COMPRESSED_BLACK_WHITE = 11,
        /// <summary>
        /// Compressed color-mapped data, using Huffman, Delta, and runlength encoding.
        /// </summary>
        COMPRESSED_RLE_COLORMAPPED = 32,
        /// <summary>
        /// Compressed color-mapped data, using Huffman, Delta, and runlength encoding. 4-pass quadtree-type process.
        /// </summary>
        COMPRESSED_RLE_COLORMAPPED_4 = 33,
    }
}