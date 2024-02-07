using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using OpenTK;

namespace PckStudio.Rendering.Camera
{
    internal abstract class Camera
    {
        protected Matrix4 projectionMatrix;
        
        public abstract Matrix4 GetViewProjection();
    }
}
