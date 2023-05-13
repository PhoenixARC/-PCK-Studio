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

		public bool Interpolate { get; set; } = false;

		private readonly List<Image> textures;

		private readonly List<Frame> frames = new List<Frame>();


		public Animation(IEnumerable<Image> image)
		{
			textures = new List<Image>(image);
            for (int i = 0; i < TextureCount; i++)
            {
                AddFrame(i);
            }
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
			{
				for (int i = 0; i < TextureCount; i++)
				{
					AddFrame(i);
				}
				return;
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

		private void CheckTextureIndex(int index)
		{
            if ((index < 0 || index >= textures.Count))
                throw new ArgumentOutOfRangeException(nameof(index));
        }

		public Frame AddFrame(int textureIndex) => AddFrame(textureIndex, MinimumFrameTime);
		public Frame AddFrame(int textureIndex, int frameTime)
		{
			CheckTextureIndex(textureIndex);
			Frame frame = new Frame(textures[textureIndex], frameTime);
			frames.Add(frame);
			return frame;
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

		public List<Image> GetTextures()
		{
			return textures;
		}

		public int GetTextureIndex(Image frameTexture)
		{
			_ = frameTexture ?? throw new ArgumentNullException(nameof(frameTexture));
			return textures.IndexOf(frameTexture);
		}

		public void SetFrame(int frameIndex, Frame frame)
		{
			frames[frameIndex] = frame;
        }

		public void SetFrame(int frameIndex, int textureIndex, int frameTime = MinimumFrameTime)
		{
			CheckTextureIndex(textureIndex);
			SetFrame(frameIndex, new Frame(textures[textureIndex], frameTime));
		}

		public string BuildAnim()
		{
			StringBuilder stringBuilder = new StringBuilder(Interpolate ? "#" : string.Empty);
			foreach (var frame in frames)
				stringBuilder.Append($"{GetTextureIndex(frame.Texture)}*{frame.Ticks},");
			return stringBuilder.ToString(0, stringBuilder.Length - 1);
		}

		public Image BuildTexture()
		{	
			if (textures[0].Width != textures[0].Height)
				throw new Exception("Invalid size");

            return textures.CombineImages(ImageLayoutDirection.Vertical);
		}
	}
}
