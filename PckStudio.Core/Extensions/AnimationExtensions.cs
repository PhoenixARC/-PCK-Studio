using System.Drawing;
using AnimatedGif;

namespace PckStudio.Core.Extensions
{
    public static class AnimationExtensions
    {
        public static Image CreateAnimationImage(this Animation animation)
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
    }
}
