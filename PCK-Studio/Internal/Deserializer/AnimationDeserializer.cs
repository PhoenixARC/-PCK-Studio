using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using OMI.Formats.Pck;
using PckStudio.Extensions;
using PckStudio.Interfaces;

namespace PckStudio.Internal.Deserializer
{
    internal sealed class AnimationDeserializer : IPckAssetDeserializer<Animation>
    {
        public static readonly AnimationDeserializer DefaultDeserializer = new AnimationDeserializer();
        
        public Animation Deserialize(PckAsset asset)
        {
            _ = asset ?? throw new ArgumentNullException(nameof(asset));
            if (asset.Size > 0)
            {
                Image texture = asset.GetTexture();
                IEnumerable<Image> frameTextures = texture.Split(ImageLayoutDirection.Vertical);
                Animation animation = new Animation(frameTextures);
                DeserializeAnimationAnim(ref animation, asset.GetProperty("ANIM"));
                return animation;
            }
            return Animation.CreateEmpty();
        }

        private static bool DeserializeAnimationAnim(ref Animation animation, string animString)
        {
            if (string.IsNullOrEmpty(animString))
            {
                Trace.TraceError($"[{nameof(AnimationExtensions)}:{nameof(DeserializeAnimationAnim)}] Failed to parse anim. anim was null or empty.");
                return false;
            }
            animString = animString.Trim();

            animation.Interpolate = animString.StartsWith("#");
            animString = animation.Interpolate ? animString.Substring(1) : animString;

            string[] animData = animString.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (animData.Length <= 0)
            {
                Trace.TraceError($"[{nameof(AnimationExtensions)}:{nameof(DeserializeAnimationAnim)}] Failed to parse anim. animData was empty.");
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

        public Animation DeserializeJavaAnimation(JObject jsonObject, Image texture)
        {
            IEnumerable<Image> textures = texture.Split(ImageLayoutDirection.Vertical);
            Animation result = new Animation(textures);
            if (jsonObject["animation"] is not JToken animation)
                return result;

            int frameTime = Animation.MinimumFrameTime;

            if (animation["frametime"] is JToken frametime_token && frametime_token.Type == JTokenType.Integer)
                frameTime = (int)frametime_token;

            if (animation["interpolate"] is JToken interpolate_token && interpolate_token.Type == JTokenType.Boolean)
                result.Interpolate = (bool)interpolate_token;

            if (animation["frames"] is JToken frames_token && frames_token.Type == JTokenType.Array)
            {
                foreach (JToken frame in frames_token.Children())
                {
                    if (frame.Type == JTokenType.Object &&
                        frame["index"] is JToken frame_index &&
                        frame_index.Type == JTokenType.Integer &&
                        frame["time"] is JToken frame_time &&
                        frame_time.Type == JTokenType.Integer)
                    {
                        Debug.WriteLine("Index: {0}, Time: {1}", frame_index, frame_time);
                        result.AddFrame((int)frame_index, (int)frame_time);
                    }
                    else if (frame.Type == JTokenType.Integer)
                    {
                        Debug.WriteLine("Index: {0}, Time: {1}", frame, frameTime);
                        result.AddFrame((int)frame, frameTime);
                    }
                }
                return result;
            }

            for (int i = 0; i < result.TextureCount; i++)
            {
                result.AddFrame(i, frameTime);
            }

            return result;
        }
    }
}
