/* Copyright (c) 2023-present miku-666
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

namespace PckStudio.Forms.Editor
{
	internal class AnimationPictureBox : PictureBoxWithInterpolationMode
    {
		public bool IsPlaying => _isPlaying;

		private const int TickInMillisecond = 50; // 1 InGame tick

        private bool _isPlaying = false;
		private int currentAnimationFrameIndex = 0;
		private Animation.Frame currentFrame;
		private Animation _animation;
		private CancellationTokenSource cts = new CancellationTokenSource();
		private object l_dispose = new object();

        public void Start(Animation animation)
		{
			_animation = animation;
			cts = new CancellationTokenSource();
			Task.Run(DoAnimate, cts.Token);
		}

		public void Stop([CallerMemberName] string callerName = default!)
		{
			Debug.WriteLine($"{nameof(AnimationPictureBox.Stop)} called from {callerName}!");
			cts.Cancel();
		}

		public void SelectFrame(Animation animation, int index)
		{
			if (IsPlaying)
				Stop();
			_animation = animation;
			currentAnimationFrameIndex = index;
            currentFrame = SetAnimationFrame(index);
		}

		protected override void OnPaint(PaintEventArgs pe)
        {
			pe.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
			pe.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            base.OnPaint(pe);
        }

        private async void DoAnimate()
		{
			_ = _animation ?? throw new ArgumentNullException(nameof(_animation));
			_isPlaying = true;
			Animation.Frame nextFrame;
			while (!cts.IsCancellationRequested)
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
                    await InterpolateFrame(currentFrame, nextFrame);
					continue;
				}
				SetAnimationFrame(currentFrame);
                if (!await DelayAsync(TickInMillisecond * currentFrame.Ticks, cts.Token))
                    break;
            }
            _isPlaying = false;
		}

        private async Task InterpolateFrame(Animation.Frame currentFrame, Animation.Frame nextFrame)
        {
            for (int tick = 0; tick < currentFrame.Ticks && !cts.IsCancellationRequested; tick++)
            {
                double delta = 1.0f - tick / (double)currentFrame.Ticks;
				if (!IsHandleCreated)
					break;
                lock (l_dispose)
                {
					Invoke(() =>
					{
						Image = currentFrame.Texture.Interpolate(nextFrame.Texture, delta);
					});
                }

				if (!await DelayAsync(TickInMillisecond, cts.Token))
					break;
            }
        }

		private async Task<bool> DelayAsync(int millisecondsDelay, CancellationToken cancellationToken, [CallerMemberName] string caller = default!)
		{
            try
            {
                await Task.Delay(millisecondsDelay, cancellationToken);
            }
            catch
            {
                Debug.WriteLine($"Stoping {caller}");
				return false;
            }
			return true;
        }

		private Animation.Frame SetAnimationFrame(int frameIndex)
		{
			var frame = _animation.GetFrame(frameIndex);
			SetAnimationFrame(frame);
			return frame;
		}

		private void SetAnimationFrame(Animation.Frame frame)
		{
			if (!IsHandleCreated)
				return;
            lock (l_dispose)
            {
				Invoke(() =>
				{
					Image = frame.Texture;
				});
            }
        }

        protected override void Dispose(bool disposing)
        {
			lock(l_dispose)
			{
				base.Dispose(disposing);
			}
        }
	}
}
