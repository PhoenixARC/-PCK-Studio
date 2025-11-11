using System.Drawing;
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
            var generateor = new AnimatedGifCreator(ms, GameConstants.GameTickInMilliseconds, 0);
            foreach (Animation.Frame frame in animation.GetInterpolatedFrames())
            {
                Image texture = (blendColor == Color.Black || blendColor == Color.White) ? frame.Texture : frame.Texture.Blend(blendColor, BlendMode.Multiply);
                generateor.AddFrame(texture, frame.Ticks * GameConstants.GameTickInMilliseconds, GifQuality.Bit8);
            }
            ms.Position = 0;
            return Image.FromStream(ms);
        }
    }
}
