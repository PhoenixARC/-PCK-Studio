using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using OpenTK;

namespace PckStudio.Rendering
{
    internal class Camera
    {
        public float Distance
        {
            get => _position.Z;
            set
            {
                _position.Z = value;
            }
        }

        public Vector2 Position
        {
            get => _position.Xy;
            set
            {
                _position.Xy = value;
            }
        }

        public Vector2 Rotation
        {
            get => _rotation;
            set
            {
                _rotation.X = MathHelper.Clamp(value.X, -180f, 180f);
                _rotation.Y = MathHelper.Clamp(value.Y, -180f, 180f);
            }
        }

        public float MinimumFov { get; set; } = 30f;
        public float MaximumFov { get; set; } = 120f;
        public float Fov
        {
            get => fov;
            set => fov = MathHelper.Clamp(value, MinimumFov, MaximumFov);
        }

        private Matrix4 viewProjection;
        private Matrix4 viewMatrix;
        private float fov;

        private Vector3 _position;
        private Vector2 _rotation;
        private Vector3 _target;

        public Camera(Vector2 position, float distance, Vector2 rotation, float fov)
        {
            _position = new Vector3(position) { Z = distance };
            _target = new Vector3(position);
            Rotation = rotation;
            Fov = fov;
        }

        internal Matrix4 GetViewProjection()
        {
            return viewProjection;
        }

        private void UpdateView()
        {
            var up = Vector3.UnitY;

            Matrix4 rotation = Matrix4.CreateFromAxisAngle(new Vector3(-1f, 0f, 0f), MathHelper.DegreesToRadians(Rotation.X))
                             * Matrix4.CreateFromAxisAngle(new Vector3(0f, 1f, 0f), MathHelper.DegreesToRadians(Rotation.Y));

            viewMatrix = Matrix4.LookAt(_position, _target, up) * rotation;
        }

        internal void Update(float aspect)
        {
            UpdateView();
            var projection = Matrix4.CreatePerspectiveFieldOfView((float)MathHelper.DegreesToRadians(Fov), aspect, 1f, 100f);
            viewProjection = viewMatrix * projection;
        }

        internal void LookAt(Vector2 pos)
        {
            _target = new Vector3(pos);
            Position = pos;
        }

        public override string ToString()
        {
            return $"FOV: {Fov}\nPosition: {Position}\nRotation: {Rotation}";
        }
    }
}
