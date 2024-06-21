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
                var texture = asset.GetTexture();
                var frameTextures = texture.Split(ImageLayoutDirection.Vertical);
                var _animation = new Animation(frameTextures, asset.GetProperty("ANIM"));
                return _animation;
            }
            return Animation.CreateEmpty();
        }

        public Animation DeserializeJavaAnimation(JObject jsonObject, Image texture)
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
