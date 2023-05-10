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

		public int TextureCount => textures.Count;

		public Frame this[int frameIndex] => frames[frameIndex];

		public bool Interpolate { get; set; } = false;

		private readonly List<Image> textures;

		private readonly List<Frame> frames = new List<Frame>();


		public Animation(IEnumerable<Image> image)
		{
			textures = new List<Image>(image);
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
				int currentFrameIndex = 0;
                int.TryParse(frameData[0], out currentFrameIndex);

				// Some textures like the Halloween 2015's Lava texture don't have a
				// frame time parameter for certain frames.
				// This will detect that and place the last frame time in its place.
				// This is accurate to console edition behavior.
				// - MattNL
				int currentFrameTime = frameData.Length < 2 || string.IsNullOrEmpty(frameData[1]) ? lastFrameTime : int.Parse(frameData[1]);
				AddFrame(currentFrameIndex, currentFrameTime);
				lastFrameTime = currentFrameTime;
			}
		}

		public Frame AddFrame(int frameTextureIndex) => AddFrame(frameTextureIndex, MinimumFrameTime);
		public Frame AddFrame(int frameTextureIndex, int frameTime)
		{
			if (frameTextureIndex < 0 || frameTextureIndex >= textures.Count)
				throw new ArgumentOutOfRangeException(nameof(frameTextureIndex));
			Frame f = new Frame(textures[frameTextureIndex], frameTime);
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
			return textures;
		}

		public int GetTextureIndex(Image frameTexture)
		{
			_ = frameTexture ?? throw new ArgumentNullException(nameof(frameTexture));
			return textures.IndexOf(frameTexture);
		}

		public void SetFrame(Frame frame, int frameTextureIndex, int frameTime = MinimumFrameTime)
			=> SetFrame(frames.IndexOf(frame), frameTextureIndex, frameTime);
		public void SetFrame(int frameIndex, int frameTextureIndex, int frameTime = MinimumFrameTime)
		{
			frames[frameIndex] = new Frame(textures[frameTextureIndex], frameTime);
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
			var textures = isClockOrCompass ? linearImages : this.textures;
			
			if (textures[0].Width != textures[0].Height)
				throw new Exception("Invalid size");

			return ImageExtensions.CombineImages(textures, ImageLayoutDirection.Vertical);
		}
	}
}
