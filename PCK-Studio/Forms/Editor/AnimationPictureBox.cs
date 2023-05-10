using System;
using System.Threading;
using System.Diagnostics;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Drawing.Drawing2D;
using System.Runtime.CompilerServices;

using PckStudio.Extensions;

namespace PckStudio.Forms.Editor
{
	internal class AnimationPictureBox : PictureBox
    {
		public bool IsPlaying => _isPlaying;

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

		public Animation.Frame GetCurrentFrame() => _animation[currentAnimationFrameIndex];

		public void SelectFrame(Animation animation, int index)
		{
			if (IsPlaying)
				Stop();
			_animation = animation;
			currentAnimationFrameIndex = index;
            currentFrame = SetAnimationFrame(index);
		}

		private const int TickInMillisecond = 50; // 1 InGame tick

        private bool _isPlaying = false;
		private int currentAnimationFrameIndex = 0;
		private Animation.Frame currentFrame;
		private Animation _animation;
		private CancellationTokenSource cts = new CancellationTokenSource();

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
					nextFrame = _animation[0];
                }
				else
				{
					nextFrame = _animation[currentAnimationFrameIndex + 1];
				}

                currentFrame = _animation[currentAnimationFrameIndex++];
				if (_animation.Interpolate)
				{
                    await InterpolateFrame(currentFrame, nextFrame);
					continue;
				}
				SetAnimationFrame(currentFrame);
                await Task.Delay(TickInMillisecond * currentFrame.Ticks);
            }
            _isPlaying = false;
		}

        private async Task InterpolateFrame(Animation.Frame currentFrame, Animation.Frame nextFrame)
        {
            for (int tick = 0; tick < currentFrame.Ticks && !cts.IsCancellationRequested; tick++)
            {
                double delta = 1.0f - tick / (double)currentFrame.Ticks;
				if (!IsDisposed)
					Invoke(() =>
					{
                        if (!IsDisposed)
                            Image = currentFrame.Texture.Interpolate(nextFrame.Texture, delta);
					});
                await Task.Delay(TickInMillisecond);
            }
        }

		private Animation.Frame SetAnimationFrame(int frameIndex)
		{
			var frame = _animation[frameIndex];
			SetAnimationFrame(frame);
			return frame;
		}

		private void SetAnimationFrame(Animation.Frame frame)
		{
            Invoke(() => Image = frame.Texture);
        }
	}
}
