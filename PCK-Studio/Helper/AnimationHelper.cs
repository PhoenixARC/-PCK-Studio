using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using OMI.Formats.Pck;
using PckStudio.Extensions;
using PckStudio.Internal;

namespace PckStudio.Helper
{
    internal static class AnimationHelper
    {
        internal static void SaveAnimationToFile(PckFile.FileData file, Animation animation)
        {
            string anim = animation.BuildAnim();
            file.Properties.SetProperty("ANIM", anim);
            var texture = animation.BuildTexture();
            file.SetData(texture, ImageFormat.Png);
        }

        internal static Animation GetAnimationFromFile(PckFile.FileData file)
        {
            _ = file ?? throw new ArgumentNullException(nameof(file));
            if (file.Size > 0)
            {
                var texture = file.GetTexture();
                var frameTextures = texture.Split(ImageLayoutDirection.Vertical);
                var _animation = new Animation(frameTextures, file.Properties.GetPropertyValue("ANIM"));
                _animation.Category = file.Filename.Split('/').Contains("items")
                    ? AnimationCategory.Items
                    : AnimationCategory.Blocks;
                return _animation;
            }
            return Animation.Empty(file.Filename.Split('/').Contains("items")
                    ? AnimationCategory.Items
                    : AnimationCategory.Blocks);
        }

        internal static Animation GetAnimationFromJavaAnimation(JObject jsonObject, Image texture)
        {
            var textures = texture.Split(ImageLayoutDirection.Vertical);
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
