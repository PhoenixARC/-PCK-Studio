using System;
using PckStudio.Models;
using System.Runtime.CompilerServices;
using System.Threading;

namespace PckStudio.Models
{
	public abstract class ModelBase
	{
		public ModelBase()
		{ }

		public Texture[] textures;

		public EventHandler OnUpdate;
        protected const float OverlayScale = 1.16f;

        protected void Initialize()
		{
            foreach (Texture texture in textures)
            {
                texture.Updatedx += (object sender, EventArgs args) => OnUpdated();
            }
        }

		public Texture[] Textures => textures;

		public event EventHandler Updatedx
		{
			add
			{
				EventHandler eventHandler = OnUpdate;
				EventHandler eventHandler2;
				do
				{
					eventHandler2 = eventHandler;
					EventHandler value2 = (EventHandler)Delegate.Combine(eventHandler2, value);
					eventHandler = Interlocked.CompareExchange(ref OnUpdate, value2, eventHandler2);
				}
				while (eventHandler != eventHandler2);
			}
			remove
			{
				EventHandler eventHandler = OnUpdate;
				EventHandler eventHandler2;
				do
				{
					eventHandler2 = eventHandler;
					EventHandler value2 = (EventHandler)Delegate.Remove(eventHandler2, value);
					eventHandler = Interlocked.CompareExchange(ref OnUpdate, value2, eventHandler2);
				}
				while (eventHandler != eventHandler2);
			}
		}

		protected void OnUpdated()
		{
			if (OnUpdate != null)
			{
				OnUpdate(this, EventArgs.Empty);
			}
		}

		public abstract void AddToModelView(MinecraftModelView modelView);
	}
}
