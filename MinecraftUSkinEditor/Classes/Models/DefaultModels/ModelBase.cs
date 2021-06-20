using System;
using MinecraftUSkinEditor.Models;
using System.Runtime.CompilerServices;
using System.Threading;

namespace MinecraftUSkinEditor.Models
{
	public abstract class ModelBase
	{


		public ModelBase()
		{
			textures = InitializeTextures();
			foreach (Texture texture in textures)
			{
				texture.Updatedx += delegate(object sender, System.EventArgs args)
				{
					OnUpdated();
				};
			}
		}



		public Texture[] textures;

		public  System.EventHandler Updated;

		public Texture[] Textures
		{
			get
			{
				return textures;
			}
		}

		public  event System.EventHandler Updatedx
		{
			add
			{
				System.EventHandler eventHandler = Updated;
				System.EventHandler eventHandler2;
				do
				{
					eventHandler2 = eventHandler;
					System.EventHandler value2 = (System.EventHandler)System.Delegate.Combine(eventHandler2, value);
					eventHandler = System.Threading.Interlocked.CompareExchange<System.EventHandler>(ref Updated, value2, eventHandler2);
				}
				while (eventHandler != eventHandler2);
			}
			remove
			{
				System.EventHandler eventHandler = Updated;
				System.EventHandler eventHandler2;
				do
				{
					eventHandler2 = eventHandler;
					System.EventHandler value2 = (System.EventHandler)System.Delegate.Remove(eventHandler2, value);
					eventHandler = System.Threading.Interlocked.CompareExchange<System.EventHandler>(ref Updated, value2, eventHandler2);
				}
				while (eventHandler != eventHandler2);
			}
		}

		protected void OnUpdated()
		{
			if (Updated != null)
			{
				Updated(this, System.EventArgs.Empty);
			}
		}

		protected abstract Texture[] InitializeTextures();

		public abstract void AddToModelView(MinecraftModelView modelView);

		[System.Runtime.CompilerServices.CompilerGenerated]
		private void b__0(object sender, System.EventArgs args)
		{
			OnUpdated();
		}

	}
}
