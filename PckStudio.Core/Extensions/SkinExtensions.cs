using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OMI.Formats.Languages;
using OMI.Formats.Pck;
using PckStudio.Core.Skin;

namespace PckStudio.Core.Extensions
{
    public static class SkinExtensions
    {
        public static PckAsset CreateFile(this Skin.Skin skin, LOCFile localizationFile)
        {
            string skinId = skin.Identifier.ToString("d08");
            PckAsset skinFile = new PckAsset($"dlcskin{skinId}.png", PckAssetType.SkinFile);

            skinFile.AddProperty("DISPLAYNAME", skin.MetaData.Name);
            if (localizationFile is not null)
            {
                string skinLocKey = $"IDS_dlcskin{skinId}_DISPLAYNAME";
                skinFile.AddProperty("DISPLAYNAMEID", skinLocKey);
                localizationFile.AddLocKey(skinLocKey, skin.MetaData.Name);
            }

            if (!string.IsNullOrEmpty(skin.MetaData.Theme))
            {
                skinFile.AddProperty("THEMENAME", skin.MetaData.Theme);
                if (localizationFile is not null)
                {
                    skinFile.AddProperty("THEMENAMEID", $"IDS_dlcskin{skinId}_THEMENAME");
                    localizationFile.AddLocKey($"IDS_dlcskin{skinId}_THEMENAME", skin.MetaData.Theme);
                }
            }

            if (skin.HasCape)
            {
                skinFile.AddProperty("CAPEPATH", $"dlccape{skinId}.png");
            }

            skinFile.AddProperty("ANIM", skin.Anim);
            skinFile.AddProperty("GAME_FLAGS", "0x18");
            skinFile.AddProperty("FREE", "1");

            foreach (SkinBOX box in skin.Model.AdditionalBoxes)
            {
                skinFile.AddProperty(box.ToProperty());
            }
            foreach (SkinPartOffset offset in skin.Model.PartOffsets)
            {
                skinFile.AddProperty(offset.ToProperty());
            }

            skinFile.SetTexture(skin.Texture);

            return skinFile;
        }

        public static PckAsset CreateCapeFile(this Skin.Skin skin, Image capeTexture)
        {
            if (!skin.HasCape)
                throw new InvalidOperationException("Skin does not contain a cape.");
            string skinId = skin.Identifier.ToString("d08");
            PckAsset capeFile = new PckAsset($"dlccape{skin.CapeId}.png", PckAssetType.CapeFile);
            capeFile.SetTexture(capeTexture);
            return capeFile;
        }

        public static Image GetPreviewImage(this Skin.Skin skin, Size size) => skin.GetPreviewImage(size.Width, size.Height);
        public static Image GetPreviewImage(this Skin.Skin skin, int width = 16, int height = 16)
        {
            Image result = new Bitmap(width, height);
            using Graphics g = Graphics.FromImage(result);
            g.ApplyConfig(GraphicsConfig.PixelPerfect());
            g.Clear(Color.Transparent);

            if (!skin.Anim.GetFlag(SkinAnimFlag.HEAD_DISABLED))
            {
                g.DrawImage(skin.Texture.GetArea(new Rectangle(8, 8, 8, 8)), 0, 0, width, height);
            }
            else if (!skin.Anim.GetFlag(SkinAnimFlag.HEAD_OVERLAY_DISABLED))
            {
                g.DrawImage(skin.Texture.GetArea(new Rectangle(40, 8, 8, 8)), 0, 0, width, height);
            }
            else
            {
                Rectangle area = skin.Model.AdditionalBoxes.Where(sb => sb.Type == "HEAD" || sb.Type == "HEADWEAR").OrderBy(sb=> sb.Pos.Z - sb.Scale).FirstOrDefault()?.GetFaceArea(SkinBOXExtensions.SkinBoxFace.Front) ?? Rectangle.Empty;
                Image img = skin.Texture.GetArea(area);
                g.DrawImage(img, 0, 0, width, height);
            }
            return result;
        }
    }
}
