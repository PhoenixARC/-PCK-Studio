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
using System.Collections.Generic;
using System;
using System.Drawing;
using System.Text;
using OMI.Formats.Pck;
using PckStudio.Core.Extensions;
using PckStudio.Interfaces;
using PckStudio.Core;
using System.Linq;
using Newtonsoft.Json.Linq;
using System.Windows.Forms;

namespace PckStudio.Core.Serializer
{
    public sealed class AnimationSerializer : IPckAssetSerializer<Animation>
    {
        public static readonly AnimationSerializer DefaultSerializer = new AnimationSerializer();

        public void Serialize(Animation animation, ref PckAsset asset)
        {
            string anim = SerializeAnim(animation);
            asset.SetProperty("ANIM", anim);
            Image texture = SerializeTexture(animation);
            asset.SetTexture(texture);
        }

        private static string SerializeAnim(Animation animation)
        {
            StringBuilder stringBuilder = new StringBuilder(animation.Interpolate ? "#" : string.Empty);
            foreach (Animation.Frame frame in animation.GetFrames())
                stringBuilder.Append($"{animation.GetTextureIndex(frame.Texture)}*{frame.Ticks},");
            return stringBuilder.ToString(0, stringBuilder.Length - 1);
        }

        public static Image SerializeTexture(Animation animation)
        {
            IReadOnlyCollection<Image> textures = animation.GetTextures();
            Size size = textures.First().Size;
            if (size.Width != size.Height)
                throw new Exception("Invalid size");

            return textures.Combine(ImageLayoutDirection.Vertical);
        }

        public static JObject SerializeJavaAnimation(Animation animation)
        {
            JObject janimation = new JObject();
            JObject mcmeta = new JObject();
            mcmeta["comment"] = $"Animation converted with {Application.ProductName}";
            mcmeta["animation"] = janimation;
            JArray jframes = new JArray();
            foreach (Animation.Frame frame in animation.GetFrames())
            {
                JObject jframe = new JObject();
                jframe["index"] = animation.GetTextureIndex(frame.Texture);
                jframe["time"] = frame.Ticks;
                jframes.Add(jframe);
            }
            janimation["interpolation"] = animation.Interpolate;
            janimation["frames"] = jframes;
            return mcmeta;
        }
    }
}
