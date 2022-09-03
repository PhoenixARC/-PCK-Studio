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
				foreach (Object3D object3D in object3DList)
				{
					object3D.Viewport = value;
				}
			}
		}

		public override System.Drawing.Image Image
		{
			set
			{
				foreach (Object3D object3D in object3DList)
				{
					object3D.Image = value;
				}
			}
		}

		internal override void Update()
		{
			Matrix3D globalTransformation = this.globalTransformation * localTransformation;
			for (int i = 0; i < object3DList.Count; i++)
			{
				object3DList[i].GlobalTransformation = globalTransformation;
			}
		}

		public override float HitTest(System.Drawing.PointF location)
		{
			float num = -1000f;
			foreach (Object3D object3D in object3DList)
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
			object3DList.Add(object3D);
		}

		private List<Object3D> object3DList = new List<Object3D>();
	}
}
