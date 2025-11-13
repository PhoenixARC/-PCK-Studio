using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using PckStudio.Core.Extensions;
using PckStudio.Core.Skin;

namespace PckStudio.Core
{
    public static class GameConstants
    {
        // 16777215 being the uint24 max value
        public const int MAX_PACK_ID = 0xffffff;

        public const int GAMETICK_IN_MILLISECONDS = 50;

        public static readonly Vector3 SkinHeadTranslation = Vector3.Zero;
        public static readonly Vector3 SkinHeadPivot = Vector3.Zero;

        public static readonly Vector3 SkinBodyTranslation = Vector3.Zero;
        public static readonly Vector3 SkinBodyPivot = Vector3.Zero;

        public static readonly Vector3 SkinRightArmTranslation = new(-5f, 2f, 0f);
        public static readonly Vector3 SkinRightArmPivot = new(-6f, 2f, 0f);
                        
        public static readonly Vector3 SkinLeftArmTranslation = new(5f, 2f, 0f);
        public static readonly Vector3 SkinLeftArmPivot = new(6f, 2f, 0f);
                                
        public static readonly Vector3 SkinRightLegTranslation = new(-2f, 12f, 0f);
        public static readonly Vector3 SkinRightLegPivot = new(-2f, 12f, 0f);
                                        
        public static readonly Vector3 SkinLeftLegTranslation = new(2f, 12f, 0f);
        public static readonly Vector3 SkinLeftLegPivot = new(2f, 12f, 0f);

        private static Dictionary<string, PositioningInfo> _posisioningInfos = new Dictionary<string, PositioningInfo>()
        {
            ["HEAD"] = new PositioningInfo(SkinHeadTranslation, SkinHeadPivot),
            ["BODY"] = new PositioningInfo(SkinBodyTranslation, SkinBodyPivot),
            ["ARM0"] = new PositioningInfo(SkinRightArmTranslation, SkinRightArmPivot),
            ["ARM1"] = new PositioningInfo(SkinLeftArmTranslation, SkinLeftArmPivot),
            ["LEG0"] = new PositioningInfo(SkinRightLegTranslation, SkinRightLegPivot),
            ["LEG1"] = new PositioningInfo(SkinLeftLegTranslation, SkinLeftLegPivot),
        };
        public record struct PositioningInfo(Vector3 Translation, Vector3 Pivot);

        public static PositioningInfo GetPositioningInfo(string partName)
        {
            if (SkinBOX.IsOverlayPart(partName))
                partName = SkinBOXExtensions.GetBaseType(partName);
            return _posisioningInfos.ContainsKey(partName) ? _posisioningInfos[partName] : default;
        }

        public static Vector3 GetSkinPartPivot(string partName) => GetPositioningInfo(partName).Pivot;

        public static Vector3 GetSkinPartTranslation(string partName) => GetPositioningInfo(partName).Translation;


        // See: https://minecraft.fandom.com/wiki/Dye#Color_values for more information.
        public static readonly Color[] DyeColors = [
            Color.FromArgb(0xf9fffe), // White
            Color.FromArgb(0xf9801d), // Orange
            Color.FromArgb(0xc74ebd), // Magenta
            Color.FromArgb(0x3ab3da), // Light Blue
            Color.FromArgb(0xfed83d), // Yellow
            Color.FromArgb(0x80c71f), // Lime
            Color.FromArgb(0xf38baa), // Pink
            Color.FromArgb(0x474f52), // Gray
            Color.FromArgb(0x9d9d97), // Light Gray
            Color.FromArgb(0x169c9c), // Cyan
            Color.FromArgb(0x8932b8), // Purple
            Color.FromArgb(0x3c44aa), // Blue
            Color.FromArgb(0x835432), // Brown
            Color.FromArgb(0x5e7c16), // Green
            Color.FromArgb(0xb02e26), // Red
            Color.FromArgb(0x1d1d21), // Black
            ];
    }
}
