using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OMI.Formats.Languages;
using OMI.Formats.Pck;
using OMI.Workers;
using PckStudio.Internal;
using PckStudio.Interfaces;
using PckStudio.Internal.Deserializer;
using PckStudio.Internal.Serializer;

namespace PckStudio.Extensions
{
    internal static class PckFileDataExtensions
    {
        private const string MipMap = "MipMapLevel";

        internal static Image GetTexture(this PckAsset file)
        {
            if (file.Type != PckAssetType.SkinFile &&
                file.Type != PckAssetType.CapeFile &&
                file.Type != PckAssetType.TextureFile)
            {
                throw new Exception("File is not suitable to contain image data.");
            }
            return file.GetDeserializedData(ImageDeserializer.DefaultDeserializer);
        }

        /// <summary>
        /// Tries to get the skin id of the skin <paramref name="file"/>
        /// </summary>
        /// <param name="file"></param>
        /// <returns>Non-zero base number on success, otherwise 0</returns>
        /// <exception cref="InvalidOperationException"></exception>
        internal static int GetSkinId(this PckAsset file)
        {
            if (file.Type != PckAssetType.SkinFile)
                throw new InvalidOperationException("File is not a skin file");
            
            string filename = Path.GetFileNameWithoutExtension(file.Filename);
            if (!filename.StartsWith("dlcskin"))
            {
                Trace.TraceWarning($"[{nameof(GetSkin)}] File does not start with 'dlcskin'");
                return 0;
            }

            int skinId = 0;
            if (!int.TryParse(filename.Substring("dlcskin".Length), out skinId))
            {
                Trace.TraceWarning($"[{nameof(GetSkin)}] Failed to parse Skin Id");
            }
            return skinId;
        }

        internal static Skin GetSkin(this PckAsset file)
        {
            if (file.Type != PckAssetType.SkinFile)
                throw new InvalidOperationException("File is not a skin file");

            //if (file.Properties.Contains("CAPEPATH"))
            //    Debug.WriteLine($"[{nameof(GetSkin)}] TODO: add cape texture/path.");

            int skinId = file.GetSkinId();

            string name = file.GetProperty("DISPLAYNAME");
            Image texture = file.GetTexture();
            SkinANIM anim = file.GetProperty("ANIM", SkinANIM.FromString);
            IEnumerable<SkinBOX> boxes = file.GetMultipleProperties("BOX").Select(kv => SkinBOX.FromString(kv.Value));
            IEnumerable<SkinPartOffset> offsets = file.GetMultipleProperties("OFFSET").Select(kv => SkinPartOffset.FromString(kv.Value));
            return new Skin(name, skinId, texture, anim, boxes, offsets);
        }

        internal static void SetSkin(this PckAsset file, Skin skin, LOCFile localizationFile)
        {
            if (file.Type != PckAssetType.SkinFile)
                throw new InvalidOperationException("File is not a skin file");

            file.SetTexture(skin.Texture);

            string skinId = skin.Id.ToString("d08");

            // TODO: keep filepath 
            file.Filename = $"dlcskin{skinId}.png";

            string skinLocKey = $"IDS_dlcskin{skinId}_DISPLAYNAME";
            file.SetProperty("DISPLAYNAME", skin.Name);
            file.SetProperty("DISPLAYNAMEID", skinLocKey);
            localizationFile.AddLocKey(skinLocKey, skin.Name);

            if (!string.IsNullOrEmpty(skin.Theme))
            {
                file.SetProperty("THEMENAME", skin.Theme);
                file.SetProperty("THEMENAMEID", $"IDS_dlcskin{skinId}_THEMENAME");
                localizationFile.AddLocKey($"IDS_dlcskin{skinId}_THEMENAME", skin.Theme);
            }

            if (skin.HasCape)
            {
                file.SetProperty("CAPEPATH", $"dlccape{skinId}.png");
            }

            file.SetProperty("ANIM", skin.ANIM.ToString());
            file.SetProperty("GAME_FLAGS", "0x18");
            file.SetProperty("FREE", "1");

            file.RemoveProperties("BOX");
            file.RemoveProperties("OFFSET");

            foreach (SkinBOX box in skin.AdditionalBoxes)
            {
                file.AddProperty(box.ToProperty());
            }
            foreach (SkinPartOffset offset in skin.PartOffsets)
            {
                file.AddProperty(offset.ToProperty());
            }
        }

        internal static T GetDeserializedData<T>(this PckAsset file, IPckDeserializer<T> deserializer)
        {
            return deserializer.Deserialize(file);
        }

        internal static T GetData<T>(this PckAsset file, IDataFormatReader<T> formatReader) where T : class
        {
            using var ms = new MemoryStream(file.Data);
            return formatReader.FromStream(ms);
        }

        internal static void SetSerializedData<T>(this PckAsset file, T obj, IPckSerializer<T> serializer)
        {
            serializer.Serialize(obj, ref file);
        }

        internal static void SetData(this PckAsset file, IDataFormatWriter formatWriter)
        {
            using (var stream = new MemoryStream())
            {
                formatWriter.WriteToStream(stream);
                file.SetData(stream.ToArray());
            }
        }

        internal static void SetTexture(this PckAsset file, Image image)
        {
            if (file.Type != PckAssetType.SkinFile &&
                file.Type != PckAssetType.CapeFile &&
                file.Type != PckAssetType.TextureFile)
            {
                throw new Exception("File is not suitable to contain image data.");
            }
            file.SetSerializedData(image, ImageSerializer.DefaultSerializer);
        }

        internal static bool IsMipmappedFile(this PckAsset file)
        {
            // We only want to test the file name itself. ex: "terrainMipMapLevel2"
            string name = Path.GetFileNameWithoutExtension(file.Filename);

            // check if last character is a digit (0-9). If not return false
            if (!char.IsDigit(name[name.Length - 1]))
                return false;

            // If string does not end with MipMapLevel, then it's not MipMapped
            if (!name.Remove(name.Length - 1, 1).EndsWith(MipMap))
                return false;
            return true;
        }

        internal static string GetNormalPath(this PckAsset file)
        {
            if (!file.IsMipmappedFile())
                return file.Filename;
            string ext = Path.GetExtension(file.Filename);
            return file.Filename.Remove(file.Filename.Length - (MipMap.Length + 1) - ext.Length) + ext;
        }
    }
}
