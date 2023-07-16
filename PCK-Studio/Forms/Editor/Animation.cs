/* Copyright (c) 2023-present miku-666
 * This software is provided 'as-is', without any express or implied
 * warranty. In no event will the authors be held liable for any damages
 * arising from the use of this software.
 * 
 * Permission is granted to anyone to use this software for any purpose,
 * including commercial applications, and to alter it and redistribute it
 * freely, subject to the following restrictions:
 * 
 * 1. The origin of this software must not be misrepresented; you must not
 *    claim that you wrote the original software. If you use this software
 *    in a product, an acknowledgment in the product documentation would be
 *    appreciated but is not required.
 * 2. Altered source versions must be plainly marked as such, and must not be
 *    misrepresented as being the original software.
 * 3. This notice may not be removed or altered from any source distribution.
**/
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

		public enum AnimationCategory
        {
            Items,
            Blocks
        }

		public AnimationCategory Category { get; set; }

        public string CategoryString => GetCategoryName(Category);

		public static string GetCategoryName(AnimationCategory category)
		{
			return category switch
			{
				AnimationCategory.Items => "items",
				AnimationCategory.Blocks => "blocks",
				_ => throw new ArgumentOutOfRangeException(category.ToString())
			};
        }

        private readonly List<Image> textures;

		private readonly List<Frame> frames = new List<Frame>();


		public Animation(IEnumerable<Image> textures)
		{
			this.textures = new List<Image>(textures);
            AddSingleFrames();
        }

		public Animation(IEnumerable<Image> textures, string ANIM)
		{
            this.textures = new List<Image>(textures);
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
				AddSingleFrames();
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

		private void AddSingleFrames()
		{
            for (int i = 0; i < TextureCount; i++)
            {
                AddFrame(i);
            }
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
