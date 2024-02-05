using System;
using System.Linq;
using System.IO;
using PckStudio.Extensions;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace PckStudio.Internal
{
    internal readonly struct SkinPartOffset
    {
        private static readonly Regex sWhitespace = new Regex(@"\s+");
        internal static string ReplaceWhitespace(string input, string replacement)
        {
            return sWhitespace.Replace(input, replacement);
        }

        private static readonly string[] ValidModelOffsetTypes = new string[]
        {
            // Body Offsets
            "HEAD",
            "BODY",
            "ARM0",
            "ARM1",
            "LEG0",
            "LEG1",
            "PANTS0",
            "PANTS1",
            "BOOTS0",
            "BOOTS1",

            // Armor Offsets
            "HELMET",
            "CHEST",
            "BODYARMOR",
            "SHOULDER0",
            "SHOULDER1",
            "ARMARMOR0",
            "ARMARMOR1",
            "BELT",
            "LEGGING0",
            "LEGGING1",
            "SOCK0", "BOOT0",
            "SOCK1", "BOOT1",

            "TOOL0",
            "TOOL1",
        };

        public readonly string Type;
        public readonly float Value;

        public SkinPartOffset(string type, float value)
        {
            Type = type;
            Value = value;
        }

        public static SkinPartOffset FromString(string offsetFormatString)
        {
            string[] offset = ReplaceWhitespace(offsetFormatString.TrimEnd('\n', '\r', ' '), ",").Split(',');
            if (offset.Length < 3)
                throw new InvalidDataException("Format string does not contain enough data.");

            string type = offset[0];

            if (!ValidModelOffsetTypes.Contains(type))
            {
                Debug.WriteLine($"'{type}' is an invalid offset type.", category: nameof(SkinPartOffset));
            }

            // Ignore => Y assumed
            //if (offset[1] != "Y")

            if (!float.TryParse(offset[2], out float value))
            {
                Debug.WriteLine($"Failed to parse y offset for: '{type}'", category: nameof(SkinPartOffset));
            }
            return new SkinPartOffset(type, value);
        }

        public (string, string) ToProperty()
        {
            string value = $"{Type} Y {Value}";
            return ("OFFSET", value.Replace(',', '.'));
        }
    }
}