﻿/* Copyright (c) 2023-present miku-666
 * This software is provided 'as-is', without any express or implied
 * warranty. In no event will the authors be held liable for any damages
 * arising from the use of this software.
 * 
 * Permission is granted to anyone to use this software for any purpose,
 * including commercial applications, and to alter it and redistribute it
 * freely, subject to the following restrictions:
 * 
 * 1.The origin of this software must not be misrepresented; you must not
 *    claim that you wrote the original software. If you use this software
 *    in a product, an acknowledgment in the product documentation would be
 *    appreciated but is not required.
 * 2. Altered source versions must be plainly marked as such, and must not be
 *    misrepresented as being the original software.
 * 3. This notice may not be removed or altered from any source distribution.
**/
using System;
using System.Threading;
using System.Diagnostics;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Drawing.Drawing2D;
using System.Runtime.CompilerServices;

using PckStudio.Extensions;
using PckStudio.Internal;
using System.Drawing;

namespace PckStudio.ToolboxItems
{
	internal class AnimationPictureBox : BlendPictureBox
    {
		public bool IsPlaying
		{
			get
			{
				lock(l_playing)
				{
					return _isPlaying;
				}
			}
			private set
			{
                lock (l_playing)
                {
                    _isPlaying = value;
                }
            }
		}

        private bool _isPlaying = false;
		private int currentAnimationFrameIndex = 0;
		private Animation.Frame currentFrame;
		private Animation _animation;
		private CancellationTokenSource cts = new CancellationTokenSource();
		private object l_dispose = new object();
		private object l_playing = new object();

        public void Start(Animation animation)
		{
			_animation = animation;
			cts = new CancellationTokenSource();
			IsPlaying = true;
            Task.Run(DoAnimate, cts.Token);
		}

		public void Stop([CallerMemberName] string callerName = default!)
		{
			Debug.WriteLine($"{nameof(AnimationPictureBox.Stop)} called from {callerName}!");
			IsPlaying = false;
			cts.Cancel();
            cts.Token.WaitHandle.WaitOne(500);
        }

		public void SelectFrame(Animation animation, int index)
		{
			if (IsPlaying)
				Stop();
			_animation = animation;
			currentAnimationFrameIndex = index;
            currentFrame = SetAnimationFrame(index);
		}

        private void DoAnimate()
		{
			_ = _animation ?? throw new ArgumentNullException(nameof(_animation));
            Animation.Frame nextFrame;
			while (!cts.IsCancellationRequested && IsPlaying)
			{
				if (currentAnimationFrameIndex >= _animation.FrameCount)
				{
					currentAnimationFrameIndex = 0;
				}

				if (currentAnimationFrameIndex + 1 >= _animation.FrameCount)
				{
					nextFrame = _animation.GetFrame(0);
                }
				else
				{
					nextFrame = _animation.GetFrame(currentAnimationFrameIndex + 1);
				}

                currentFrame = _animation.GetFrame(currentAnimationFrameIndex++);
				if (_animation.Interpolate)
				{
                    InterpolateFrame(currentFrame, nextFrame);
					continue;
				}
				SetAnimationFrame(currentFrame);
                if (cts.Token.WaitHandle.WaitOne(Animation.GameTickInMilliseconds * currentFrame.Ticks))
				{
					IsPlaying = false;
                    break;
				}
            }
        }

        private void InterpolateFrame(Animation.Frame currentFrame, Animation.Frame nextFrame)
        {
            for (int tick = 0; tick < currentFrame.Ticks && !cts.IsCancellationRequested; tick++)
            {
                double delta = 1.0f - tick / (double)currentFrame.Ticks;
				SetTexture(currentFrame.Texture.Interpolate(nextFrame.Texture, delta));
				if (cts.Token.WaitHandle.WaitOne(Animation.GameTickInMilliseconds))
					break;
            }
        }

		private Animation.Frame SetAnimationFrame(int frameIndex)
		{
			var frame = _animation.GetFrame(frameIndex);
			SetAnimationFrame(frame);
			return frame;
		}

		private void SetTexture(Image texture)
		{
			if (!IsHandleCreated || Disposing)
				return;
			Invoke(() => Image = texture);
		}

		private void SetAnimationFrame(Animation.Frame frame)
		{
			SetTexture(frame.Texture);
        }

        protected override void Dispose(bool disposing)
        {
			Stop();
			cts.Token.WaitHandle.WaitOne(500);
			base.Dispose(disposing);
        }
	}
}
