using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OMI.Formats.Languages;
using OMI.Formats.Pck;
using PckStudio.Internal;

namespace PckStudio.Extensions
{
    internal static class SkinExtensions
    {
        public static PckFileData CreateFile(this Skin skin, LOCFile localizationFile)
        {
            string skinId = skin.Id.ToString("d08");
            PckFileData skinFile = new PckFileData($"dlcskin{skinId}.png", PckFileType.SkinFile);

            string skinLocKey = $"IDS_dlcskin{skinId}_DISPLAYNAME";
            skinFile.Properties.Add("DISPLAYNAME", skin.Name);
            skinFile.Properties.Add("DISPLAYNAMEID", skinLocKey);
            localizationFile.AddLocKey(skinLocKey, skin.Name);

            if (!string.IsNullOrEmpty(skin.Theme))
            {
                skinFile.Properties.Add("THEMENAME", skin.Theme);
                skinFile.Properties.Add("THEMENAMEID", $"IDS_dlcskin{skinId}_THEMENAME");
                localizationFile.AddLocKey($"IDS_dlcskin{skinId}_THEMENAME", skin.Theme);
            }

            if (skin.HasCape)
            {
                skinFile.Properties.Add("CAPEPATH", $"dlccape{skinId}.png");
            }

            skinFile.Properties.Add("ANIM", skin.ANIM);
            skinFile.Properties.Add("GAME_FLAGS", "0x18");
            skinFile.Properties.Add("FREE", "1");

            foreach (SkinBOX box in skin.AdditionalBoxes)
            {
                skinFile.Properties.Add(box.ToProperty());
            }
            foreach (SkinPartOffset offset in skin.PartOffsets)
            {
                skinFile.Properties.Add(offset.ToProperty());
            }

            skinFile.SetData(skin.Texture, ImageFormat.Png);

            return skinFile;
        }

        public static PckFileData CreateCapeFile(this Skin skin)
        {
            if (!skin.HasCape)
                throw new InvalidOperationException("Skin does not contain a cape.");
            string skinId = skin.Id.ToString("d08");
            PckFileData capeFile = new PckFileData($"dlccape{skinId}.png", PckFileType.CapeFile);
            capeFile.SetData(skin.CapeTexture, ImageFormat.Png);
            return capeFile;
        }
    }
}
