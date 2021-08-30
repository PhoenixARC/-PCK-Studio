using System;
using System.Drawing;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;

namespace PckStudio.Models
{
	public class Texture
	{
		public System.Drawing.Size Size
		{
			get
			{
				return this.defaultSource.Size;
			}
		}

		public int Width
		{
			get
			{
				return this.defaultSource.Width;
			}
		}

		public int Height
		{
			get
			{
				return this.defaultSource.Height;
			}
		}

		public System.Drawing.Image Source
		{
			get
			{
				if (this.customSource != null)
				{
					return this.customSource;
				}
				return this.defaultSource;
			}
			set
			{
				if (value == this.customSource)
				{
					return;
				}
				if (value == null)
				{
					this.customSource = null;
				}
				else
				{
					if (value.Width != this.defaultSource.Width || value.Height != this.defaultSource.Height)
					{
						throw new System.ArgumentException("Tekstura ma nieodpowiedni rozmiar");
					}
					this.customSource = value;
				}
				this.OnUpdated();
			}
		}

		public string FileName
		{
			[System.Runtime.CompilerServices.CompilerGenerated]
			get
			{
				return this.k__BackingField;
			}
			[System.Runtime.CompilerServices.CompilerGenerated]
			set
			{
				this.k__BackingField = value;
			}
		}

		public event System.EventHandler Updatedx
		{
			add
			{
				System.EventHandler eventHandler = this.Updated;
				System.EventHandler eventHandler2;
				do
				{
					eventHandler2 = eventHandler;
					System.EventHandler value2 = (System.EventHandler)System.Delegate.Combine(eventHandler2, value);
					eventHandler = System.Threading.Interlocked.CompareExchange<System.EventHandler>(ref this.Updated, value2, eventHandler2);
				}
				while (eventHandler != eventHandler2);
			}
			remove
			{
				System.EventHandler eventHandler = this.Updated;
				System.EventHandler eventHandler2;
				do
				{
					eventHandler2 = eventHandler;
					System.EventHandler value2 = (System.EventHandler)System.Delegate.Remove(eventHandler2, value);
					eventHandler = System.Threading.Interlocked.CompareExchange<System.EventHandler>(ref this.Updated, value2, eventHandler2);
				}
				while (eventHandler != eventHandler2);
			}
		}

		protected void OnUpdated()
		{
			if (this.Updated != null)
			{
				this.Updated(this, System.EventArgs.Empty);
			}
		}

		public Texture(System.Drawing.Image defaultSource)
		{
			this.defaultSource = defaultSource;
		}

		public override string ToString()
		{
			if (this.customSource != null)
			{
				return System.IO.Path.GetFileName(this.FileName);
			}
			return this.defaultSource.Width + " x " + this.defaultSource.Height;
		}

		private System.Drawing.Image defaultSource;

		private System.Drawing.Image customSource;

		private System.EventHandler Updated;

		[System.Runtime.CompilerServices.CompilerGenerated]
		private string k__BackingField;
	}
}
