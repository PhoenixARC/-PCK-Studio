using System;
using OpenTK;

namespace PckStudio.Rendering.Camera
{
    public abstract class Camera
    {
        protected Matrix4 projectionMatrix;
        
        public abstract Matrix4 GetViewProjection();
    }
}
