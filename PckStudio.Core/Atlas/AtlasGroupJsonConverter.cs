/* Copyright (c) 2025-present miku-666
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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PckStudio.Core.Extensions;
using PckStudio.Core.Json;

namespace PckStudio.Core
{
    public class AtlasGroupJsonConverter : JsonConverter<AtlasGroup>
    {
        public override AtlasGroup ReadJson(JsonReader reader, Type objectType, AtlasGroup existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            JObject jObject = JObject.Load(reader);

            if (!jObject.TryGetValue("name", out string name) || !jObject.TryGetValue("row", out int row) || !jObject.TryGetValue("column", out int column))
                return default;

            bool allowCustomColor = default;
            jObject.TryGetValue("allowCustomColor", out allowCustomColor);
            if (jObject.TryGetValue("overlay", out JsonRowAndColumn rowsAndColumns))
                return new AtlasOverlayGroup(name, row, column, rowsAndColumns, allowCustomColor);

            int frameTime = Animation.MINIMUM_FRAME_TIME;
            int frameCount = default;
            int rowSpan = default;
            int columnSpan = default;
            ImageLayoutDirection direction = default;
            bool isAnimation = jObject.TryGetValue("frameTime", out frameTime) &&
                jObject.TryGetValue("frameCount", out frameCount) &&
                jObject.TryGetValue("direction", out direction);
            bool isLargeTile = jObject.TryGetValue("rowSpan", out rowSpan) && jObject.TryGetValue("columnSpan", out columnSpan);
            if (isAnimation && isLargeTile)
                return new AtlasLargeTileAnimation(name, row, column, rowSpan, columnSpan, frameCount, direction, frameTime, allowCustomColor);
            if (isAnimation)
                return new AtlasAnimation(name, row, column, frameCount, direction, frameTime, allowCustomColor);
            if (isLargeTile)
                return new AtlasLargeTile(name, row, column, rowSpan, columnSpan, allowCustomColor);
            return default;
        }

        public override void WriteJson(JsonWriter writer, AtlasGroup value, JsonSerializer serializer) => serializer.Serialize(writer, value);
    }
}