using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using AnimatedGif;

namespace PckStudio.Core.Extensions
{
    public static class AnimationExtensions
    {
        public static Image CreateAnimationImage(this Animation animation) => animation.CreateAnimationImage(Color.Black);

        public static Image CreateAnimationImage(this Animation animation, Color blendColor)
        {
            if (animation.FrameCount  == 0)
            {
                return null;
            }
            var ms = new System.IO.MemoryStream();
            var generateor = new AnimatedGifCreator(ms, GameConstants.GAMETICK_IN_MILLISECONDS, 0);
            foreach (Animation.Frame frame in animation.GetInterpolatedFrames())
            {
                Image texture = (blendColor == Color.Black || blendColor == Color.White) ? frame.Texture : frame.Texture.Blend(blendColor, BlendMode.Multiply);
                generateor.AddFrame(texture, frame.Ticks * GameConstants.GAMETICK_IN_MILLISECONDS, GifQuality.Bit8);
            }
            ms.Position = 0;
            return Image.FromStream(ms);
        }

        public static Animation Combine(this Animation first, Animation second, ImageLayoutDirection layoutDirection)
        {
            if (first == null)
                return second;
            if (second == null)
                return first;
            if (first.TextureCount != second.TextureCount)
                return first;
            if (first.FrameCount != second.FrameCount)
                return first;

            Image[] secondTextures = second.GetTextures().ToArray();

            Animation animation = new Animation(first.GetTextures().enumerate().Select(ift => ift.value.Combine(secondTextures[ift.index], layoutDirection)));
            foreach ((int texId, int frameTime) item in first.GetFrames().Select(f => (texId: first.GetTextureIndex(f.Texture), frameTime: f.Ticks)))
            {
                animation.AddFrame(item.texId, item.frameTime);
            }
            return animation;
        }
    }
}
