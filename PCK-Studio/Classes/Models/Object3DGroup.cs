using System;
using System.Collections.Generic;

namespace PckStudio.Models
{
	public class Object3DGroup : Object3D
	{
		internal override MinecraftModelView Viewport
		{
			set
			{
				base.Viewport = value;
				foreach (Object3D object3D in objects)
				{
					object3D.Viewport = value;
				}
			}
		}

		public override System.Drawing.Image Image
		{
			set
			{
				foreach (Object3D object3D in objects)
				{
					object3D.Image = value;
				}
			}
		}

		internal override void Update()
		{
			Matrix3D globalTransformation = this.globalTransformation * localTransformation;
			for (int i = 0; i < objects.Count; i++)
			{
				objects[i].GlobalTransformation = globalTransformation;
			}
		}

		public override float HitTest(System.Drawing.PointF location)
		{
			float num = -1000f;
			foreach (Object3D object3D in objects)
			{
				float num2 = object3D.HitTest(location);
				if (num2 > num)
				{
					num = num2;
				}
			}
			return num;
		}

		public void Add(Object3D object3D)
		{
			if (object3D == this)
			{
				throw new ArgumentException("Cannot add Object3D into itself.");
			}
			objects.Add(object3D);
		}

		private List<Object3D> objects = new List<Object3D>();
	}
}
