/* Copyright (c) 2024-present miku-666
 * This software is provided 'as-is', without any express or implied
 * warranty. In no event will the authors be held liable for any damages
 * arising from the use of this software.
 * 
 * Permission is granted to anyone to use this software for any purpose,
 * including commercial applications, and to alter it and redistribute it
 * freely, subject to the following restrictions:
 * 
 * 1.The origin of this software must not be misrepresented; you must not
 *    claim that you wrote the original software. If you use this software
 *    in a product, an acknowledgment in the product documentation would be
 *    appreciated but is not required.
 * 2. Altered source versions must be plainly marked as such, and must not be
 *    misrepresented as being the original software.
 * 3. This notice may not be removed or altered from any source distribution.
**/
// movement code taken from: 
// https://github.com/TheCherno/Hazel/blob/master/Hazel/src/Hazel/Renderer/EditorCamera.cpp
using System;
using System.Drawing;
using OpenTK;

namespace PckStudio.Rendering.Camera
{
    internal class PerspectiveCamera : Camera
    {
        public float Distance
        {
            get => _distance;
            set
            {
                _distance = value;
                UpdateViewMatrix();
            }
        }

        public Size ViewportSize
        {
            get => _viewportSize;
            set
            {
                _viewportSize = value;
                UpdateProjection();
            }
        }

        public Vector3 WorldPosition => _position;
        public Vector3 FocalPoint
        {
            get => _focalPoint;
            set
            {
                _focalPoint = value;
                UpdateViewMatrix();
            }
        }

        public Vector2 Rotation
        {
            get => _rotation;
            set
            {
                _rotation.X = MathHelper.Clamp(value.X, -180f, 180f);
                _rotation.Y = MathHelper.Clamp(value.Y, -180f, 180f);
                UpdateViewMatrix();
            }
        }

        public Vector3 Orientation => -Vector3.UnitZ;

        public Vector3 Up = Vector3.UnitY;
        
        public float MinimumFov { get; set; } = 30f;
        public float MaximumFov { get; set; } = 120f;

        public float Fov
        {
            get => fov;
            set
            {
                fov = MathHelper.Clamp(value, MinimumFov, MaximumFov);
                UpdateProjection();
            }
        }

        public PerspectiveCamera(float fov, Vector3 position)
        {
            Fov = fov;
            _position = position;
            _focalPoint = Vector3.Zero;
        }

        private Matrix4 viewMatrix;
        private float fov;

        private float _distance;
        private Size _viewportSize;
        private Vector3 _position;
        private Vector2 _rotation;
        private Vector3 _focalPoint;

        public override Matrix4 GetViewProjection()
        {
            return viewMatrix * projectionMatrix;
        }

        private void UpdateViewMatrix()
        {
            // m_Yaw = m_Pitch = 0.0f; // Lock the camera's rotation
            _position = CalculatePosition();

            Quaternion orientation = GetOrientation();
            var rotation = Matrix4.CreateTranslation(_position);
            rotation *= Matrix4.CreateFromQuaternion(orientation);
            rotation *= Matrix4.CreateTranslation(_position).Inverted();

            viewMatrix = Matrix4.CreateTranslation(_position) * rotation;
            viewMatrix = viewMatrix.Inverted();
        }

        private void UpdateProjection()
        {
            float aspect = (float)ViewportSize.Width / (float)ViewportSize.Height;
            projectionMatrix = Matrix4.CreatePerspectiveFieldOfView((float)MathHelper.DegreesToRadians(Fov), aspect, 1f, 1000f);
        }

        private Vector2 GetPanSpeed()
        {
            float x = Math.Min(ViewportSize.Width / 100.0f, 1.4f); // max = 2.4f
            float xFactor = 0.0366f * (x * x) - 0.1778f * x + 0.3021f;

            float y = Math.Min(ViewportSize.Height / 100.0f, 1.4f); // max = 2.4f
            float yFactor = 0.0366f * (y * y) - 0.1778f * y + 0.3021f;

            return new Vector2(xFactor, yFactor);
        }

        public void Pan(Vector2 delta)
        {
            Pan(delta.X, delta.Y);
        }

        public void Pan(float deltaX, float deltaY)
        {
            Vector2 panSpeed = GetPanSpeed();
            _focalPoint += -GetRightDirection() * deltaX * panSpeed.X * _distance;
            _focalPoint += GetUpDirection() * deltaY * panSpeed.Y * _distance;
            UpdateViewMatrix();
        }

        public void Rotate(Vector2 delta)
        {
            Rotate(delta.X, delta.Y);
        }

        public void Rotate(float deltaX,float deltaY)
        {
            const float RotationSpeed = 0.8f;
            float yawSign = GetUpDirection().Y < 0 ? -1.0f : 1.0f;
            _rotation.Y += yawSign * deltaX * RotationSpeed;
            _rotation.X += deltaY * RotationSpeed;

            UpdateViewMatrix();
        }

        public Vector3 GetUpDirection()
        {
            return GetOrientation() * Up;
        }

        public Vector3 GetRightDirection()
        {
            return GetOrientation() * Vector3.UnitX;
        }

        public Vector3 GetForwardDirection()
        {
            return GetOrientation() * Orientation;
        }

        private Vector3 CalculatePosition()
	    {
            Vector3 forwadDirection = GetForwardDirection();
            return FocalPoint - forwadDirection * Distance;
	    }

	    private Quaternion GetOrientation()
	    {
		    return new Quaternion(new Vector3(-Rotation));
	    }

        public override string ToString()
        {
            return $"FOV: {Fov}\nPosition: {WorldPosition}\nRotation: {Rotation}";
        }

    }
}
