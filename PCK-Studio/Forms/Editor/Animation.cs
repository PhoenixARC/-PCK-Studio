using System;
using System.Collections.Generic;
using System.Drawing;
using PckStudio.Extensions;
using System.Text;


// TODO: change namespace
namespace PckStudio.Forms.Editor
{
	sealed class Animation
	{
		public const int MinimumFrameTime = 1;

		public int FrameCount => frames.Count;

		public int TextureCount => frameTextures.Count;

		public Frame this[int frameIndex] => frames[frameIndex];

		// TODO: implement this
		public bool Interpolate { get; set; } = false;

		private readonly List<Image> frameTextures;

		private readonly List<Frame> frames = new List<Frame>();


		public Animation(IEnumerable<Image> image)
		{
			frameTextures = new List<Image>(image);
		}

		public Animation(IEnumerable<Image> frameTextures, string ANIM) : this(frameTextures)
		{
			ParseAnim(ANIM);
		}

		public class Frame
		{
			public readonly Image Texture;
			public int Ticks;

			public Frame(Image texture) : this(texture, MinimumFrameTime)
			{ }

			public Frame(Image texture, int frameTime)
			{
				Texture = texture;
				Ticks = frameTime;
			}
		}

		private void ParseAnim(string anim)
		{
			_ = anim ?? throw new ArgumentNullException(nameof(anim));
			anim = anim.Trim();
			anim = (Interpolate = anim.StartsWith("#")) ? anim.Substring(1) : anim;
			string[] animData = anim.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
			int lastFrameTime = MinimumFrameTime;
			if (animData.Length <= 0)
				for (int i = 0; i < TextureCount; i++)
				{
					AddFrame(i);
				}
			
			foreach (string frameInfo in animData)
			{
				string[] frameData = frameInfo.Split('*');
				//if (frameData.Length < 2)
				//    continue; // shouldn't happen
				int currentFrameIndex = 0;
                int.TryParse(frameData[0], out currentFrameIndex);

				// Some textures like the Halloween 2015's Lava texture don't have a
				// frame time parameter for certain frames.
				// This will detect that and place the last frame time in its place.
				// This is accurate to console edition behavior.
				// - MattNL
				int currentFrameTime = string.IsNullOrEmpty(frameData[1]) ? lastFrameTime : int.Parse(frameData[1]);
				AddFrame(currentFrameIndex, currentFrameTime);
				lastFrameTime = currentFrameTime;
			}
		}

		public Frame AddFrame(int frameTextureIndex) => AddFrame(frameTextureIndex, MinimumFrameTime);
		public Frame AddFrame(int frameTextureIndex, int frameTime)
		{
			if (frameTextureIndex < 0 || frameTextureIndex >= frameTextures.Count)
				throw new ArgumentOutOfRangeException(nameof(frameTextureIndex));
			Frame f = new Frame(frameTextures[frameTextureIndex], frameTime);
			frames.Add(f);
			return f;
		}

		public bool RemoveFrame(int frameIndex)
		{
			frames.RemoveAt(frameIndex);
			return true;
		}

		public Frame GetFrame(int index) => frames[index];

		public List<Frame> GetFrames()
		{
			return frames;
		}

		public List<Image> GetFrameTextures()
		{
			return frameTextures;
		}

		public int GetTextureIndex(Image frameTexture)
		{
			_ = frameTexture ?? throw new ArgumentNullException(nameof(frameTexture));
			return frameTextures.IndexOf(frameTexture);
		}

		public void SetFrame(Frame frame, int frameTextureIndex, int frameTime = MinimumFrameTime)
			=> SetFrame(frames.IndexOf(frame), frameTextureIndex, frameTime);
		public void SetFrame(int frameIndex, int frameTextureIndex, int frameTime = MinimumFrameTime)
		{
			frames[frameIndex] = new Frame(frameTextures[frameTextureIndex], frameTime);
		}

		public string BuildAnim()
		{
			StringBuilder stringBuilder = new StringBuilder(Interpolate ? "#" : string.Empty);
			foreach (var frame in frames)
				stringBuilder.Append($"{GetTextureIndex(frame.Texture)}*{frame.Ticks},");
			return stringBuilder.ToString(0, stringBuilder.Length - 1);
		}

		public Image BuildTexture(bool isClockOrCompass, List<Image> linearImages = default!)
		{
			int width = frameTextures[0].Width;
			int height = frameTextures[0].Height;
			if (width != height)
				throw new Exception("Invalid size");

			var textures = isClockOrCompass ? linearImages : frameTextures;

			return ImageExtensions.CombineImages(textures, ImageLayoutDirection.Vertical);
		}
	}
}
