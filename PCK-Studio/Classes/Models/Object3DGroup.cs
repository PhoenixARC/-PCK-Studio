using System;
using System.Collections.Generic;

namespace PckStudio.Models
{
	public class Object3DGroup : global::PckStudio.Models.Object3D
	{
		internal override global::PckStudio.Models.MinecraftModelView Viewport
		{
			set
			{
				base.Viewport = value;
				foreach (global::PckStudio.Models.Object3D object3D in this.object3DList)
				{
					object3D.Viewport = value;
				}
			}
		}

		public override global::System.Drawing.Image Image
		{
			set
			{
				foreach (global::PckStudio.Models.Object3D object3D in this.object3DList)
				{
					object3D.Image = value;
				}
			}
		}

		internal override void Update()
		{
			global::PckStudio.Models.Matrix3D globalTransformation = this.globalTransformation * this.localTransformation;
			for (int i = 0; i < this.object3DList.Count; i++)
			{
				this.object3DList[i].GlobalTransformation = globalTransformation;
			}
		}

		public override float HitTest(global::System.Drawing.PointF location)
		{
			float num = -1000f;
			foreach (global::PckStudio.Models.Object3D object3D in this.object3DList)
			{
				float num2 = object3D.HitTest(location);
				if (num2 > num)
				{
					num = num2;
				}
			}
			return num;
		}

		public void Add(global::PckStudio.Models.Object3D object3D)
		{
			if (object3D == this)
			{
				throw new global::System.ArgumentException("Cannot add Object3D into itself.");
			}
			this.object3DList.Add(object3D);
		}

		public Object3DGroup()
		{
		}

		private global::System.Collections.Generic.List<global::PckStudio.Models.Object3D> object3DList = new global::System.Collections.Generic.List<global::PckStudio.Models.Object3D>();
	}
}
