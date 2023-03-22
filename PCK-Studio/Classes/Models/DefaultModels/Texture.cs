using System;
using System.Drawing;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;

namespace PckStudio.Models
{
	public class Texture
	{
		public Size Size => defaultSource.Size;

		public int Width => defaultSource.Width;
		
		public int Height => defaultSource.Height;

		public Image Source
		{
			get
			{
				return customSource ?? defaultSource;
			}
			set
			{
				if (value == customSource)
				{
					return;
				}
				if (value == null)
				{
					customSource = null;
				}
				else
				{
					if (value.Width != defaultSource.Width || value.Height != defaultSource.Height)
					{
						throw new ArgumentException("The texture is not the correct size");
					}
					customSource = value;
				}
				OnUpdate();
			}
		}

		public string FileName { get; set; }

		public event EventHandler Updatedx
		{
			add
			{
				EventHandler eventHandler = Update;
				EventHandler eventHandler2;
				do
				{
					eventHandler2 = eventHandler;
					EventHandler value2 = (EventHandler)Delegate.Combine(eventHandler2, value);
					eventHandler = Interlocked.CompareExchange(ref Update, value2, eventHandler2);
				}
				while (eventHandler != eventHandler2);
			}
			remove
			{
				EventHandler eventHandler = Update;
				EventHandler eventHandler2;
				do
				{
					eventHandler2 = eventHandler;
					EventHandler value2 = (EventHandler)Delegate.Remove(eventHandler2, value);
					eventHandler = Interlocked.CompareExchange(ref Update, value2, eventHandler2);
				}
				while (eventHandler != eventHandler2);
			}
		}

		protected void OnUpdate()
		{
			if (Update != null)
			{
				Update(this, EventArgs.Empty);
			}
		}

		public static implicit operator Texture(Image image) => new Texture(image);

		public Texture(Image defaultSource, Image customSource = null)
		{
			this.defaultSource = defaultSource;
			this.customSource = customSource;
		}

		public override string ToString()
		{
			if (customSource != null)
			{
				return Path.GetFileName(FileName);
			}
			return defaultSource.Width + " x " + defaultSource.Height;
		}

		private Image defaultSource;

		private Image customSource;

		private EventHandler Update;
	}
}
