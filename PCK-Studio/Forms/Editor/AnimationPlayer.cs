using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.CompilerServices;
using System.Diagnostics;

namespace PckStudio.Forms.Editor
{
	// TODO: write as a UI control ??
	sealed class AnimationPlayer
	{
		public const int BaseTickSpeed = 48;
		public bool IsPlaying { get; private set; } = false;

		private int currentAnimationFrameIndex = 0;
		private PictureBox display;
		private Animation _animation;
		private CancellationTokenSource cts = new CancellationTokenSource();

		public AnimationPlayer(PictureBox display)
		{
			SetContext(display);
		}

		private async void DoAnimate()
		{
			_ = display ?? throw new ArgumentNullException(nameof(display));
			_ = _animation ?? throw new ArgumentNullException(nameof(_animation));
			IsPlaying = true;
			while (!cts.IsCancellationRequested)
			{
				if (currentAnimationFrameIndex >= _animation.FrameCount)
					currentAnimationFrameIndex = 0;
				Animation.Frame frame = SetDisplayFrame(currentAnimationFrameIndex++);
				await Task.Delay(BaseTickSpeed * frame.Ticks);
			}
			IsPlaying = false;
		}

		public void Start(Animation animation)
		{
			_animation = animation;
			cts = new CancellationTokenSource();
			Task.Run(DoAnimate, cts.Token);
		}

		public void Stop([CallerMemberName] string callerName = default!)
		{
			Debug.WriteLine($"{nameof(AnimationPlayer.Stop)} called from {callerName}!");
			cts.Cancel();
		}

		public Animation.Frame GetCurrentFrame() => _animation[currentAnimationFrameIndex];

		public void SetContext(PictureBox display) => this.display = display;

		public void SelectFrame(Animation animation, int index)
		{
			_animation = animation;
			if (IsPlaying)
				Stop();
			SetDisplayFrame(index);
			currentAnimationFrameIndex = index;
		}

		private Animation.Frame SetDisplayFrame(int frameIndex)
		{
			Monitor.Enter(_animation);
			Animation.Frame frame = _animation[frameIndex];
			display.Image = frame.Texture;
			Monitor.Exit(_animation);
			return frame;
		}
	}
}
