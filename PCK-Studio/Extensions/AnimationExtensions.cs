using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AnimatedGif;
using Newtonsoft.Json.Linq;
using PckStudio.Internal;

namespace PckStudio.Extensions
{
    internal static class AnimationExtensions
    {
        internal static Image CreateAnimationImage(this Animation animation)
        {
            if (animation.FrameCount  == 0)
            {
                return null;
            }
            var ms = new System.IO.MemoryStream();
            var generateor = new AnimatedGifCreator(ms, Animation.GameTickInMilliseconds, 0);
            foreach (var frame in animation.GetInterpolatedFrames())
            {
                generateor.AddFrame(frame.Texture, frame.Ticks * Animation.GameTickInMilliseconds, GifQuality.Bit8);
            }
            ms.Position = 0;
            return Image.FromStream(ms);
        }

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
