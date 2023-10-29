using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using OpenTK;

namespace PckStudio.Rendering
{
    internal abstract class Camera
    {
        public abstract float Distance { get; set; }

        public abstract Vector2 Position { get; set; }

        internal abstract Matrix4 GetViewProjection();

        internal abstract void Update(float aspect);

        public override string ToString()
        {
            return $"Position: {Position}\nDistance: {Distance}";
        }
    }
}
