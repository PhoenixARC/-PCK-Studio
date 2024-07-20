using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            var generateor = new AnimatedGifCreator(ms, GameConstants.GameTickInMilliseconds, 0);
            foreach (Animation.Frame frame in animation.GetInterpolatedFrames())
            {
                generateor.AddFrame(frame.Texture, frame.Ticks * GameConstants.GameTickInMilliseconds, GifQuality.Bit8);
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

        internal static bool ApplyAnim(this Animation animation, string animString)
        {
            if (string.IsNullOrEmpty(animString))
            {
                Trace.TraceError($"[{nameof(AnimationExtensions)}:{nameof(ApplyAnim)}] Failed to parse anim. anim was null or empty.");
                return false;
            }
            animString = animString.Trim();

            animation.Interpolate = animString.StartsWith("#");
            animString = animation.Interpolate ? animString.Substring(1) : animString;

            string[] animData = animString.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (animData.Length <= 0)
            {
                Trace.TraceError($"[{nameof(AnimationExtensions)}:{nameof(ApplyAnim)}] Failed to parse anim. animData was empty.");
                return false;
            }

            int lastFrameTime = Animation.MinimumFrameTime;
            foreach (string frameInfo in animData)
            {
                string[] frameData = frameInfo.Split('*');
                int currentFrameIndex = 0;
                int.TryParse(frameData[0], out currentFrameIndex);

                // Some textures like the Halloween 2015's Lava texture don't have a
                // frame time parameter for certain frames.
                // This will detect that and place the last frame time in its place.
                // This is accurate to console edition behavior.
                // - MattNL
                int currentFrameTime = frameData.Length < 2 || string.IsNullOrEmpty(frameData[1]) ? lastFrameTime : int.Parse(frameData[1]);
                animation.AddFrame(currentFrameIndex, currentFrameTime);
                lastFrameTime = currentFrameTime;
            }
            return true;
        }

        internal static string BuildAnim(this Animation animation)
        {
            StringBuilder stringBuilder = new StringBuilder(animation.Interpolate ? "#" : string.Empty);
            foreach (Animation.Frame frame in animation.GetFrames())
                stringBuilder.Append($"{animation.GetTextureIndex(frame.Texture)}*{frame.Ticks},");
            return stringBuilder.ToString(0, stringBuilder.Length - 1);
        }

        internal static Image BuildTexture(this Animation animation)
        {
            IReadOnlyCollection<Image> textures = animation.GetTextures();
            Size size = textures.First().Size;
            if (size.Width != size.Height)
                throw new Exception("Invalid size");

            return textures.Combine(ImageLayoutDirection.Vertical);
        }

    }
}
