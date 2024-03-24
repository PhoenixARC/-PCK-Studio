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
using System;
using System.Windows.Forms;

namespace PckStudio.IO.TGA
{
    internal struct TGAExtentionData
    {
        public const short ExtensionSize = 0x1EF;
        public string AuthorName;
        public string AuthorComment;
        public DateTime TimeStamp;
        public string JobID;
        public TimeSpan JobTime;
        public string SoftwareID;
        public byte[] SoftwareVersion;
        public int KeyColor;
        public int PixelAspectRatio;
        public int GammaValue;
        public int ColorCorrectionOffset;
        public int PostageStampOffset;
        public int ScanLineOffset;
        public byte AttributesType;

        public static TGAExtentionData Create()
        {
            var extensionData = new TGAExtentionData();
            extensionData.AuthorName = "";
            extensionData.AuthorComment = "";
            extensionData.AuthorComment = "";
            extensionData.TimeStamp = DateTime.Now;
            extensionData.JobID = "";
            extensionData.JobTime = new TimeSpan(extensionData.TimeStamp.Hour, extensionData.TimeStamp.Minute, extensionData.TimeStamp.Second);
            extensionData.SoftwareID = Application.ProductName;
            Version.TryParse(Application.ProductVersion, out Version currentVersion);
            extensionData.SoftwareVersion = [(byte)currentVersion.Major, (byte)currentVersion.Minor, (byte)currentVersion.Build];
            extensionData.KeyColor = 0;
            extensionData.PixelAspectRatio = 0;
            extensionData.GammaValue = 0;
            extensionData.ColorCorrectionOffset = 0;
            extensionData.PostageStampOffset = 0;
            extensionData.ScanLineOffset = 0;
            extensionData.AttributesType = 3;
            return extensionData;
        }
    }
}