﻿/* Copyright (c) 2023-present miku-666
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
using System.Collections.ObjectModel;
using System.Linq;

namespace PckStudio.Internal
{
    internal sealed class Animation
	{
		public const int MinimumFrameTime = 1;

		public const int GameTickInMilliseconds = 50;

		public static Animation Empty(AnimationCategory category)
		{
            var animation = new Animation(Array.Empty<Image>(), string.Empty);
			animation.Category = category;
			return animation;
		}

		public int FrameCount => frames.Count;

		public int TextureCount => textures.Count;

		public bool Interpolate { get; set; } = false;


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

		private readonly IList<Frame> frames = new List<Frame>();

		public Animation(IEnumerable<Image> textures)
		{
			this.textures = new List<Image>(textures);
        }

		public Animation(IEnumerable<Image> textures, string ANIM)
			: this(textures)
		{
            ParseAnim(ANIM);
		}

		public class Frame
		{
			public readonly Image Texture;
			public int Ticks
			{
				get
				{
					return _ticks;
				}
				set
				{
					lock(l_ticks)
					{
						_ticks = value;
					}
				}
			}

			private int _ticks;
			private object l_ticks = new object();

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
			if (string.IsNullOrEmpty(anim))
			{
				AddSingleFrames();
				return;
			}
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
            if (!textures.IndexInRange(index))
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

		public IReadOnlyCollection<Frame> GetFrames()
		{
			return new ReadOnlyCollection<Frame>(frames);
		}

		public IReadOnlyCollection<Frame> GetInterpolatedFrames()
		{
            if (Interpolate)
            {
                return new ReadOnlyCollection<Frame>(InternalGetInterpolatedFrames().ToList());
            }
			return GetFrames();
        }

		private IEnumerable<Frame> InternalGetInterpolatedFrames()
		{
			for (int i = 0; i < FrameCount; i++)
			{
				Frame currentFrame = frames[i];
				Frame nextFrame = frames[0];
				if (i + 1 < FrameCount)
					nextFrame = frames[i + 1];
				for (int tick = 0; tick < currentFrame.Ticks; tick++)
				{
					double delta = 1.0f - tick / (double)currentFrame.Ticks;
					yield return new Frame(currentFrame.Texture.Interpolate(nextFrame.Texture, delta));
				}
			}
			yield break;
		}


        public IReadOnlyCollection<Image> GetTextures()
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
			lock(frames)
			{
				frames[frameIndex] = frame;
			}
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

            return textures.Combine(ImageLayoutDirection.Vertical);
		}

        internal void SetFrameTicks(int ticks)
        {
			lock(frames)
			{
				foreach (var frame in frames)
				{
					frame.Ticks = ticks;
				}
			}
        }

        internal void SwapFrames(int sourceIndex, int destinationIndex)
        {
			lock(frames)
			{
				frames.Swap(sourceIndex, destinationIndex);
			}
        }
    }
}
