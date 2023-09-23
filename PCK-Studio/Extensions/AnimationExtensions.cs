using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;
using PckStudio.Internal;

namespace PckStudio.Extensions
{
    internal static class AnimationExtensions
    {

        internal static JObject ConvertToJavaAnimation(this Animation animation)
        {
            JObject janimation = new JObject();
            JObject mcmeta = new JObject();
            mcmeta["comment"] = $"Animation converted with {Application.ProductName}";
            mcmeta["animation"] = janimation;
            JArray jframes = new JArray();
            foreach (var frame in animation.GetFrames())
            {
                JObject jframe = new JObject();
                jframe["index"] = animation.GetTextureIndex(frame.Texture);
                jframe["time"] = frame.Ticks;
                jframes.Add(jframe);
            };
            janimation["interpolation"] = animation.Interpolate;
            janimation["frames"] = jframes;
            return mcmeta;
        }

    }
}
